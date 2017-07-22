using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {

    [SerializeField]
	float speed = 3;

    [SerializeField]
    TextMesh text;

    [SerializeField]
	float minX;

	[SerializeField]
    float maxX;

    float originX;

    public bool CanMove = true;

    void OnEnable()
    {
        Init("");
    }

    void Start()
    {
        originX = transform.position.x;
        transform.position = new Vector3(maxX, transform.position.y, transform.position.z);
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
        if (transform.position.x > originX || transform.position.x < -originX)
        {
            transform.position += Vector3.left * speed * 1.5f * Time.deltaTime;
        }
        else
        {
            transform.position += Vector3.left * speed * Time.deltaTime;
        }

        if (transform.position.x < minX)
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
