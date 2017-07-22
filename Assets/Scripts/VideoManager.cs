using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour {

	public static VideoManager Instance;

	[SerializeField]
	VideoPlayer video = null;

	[SerializeField]
    AudioSource audio = null;

    List<TranscriptNode> transcript = null;

	bool isVOPlaying = false, lastIsVOPlaying = false;
    bool isInNode = false;
    float startTime = -1;
    int nodeIndex = -1;

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

    void Start()
    {
        if (video == null) return;

        video.targetCamera = Camera.main;
    }

	void Update()
	{
		if (video == null) return;
        if (transcript == null) return;

		if (video.isPlaying)
		{
			isVOPlaying = true;
		}
		else
		{
			isVOPlaying = false;
		}

		if (lastIsVOPlaying && !isVOPlaying)
		{
			OnVOCompleted();
		}

        if (isVOPlaying)
        {
            if (nodeIndex < 0)
            {
                if (nodeIndex > transcript.Count - 1) return;

                nodeIndex++;
                startTime = Time.time;
            }

            if (Time.time - startTime >= transcript[nodeIndex].start && !isInNode)
			{
                isInNode = true;
                OnEnterNode();
			}

            if (Time.time - startTime >= transcript[nodeIndex].start + transcript[nodeIndex].duration && isInNode)
			{
				if (nodeIndex > transcript.Count - 1) return;

				nodeIndex++;
                isInNode = false;
			}
        }
	}

	void LateUpdate()
	{
		lastIsVOPlaying = isVOPlaying;
	}

	void OnVOCompleted()
	{
		Debug.Log("VO completed");
	}

    void OnEnterNode()
	{
		Debug.Log(nodeIndex + " : " + transcript[nodeIndex].text);
        GameController.Instance.OnEnterNode(nodeIndex);
	}

    public void PlayVO(ref VideoClip clip, ref List<TranscriptNode> transcript)
	{
		if (video == null) return;
		if (audio == null) return;
		if (clip == null) return;
		if (transcript == null) return;

        this.transcript = transcript;
		video.clip = clip;
        // video.SetTargetAudioSource(0, audio);
		video.Play();
	}

	public void PauseVO()
	{
		if (video == null) return;

		video.Pause();
	}

	public void ResumeVO()
	{
		if (video == null) return;

        video.Play();
	}
}
