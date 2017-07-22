using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {

    [SerializeField]
	float speed = 3;

    [SerializeField]
    TextMesh text;

    public bool CanMove = true;

    float originX;

    void OnEnable()
    {
        Init("");
    }

    void Start()
    {
        originX = transform.position.x;
    }

    void Update()
    {
        if (CanMove)
        {
            MoveLeft();
        }
    }

    void MoveLeft()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;

        if (transform.position.x < -originX)
        {
            Destroy(gameObject);
        }
	}

	void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			CanMove = false;
		}
	}

	void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			CanMove = true;
		}
	}

    public void Init(string word)
    {
        if (text == null) return;

        text.text = word;
    }
}
