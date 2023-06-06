using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour
{
    [SerializeField] private Transform leftPosition;
    [SerializeField] private Transform rightPosition;
    [SerializeField] private float idleTime = 1f;

    private EnemyController enemy;

    private Transform tr;
    private Vector3 initScale;
    private bool movingLeft = false;
    private float idleTimer = 0;

    private Animator animator;

    private readonly int leftDir = -1;
    private readonly int rightDir = 1;
    float speed = 0f;

    private void Awake()
    {
        enemy = GetComponent<EnemyController>();
        speed = enemy != null ? enemy.Speed : 1f;
        tr = enemy != null ? enemy.transform : transform;
        initScale = tr.localScale;
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (leftPosition == null || rightPosition == null)
            return;

        if (movingLeft)
        {
            if (tr.position.x >= leftPosition.position.x)
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
            if (tr.position.x <= rightPosition.position.x)
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
        animator?.SetFloat("Move", 1f);

        tr.localScale = new Vector3(Mathf.Abs(initScale.x) * dir, initScale.y, initScale.z);

        tr.position = new Vector3(tr.position.x + Time.deltaTime * dir * speed,
                                              tr.position.y,
                                              tr.position.z);
    }

    private void ChangeDir()
    {
        idleTimer += Time.deltaTime;
        animator?.SetFloat("Move", 0f);

        if (idleTimer > idleTime)
        {
            movingLeft = !movingLeft;
        }
    }

    private void OnDisable()
    {
        animator?.SetFloat("Move", 0f);
    }
}
