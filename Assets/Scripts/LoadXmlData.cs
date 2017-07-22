using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;

public class TranscriptNode
{
    public float start;
    public float duration;
    public string text;

    public TranscriptNode(float start, float duration, string text)
    {
        this.start = start;
        this.duration = duration;
        this.text = text;
    }
}

public class LoadXmlData
{
    void Start()
    {
        // GetData();
    }

    public void GetTranscript(ref TextAsset gameAsset, ref List<TranscriptNode> transcript)
    {
        if (gameAsset == null) return;

        transcript = new List<TranscriptNode>();

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(gameAsset.text);
        XmlNodeList textList = xmlDoc.GetElementsByTagName("transcript");

        foreach (XmlNode textNode in textList)
        {
            XmlNodeList textContent = textNode.ChildNodes;

            foreach (XmlNode node in textContent)
            {
                if (node.Name == "text")
                {
					// Debug.Log(node.Attributes["start"].Value);
					// Debug.Log(node.Attributes["dur"].Value);
                    // Debug.Log(node.InnerText);
                    transcript.Add(new TranscriptNode(float.Parse(node.Attributes["start"].Value), float.Parse(node.Attributes["dur"].Value), node.InnerText.Trim().ToLower()));
                }
            }
        }
    }

    public void GetNegWords(ref TextAsset libAsset, ref List<string> negWords)
	{
		if (libAsset == null) return;

        negWords = new List<string>();

		string fs = libAsset.text;
		string[] fLines = Regex.Split(fs, "\n|\r|\r\n");

        for (int i = 0; i < fLines.Length; i++)
        {
            fLines[i] = fLines[i].Trim();
            if (fLines[i] == "") continue;
            negWords.Add(fLines[i]);
		}
    }
}