using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	public static SoundManager Instance;

	[SerializeField]
	AudioSource voiceover = null;

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

	void Update()
	{
		if (voiceover == null) return;
        if (transcript == null) return;

		if (voiceover.isPlaying)
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

    public void PlayVO(ref AudioClip clip, ref List<TranscriptNode> transcript)
	{
		if (voiceover == null) return;
		if (clip == null) return;
		if (transcript == null) return;

        this.transcript = transcript;
		voiceover.clip = clip;
		voiceover.Play();
	}

	public void PauseVO()
	{
		if (voiceover == null) return;

		voiceover.Pause();
	}

	public void ResumeVO()
	{
		if (voiceover == null) return;

		voiceover.UnPause();
	}
}
