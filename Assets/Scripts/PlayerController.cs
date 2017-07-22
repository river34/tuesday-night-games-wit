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
    float minX, maxX, minY;
    Vector3 position;
    IEnumerator coroutine;

    const float GRAVITY = 40;
    const float V0 = 200;
	const float FORWARD_SPEED = 4;
	const float BACKWARD_SPEED = -2;

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
        minX = transform.position.x;
        maxX = -minX;
        minY = transform.position.y;
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
            holdingTime = (Time.time - holdingTime > Time.deltaTime * 3) ? Time.deltaTime * 3 : Time.time - holdingTime;
			StartJumping();
		}

        if (coroutine == null)
        {
			position = transform.position;
            position.x += BACKWARD_SPEED * Time.deltaTime;
			if (position.x < minX)
			{
                position.x = minX;
			}
			transform.position = position;
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
        while(transform.position.y >= minY)
		{
			yield return new WaitForSeconds(Time.deltaTime);
            position = transform.position;
            position.x += FORWARD_SPEED * Time.deltaTime;
            if (position.x > maxX)
            {
                position.x = maxX;
            }
			position.y += velocity * Time.deltaTime;
			velocity -= GRAVITY * Time.deltaTime;
            transform.position = position;
        }

        position = transform.position;
        position.y = minY;
        transform.position = position;
		EndJumping();
		holdingTime = -1;
        velocity = -1;
        coroutine = null;
    }

    void StopJumping()
    {
		position = transform.position;
		position.y = minY;
		transform.position = position;
		EndJumping();
		holdingTime = -1;
		velocity = -1;

        if (coroutine != null)
		{
			StopCoroutine(coroutine);
			coroutine = null;
        }
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
            // SoundManager.Instance.PauseVO();
            VideoManager.Instance.PauseVO();
            StopWalking();
            StopJumping();
		}
	}

	void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Block"))
		{
			// SoundManager.Instance.ResumeVO();
			VideoManager.Instance.ResumeVO();
            StartWalking();
		}
	}
}
