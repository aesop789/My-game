using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTrap : MonoBehaviour
{
    [SerializeField] private Transform leftPosition;
    [SerializeField] private Transform rightPosition;
    [SerializeField] private float idleTime = 1f;
    [SerializeField] private float speed = 1f;
    [SerializeField] private int damage = 1;

    private Vector3 initScale;
    private bool movingLeft = false;
    private float idleTimer = 0;

    private Animator animator;

    private readonly int leftDir = -1;
    private readonly int rightDir = 1;

    private void Awake()
    {
        initScale = transform.localScale;
        animator = GetComponentInChildren<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Debug.Log("Player hit!");
            collision.GetComponent<PlayerController>().TakeDamage(damage);
        }
    }

    private void Update()
    {
        if (movingLeft)
        {
            if (transform.position.x >= leftPosition.position.x)
            {
                Move(leftDir);
            }
            else
            {
                ChangeDir();
            }
        }
        else
        {
            if (transform.position.x <= rightPosition.position.x)
            {
                Move(rightDir);
            }
            else
            {
                ChangeDir();
            }
        }
    }

    private void Move(int dir)
    {
        idleTimer = 0;
        //animator?.SetFloat("Move", 1f);

        transform.localScale = new Vector3(Mathf.Abs(initScale.x) * dir, initScale.y, initScale.z);

        transform.position = new Vector3(transform.position.x + Time.deltaTime * dir * speed,
                                              transform.position.y,
                                              transform.position.z);
    }

    private void ChangeDir()
    {
        idleTimer += Time.deltaTime;
        //animator?.SetFloat("Move", 0f);

        if (idleTimer > idleTime)
        {
            movingLeft = !movingLeft;
        }
    }

    private void OnDisable()
    {
        //animator?.SetFloat("Move", 0f);
    }
}
