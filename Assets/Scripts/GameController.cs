using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class GameController : MonoBehaviour {

    public static GameController Instance;

    [SerializeField]
	AudioClip voiceover = null;

    [SerializeField]
    TextAsset gameAsset = null;

    [SerializeField]
    TextAsset libAsset = null;

    [SerializeField]
    GameObject block = null;

	List<TranscriptNode> transcript = null;

    List<string> negWords = null;

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

    void Update()
    {
        if (Input.GetKeyDown("1"))
        {
            Init();
        }
    }

    void Init()
    {
		if (voiceover == null) return;
		if (gameAsset == null) return;

		Debug.Log("Load data. Init game");

		LoadXmlData loader = new LoadXmlData();
        loader.GetTranscript(ref gameAsset, ref transcript);
        loader.GetNegWords(ref libAsset, ref negWords);

		SoundManager.Instance.PlayVO(ref voiceover, ref transcript);

        PlayerController.Instance.StartWalking();
    }

    void GenerateBlocks(TranscriptNode node)
    {
        foreach (string word in negWords)
        {
            if (Regex.IsMatch(node.text, @"\b" + word + @"\b", RegexOptions.Singleline | RegexOptions.IgnoreCase))
            {
                Debug.Log(word);
                CreateBlock(word, node.duration);
                return;
            }
        }
    }

    void CreateBlock(string word, float delay)
    {
		if (block == null) return;

        StartCoroutine(CreateBlockInDelay(word, delay));
    }

    IEnumerator CreateBlockInDelay(string word, float delay)
    {
        yield return new WaitForSeconds(delay);

		GameObject newBlock = Instantiate<GameObject>(block, block.transform.parent);
		newBlock.SetActive(true);
		newBlock.GetComponent<Block>().Init(word);
    }

    public void OnEnterNode(int nodeIndex)
    {
        GenerateBlocks(transcript[nodeIndex]);
    }
}
