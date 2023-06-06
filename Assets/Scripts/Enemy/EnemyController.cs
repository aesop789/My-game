using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private int attackDamage;
    [SerializeField] private float attackRange;
    [SerializeField] private float attackSize;
    [SerializeField] private float attackCooldown;
    [SerializeField] private BoxCollider2D col;
    [SerializeField] private LayerMask attackLayer;
    [SerializeField] private bool ranged = false;
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform projectileRoot;
    [SerializeField] private float speed = 1f;
    public float Speed { get { return speed; } }

    HealthView healthBar = null;
    [SerializeField] private int maxHealth;
    [SerializeField] private float hitCooldown = 0.5f;
    public bool isDead { get; private set; } = false;


    [Header("Audio")]
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip dieSound;

    private new AudioSource audio = null;

    private float lastAttackTime = float.MinValue;
    private Animator animator;

    private static readonly int AttackAnim = Animator.StringToHash("Attack");
    private static readonly int DieAnim = Animator.StringToHash("Die");
    private static readonly int HitAnim = Animator.StringToHash("Hit");

    private PlayerController player = null;
    private Patrol patrol;
    private Rigidbody2D m_rb;

    private int m_health = 0;
    private float m_lastHitTime = float.MinValue;

    public Action<int> OnHealthChanged;
    public Action OnDie;

    private void Awake()
    {
        if (col == null)
        {
            col = GetComponent<BoxCollider2D>();
            if (col == null)
            {
                Debug.LogError("No box collider!");
            }
        }
        animator = GetComponentInChildren<Animator>();
        patrol = GetComponent<Patrol>();
        m_rb = GetComponent<Rigidbody2D>();
        m_rb.simulated = true;
        m_health = maxHealth;
        isDead = false;
        audio = GetComponent<AudioSource>();
        var healthBars = FindObjectsOfType<HealthView>();
        foreach (var bar in healthBars)
        {
            if (bar.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                healthBar = bar;
                break;
            }
        }
    }

    private void Update()
    {
        if (isDead)
            return;

        if (PlayerInReach())
        {
            if (lastAttackTime + attackCooldown < Time.time)
            {
                Attack();
            }
        }

        if (patrol != null)
        {
            var info = animator.GetCurrentAnimatorStateInfo(0);

            if (info.IsName("Idle") || info.IsName("Run"))
                patrol.enabled = !PlayerInReach();
            else
                patrol.enabled = false;
        }
    }

    private bool PlayerInReach()
    {
        RaycastHit2D hit = Physics2D.BoxCast(col.bounds.center + transform.right * attackRange * transform.localScale.x,
                                             new Vector3(attackSize, col.bounds.size.y, col.bounds.size.z),
                                             0,
                                             Vector2.left,
                                             0,
                                             attackLayer);
        if (hit.collider != null)
        {
            player = hit.collider.GetComponent<PlayerController>();
        }

        return hit.collider != null;
    }

    public void Attack()
    {
        lastAttackTime = Time.time;
        animator.SetTrigger(AttackAnim);
        audio.PlayOneShot(attackSound);
    }

    public void DoDamage()
    {
        if (!ranged)
        {
            if (PlayerInReach())
            {
                player.TakeDamage(attackDamage);
            }
        }
        else
        {
            var pr = Instantiate(projectile, projectileRoot.position, projectileRoot.rotation);
            pr.transform.localScale = transform.localScale;
        }
    }

    public void TakeDamage(int amount)
    {
        if (m_lastHitTime + hitCooldown < Time.time && !isDead)
        {
            m_lastHitTime = Time.time;
            m_health -= amount;

            if (m_health <= 0)
            {
                Die();
            }
            else
            {
                OnHealthChanged?.Invoke(m_health);
                animator.SetTrigger(HitAnim);
                audio.PlayOneShot(hitSound);
            }
            healthBar?.UpdateHealth(m_health, maxHealth);
        }
    }

    private void Die()
    {
        m_rb.simulated = false;
        isDead = true;
        audio.PlayOneShot(dieSound);
        OnDie?.Invoke();
        animator.SetTrigger(DieAnim);
        if (patrol != null)
        {
            patrol.enabled = false;
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(col.bounds.center + transform.right * attackRange * transform.localScale.x,
                            new Vector3(attackSize, col.bounds.size.y, col.bounds.size.z));
    }
}
