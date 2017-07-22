using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public static GameController Instance;

    [SerializeField]
	AudioClip voiceover = null;

    [SerializeField]
    VideoClip video = null;

    [SerializeField]
    TextAsset gameAsset = null;

    [SerializeField]
    TextAsset libAsset = null;

    [SerializeField]
    GameObject block = null;

    [SerializeField]
    GameObject startScene = null;

    [SerializeField]
	GameObject inputScene = null;

	[SerializeField]
	Text caption = null;

    [SerializeField]
    Sprite[] sprites = null;

	List<TranscriptNode> transcript = null;

    List<string> negWords = null;

    public enum State { Init, Start, Input, Game, NumOfStates };

    public State currentState, lastState;

    float startTime = -1;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);    
        }

        DontDestroyOnLoad(this.gameObject);
    }

    void Starte()
    {
        currentState = State.Init;
    }

    void Update()
    {
        if (Input.GetKeyDown("1"))
        {
            InitGame();

            GoToNextState();
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (lastState != currentState)
        {
            Debug.Log("state : " + currentState);
        }

		if (currentState == State.Init)
		{
			PlayInit();
		}

        if (currentState == State.Start)
        {
            PlayStart();
        }

        if (currentState == State.Input)
        {
            PlayInput();
        }

		if (currentState == State.Game)
		{
			PlayGame();
		}
    }

    void LateUpdate()
    {
        lastState = currentState;
    }

    void GoToNextState()
    {
        if (currentState > State.NumOfStates - 1) return;

        currentState++;
    }

    void PlayInit()
    {
        GoToNextState();
    }

    void PlayStart()
    {
        if (lastState != currentState)
		{
			startTime = Time.time;
			startScene.SetActive(true);
			inputScene.SetActive(false);
			caption.gameObject.SetActive(false);
        }

        if (Time.time - startTime > 5 || Input.anyKey)
        {
            startTime = -1;
			GoToNextState();
        }
    }

    void PlayInput()
    {
		if (lastState != currentState)
		{
			startScene.SetActive(false);
			inputScene.SetActive(true);
			caption.gameObject.SetActive(false);
		}
    }

    void PlayGame()
    {
		if (lastState != currentState)
		{
			InitGame();
			startScene.SetActive(false);
			inputScene.SetActive(false);
			caption.gameObject.SetActive(true);
		}
	}

	void InitGame()
	{
		if (gameAsset == null) return;
		// if (voiceover == null) return;
		if (video == null) return;

		Debug.Log("Load data. Init game");

		LoadXmlData loader = new LoadXmlData();
		loader.GetTranscript(ref gameAsset, ref transcript);
		loader.GetNegWords(ref libAsset, ref negWords);

		// SoundManager.Instance.PlayVO(ref voiceover, ref transcript);
		VideoManager.Instance.PlayVO(ref video, ref transcript);

		PlayerController.Instance.StartWalking();
	}

    void GenerateBlocks(TranscriptNode node)
    {
        int num = 0;
        foreach (string word in negWords)
        {
            if (Regex.IsMatch(node.text, @"\b" + word + @"\b", RegexOptions.Singleline | RegexOptions.IgnoreCase))
            {
                Debug.Log(word);
                CreateBlock(word, node.duration + num * 0.2f);
                num++;
            }
        }

        if (caption == null) return;
        caption.text = node.text; 
    }

    void CreateBlock(string word, float delay)
    {
		if (block == null) return;

        StartCoroutine(CreateBlockInDelay(word, delay));
    }

    IEnumerator CreateBlockInDelay(string word, float delay)
    {
        if (sprites == null) yield return null;

        yield return new WaitForSeconds(delay);

		GameObject newBlock = Instantiate<GameObject>(block, block.transform.parent);
        newBlock.transform.localScale *= Random.Range(0.6f, 1);
        newBlock.GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];
		newBlock.SetActive(true);
		newBlock.GetComponent<Block>().Init(word);
    }

    public void OnEnterNode(int nodeIndex)
    {
        GenerateBlocks(transcript[nodeIndex]);
    }

    public void OnGetInput()
    {
        GoToNextState();
    }
}
