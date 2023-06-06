using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[SerializeField] private float speed;
	[SerializeField] private int maxHealth;
	[SerializeField] private float hitCooldown = 0.5f;
	HealthView healthBar = null;

	[SerializeField] private int attackDamage = 1;
	[SerializeField] private float attackCooldown = 0.5f;
	[SerializeField] private float attackRange;
	[SerializeField] private float attackSize;
	[SerializeField] private LayerMask attackLayer;
	[Header("Jumping")]
	[SerializeField] private float jumpForce;
	[SerializeField] private float fallMultiplier;
	[SerializeField] private Transform groundCheck;
	[SerializeField] private float groundCheckRadius;
	[SerializeField] private LayerMask groundLayer;
	[SerializeField] private LayerMask wallLayer;
	[SerializeField] private int extraJumpCount = 1;
	[SerializeField] private GameObject jumpEffect;
	[Header("Dashing")]
	[SerializeField] private float dashSpeed = 30f;
	[SerializeField] private float startDashTime = 0.1f;
	[SerializeField] private float dashCooldown = 0.2f;
	[SerializeField] private GameObject dashEffect;

	[HideInInspector] public bool isGrounded;
	[HideInInspector] public float moveInput;
	[HideInInspector] public bool canMove = true;
	[HideInInspector] public bool isDashing = false;
	[HideInInspector] public bool isAttacking = false;
	[HideInInspector] public bool actuallyWallGrabbing = false;
	[HideInInspector] public bool isCurrentlyPlayable = false;
	public bool isDead { get; private set; } = false;

	[Header("Wall grab & jump")]
	public Vector2 grabRightOffset = new Vector2(0.16f, 0f);
	public Vector2 grabLeftOffset = new Vector2(-0.16f, 0f);
	public float grabCheckRadius = 0.24f;
	public float slideSpeed = 2.5f;
	public Vector2 wallJumpForce = new Vector2(10.5f, 18f);
	public Vector2 wallClimbForce = new Vector2(4f, 14f);

	[Header("Audio")]
	[SerializeField] private AudioClip attackSound;
	[SerializeField] private AudioClip hitSound;
	[SerializeField] private AudioClip dieSound;

	private new AudioSource audio = null;

	private Rigidbody2D m_rb;
	[SerializeField] private BoxCollider2D col;
	private ParticleSystem m_dustParticle;
	private bool m_facingRight = true;
	private readonly float m_groundedRememberTime = 0.25f;
	private float m_groundedRemember = 0f;
	private int m_extraJumps;
	private float m_extraJumpForce;
	private float m_dashTime;
	private bool m_hasDashedInAir = false;
	private bool m_onWall = false;
	private bool m_onRightWall = false;
	private bool m_onLeftWall = false;
	private bool m_wallGrabbing = false;
	private readonly float m_wallStickTime = 0.25f;
	private float m_wallStick = 0f;
	private bool m_wallJumping = false;
	private float m_dashCooldown;
	private int m_health = 0;
	private float m_lastHitTime = float.MinValue;
	private float m_lastAttackTime = float.MinValue;

	// 0 -> none, 1 -> right, -1 -> left
	private int m_onWallSide = 0;
	private int m_playerSide = 1;

	public Action<int> OnHealthChanged;
	public Action OnDie;

	void Start()
	{
		PoolManager.instance.CreatePool(dashEffect, 2);
		PoolManager.instance.CreatePool(jumpEffect, 2);

		if (transform.CompareTag("Player"))
		{
			isCurrentlyPlayable = true;
		}

		m_extraJumps = extraJumpCount;
		m_dashTime = startDashTime;
		m_dashCooldown = dashCooldown;
		m_extraJumpForce = jumpForce * 0.7f;
		m_health = maxHealth;
		isDead = false;

		m_rb = GetComponent<Rigidbody2D>();
		col = GetComponent<BoxCollider2D>();
		m_dustParticle = GetComponentInChildren<ParticleSystem>();
		m_rb.simulated = true;
		audio = GetComponent<AudioSource>();
		var healthBars = FindObjectsOfType<HealthView>();
		foreach (var bar in healthBars)
		{
			if (bar.gameObject.layer == LayerMask.NameToLayer("Player"))
			{
				healthBar = bar;
				break;
			}
		}
	}

	private void FixedUpdate()
	{
		if (isDead)
		{
			return;
		}

		isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
		var position = transform.position;

		m_onWall = Physics2D.OverlapCircle((Vector2)position + grabRightOffset, grabCheckRadius, wallLayer)
			        || Physics2D.OverlapCircle((Vector2)position + grabLeftOffset, grabCheckRadius, wallLayer);
		m_onRightWall = Physics2D.OverlapCircle((Vector2)position + grabRightOffset, grabCheckRadius, wallLayer);
		m_onLeftWall = Physics2D.OverlapCircle((Vector2)position + grabLeftOffset, grabCheckRadius, wallLayer);

		CalculateSides();

		if((m_wallGrabbing || isGrounded) && m_wallJumping)
		{
			m_wallJumping = false;
		}

		if (isCurrentlyPlayable)
		{
			if(m_wallJumping)
			{
				m_rb.velocity = Vector2.Lerp(m_rb.velocity, (new Vector2(moveInput * speed, m_rb.velocity.y)), 1.5f * Time.fixedDeltaTime);
			}
			else
			{
				if (canMove && !m_wallGrabbing)
				{
					m_rb.velocity = new Vector2(moveInput * speed, m_rb.velocity.y);
				}
				else if (!canMove)
				{
					m_rb.velocity = new Vector2(0f, m_rb.velocity.y);
				}
			}

			if (m_rb.velocity.y < 0f)
			{
				m_rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
			}

			if (!m_facingRight && moveInput > 0f)
			{
				Flip();
			}
			else if (m_facingRight && moveInput < 0f)
			{
				Flip();
			}

			if (isDashing)
			{
				if (m_dashTime <= 0f)
				{
					isDashing = false;
					m_dashCooldown = dashCooldown;
					m_dashTime = startDashTime;
					m_rb.velocity = Vector2.zero;
				}
				else
				{
					m_dashTime -= Time.deltaTime;
					if (m_facingRight)
					{
						m_rb.velocity = Vector2.right * dashSpeed;
					}
					else
					{
						m_rb.velocity = Vector2.left * dashSpeed;
					}
				}
			}

			if(m_onWall && !isGrounded && m_rb.velocity.y <= 0f && m_playerSide == m_onWallSide)
			{
				actuallyWallGrabbing = true;
				m_wallGrabbing = true;
				m_rb.velocity = new Vector2(moveInput * speed, -slideSpeed);
				m_wallStick = m_wallStickTime;
			}
			else
			{
				m_wallStick -= Time.deltaTime;
				actuallyWallGrabbing = false;
				if (m_wallStick <= 0f)
				{
					m_wallGrabbing = false;
				}
			}
			if (m_wallGrabbing && isGrounded)
			{
				m_wallGrabbing = false;
			}

			float playerVelocityMag = m_rb.velocity.sqrMagnitude;
			if(m_dustParticle.isPlaying && playerVelocityMag == 0f)
			{
				m_dustParticle.Stop();
			}
			else if(!m_dustParticle.isPlaying && playerVelocityMag > 0f)
			{
				m_dustParticle.Play();
			}

		}
	}

	private void Update()
	{
		if (isDead)
		{
			return;
		}

		moveInput = InputSystem.HorizontalRaw();

		if (isGrounded)
		{
			m_extraJumps = extraJumpCount;
		}

		m_groundedRemember -= Time.deltaTime;
		if (isGrounded)
		{
			m_groundedRemember = m_groundedRememberTime;
		}

		if (!isCurrentlyPlayable)
		{
			return;
		}

		if (!isDashing && !m_hasDashedInAir && m_dashCooldown <= 0f)
		{
			if (InputSystem.Dash())
			{
				isDashing = true;
				PoolManager.instance.ReuseObject(dashEffect, transform.position, Quaternion.identity);
				if(!isGrounded)
				{
					m_hasDashedInAir = true;
				}
			}
		}
		m_dashCooldown -= Time.deltaTime;

		if (m_hasDashedInAir && isGrounded)
		{
			m_hasDashedInAir = false;
		}

		if (isGrounded)
		{
			if (m_lastAttackTime + attackCooldown <= Time.time)
			{
				isAttacking = InputSystem.Attack();
				if (isAttacking)
				{
					m_lastAttackTime = Time.time;
					audio.PlayOneShot(attackSound);
				}
			}
			else
			{
				isAttacking = false;
			}
		}
		else
        {
			isAttacking = false;
        }

		if(InputSystem.Jump() && m_extraJumps > 0 && !isGrounded && !m_wallGrabbing)
		{
			m_rb.velocity = new Vector2(m_rb.velocity.x, m_extraJumpForce);
			m_extraJumps--;
			PoolManager.instance.ReuseObject(jumpEffect, groundCheck.position, Quaternion.identity);
		}
		else if(InputSystem.Jump() && (isGrounded || m_groundedRemember > 0f))
		{
			m_rb.velocity = new Vector2(m_rb.velocity.x, jumpForce);
			PoolManager.instance.ReuseObject(jumpEffect, groundCheck.position, Quaternion.identity);
		}
		else if(InputSystem.Jump() && m_wallGrabbing && moveInput!=m_onWallSide )
		{
			m_wallGrabbing = false;
			m_wallJumping = true;
			Debug.Log("Wall jumped");
			if (m_playerSide == m_onWallSide)
			{
				Flip();
			}
			m_rb.AddForce(new Vector2(-m_onWallSide * wallJumpForce.x, wallJumpForce.y), ForceMode2D.Impulse);
		}
		else if(InputSystem.Jump() && m_wallGrabbing && moveInput != 0 && (moveInput == m_onWallSide))
		{
			m_wallGrabbing = false;
			m_wallJumping = true;
			Debug.Log("Wall climbed");
			if (m_playerSide == m_onWallSide)
			{
				Flip();
			}
			m_rb.AddForce(new Vector2(-m_onWallSide * wallClimbForce.x, wallClimbForce.y), ForceMode2D.Impulse);
		}

	}

	void Flip()
	{
		m_facingRight = !m_facingRight;
		Vector3 scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;
	}

	void CalculateSides()
	{
		if (m_onRightWall)
		{
			m_onWallSide = 1;
		}
		else if (m_onLeftWall)
		{
			m_onWallSide = -1;
		}
		else
		{
			m_onWallSide = 0;
		}

		if (m_facingRight)
		{
			m_playerSide = 1;
		}
		else
		{
			m_playerSide = -1;
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
				audio.PlayOneShot(hitSound);
				OnHealthChanged?.Invoke(m_health);
			}
			healthBar?.UpdateHealth(m_health, maxHealth);
		}
	}

	private void Die()
    {
		m_rb.simulated = false;
		isDead = true;
		OnDie?.Invoke();
		audio.PlayOneShot(dieSound);
	}

	public void DoDamage()
    {
		RaycastHit2D[] results = Physics2D.BoxCastAll(col.bounds.center + transform.right * attackRange * transform.localScale.x,
											 new Vector3(attackSize, col.bounds.size.y, col.bounds.size.z),
											 0,
											 Vector2.left,
											 0,
											 attackLayer);
		foreach(var hit in results)
		{
			hit.collider.GetComponent<EnemyController>().TakeDamage(attackDamage);
		}
	}

	public void AddHealth(int amount)
    {
		m_health = Mathf.Clamp(m_health + amount, 0, maxHealth);
		healthBar?.UpdateHealth(m_health, maxHealth);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
		Gizmos.DrawWireSphere((Vector2)transform.position + grabRightOffset, grabCheckRadius);
		Gizmos.DrawWireSphere((Vector2)transform.position + grabLeftOffset, grabCheckRadius);
		Gizmos.DrawWireCube(col.bounds.center + transform.right * attackRange * transform.localScale.x,
							new Vector3(attackSize, col.bounds.size.y, col.bounds.size.z));
	}
}
