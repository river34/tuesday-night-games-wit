using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public static PlayerController Instance;

    [SerializeField]
    Animator animator;

    bool isPointerDown = false, lastIsPointerDown = false;
    public float holdingTime = -1;
    float velocity = -1;
    float originY;
    Vector3 position;
    IEnumerator coroutine;

    const float GRAVITY = 40;
    const float V0 = 100;

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
        originY = transform.position.y;
    }

    void Update()
    {
		if (Input.GetKey(KeyCode.Space))
		{
            isPointerDown = true;
		}
        else
        {
            isPointerDown = false;   
        }

        if (coroutine == null && !lastIsPointerDown && isPointerDown)
        {
            holdingTime = Time.time;
        }

        if (coroutine == null && lastIsPointerDown && !isPointerDown)
		{
            holdingTime = (Time.time - holdingTime > Time.deltaTime * 4) ? Time.deltaTime * 4 : Time.time - holdingTime;
			StartJumping();
		}
    }

    void LateUpdate()
    {
        lastIsPointerDown = isPointerDown;
    }

    void StartJumping()
    {
		if (animator == null) return;

		animator.SetBool("Jump", true);

        velocity = V0 * holdingTime;

        coroutine = Jumping();

        StartCoroutine(coroutine);
	}

    IEnumerator Jumping()
    {
        while(transform.position.y >= originY)
		{
			yield return new WaitForSeconds(Time.deltaTime);
            position = transform.position;
			position.y += velocity * Time.deltaTime;
			velocity -= GRAVITY * Time.deltaTime;
            transform.position = position;
        }

        position.y = originY;
        transform.position = position;
		EndJumping();
		holdingTime = -1;
        velocity = -1;
        coroutine = null;
    }

	void EndJumping()
	{
		if (animator == null) return;

		animator.SetBool("Jump", false);
	}

    public void StartWalking()
    {
		if (animator == null) return;

		animator.SetBool("Walk", true);
	}

    public void StopWalking()
	{
		if (animator == null) return;

		animator.SetBool("Walk", false);
    }

	void EndWalking()
	{
		if (animator == null) return;

		animator.SetBool("Walk", false);
	}

	void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Block"))
		{
            SoundManager.Instance.PauseVO();
            StopWalking();
		}
	}

	void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Block"))
		{
            SoundManager.Instance.ResumeVO();
            StartWalking();
		}
	}
}
