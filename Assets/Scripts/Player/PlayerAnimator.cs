using UnityEngine;

namespace SupanthaPaul
{
	public class PlayerAnimator : MonoBehaviour
	{
		private Rigidbody2D m_rb;
		private PlayerController m_controller;
		private Animator m_anim;
		private static readonly int Move = Animator.StringToHash("Move");
		private static readonly int JumpState = Animator.StringToHash("JumpState");
		private static readonly int IsJumping = Animator.StringToHash("IsJumping");
		private static readonly int WallGrabbing = Animator.StringToHash("WallGrabbing");
		private static readonly int IsDashing = Animator.StringToHash("IsDashing");
		private static readonly int Attack = Animator.StringToHash("Attack");
		private static readonly int Die = Animator.StringToHash("Die");
		private static readonly int Hit = Animator.StringToHash("Hit");

		private void Start()
		{
			m_anim = GetComponentInChildren<Animator>();
			m_controller = GetComponent<PlayerController>();
			m_rb = GetComponent<Rigidbody2D>();
			m_controller.OnHealthChanged += (x) => { if (x > 0) m_anim.SetTrigger(Hit); };
			m_controller.OnDie += () => { m_anim.SetTrigger(Die); };
		}

		private void Update()
		{
			if (m_controller.isDead)
			{
				//m_anim.SetTrigger(Die);
				//m_anim.SetBool("Dead", true);
				return;
			}

			m_anim.SetFloat(Move, Mathf.Abs(m_rb.velocity.x));

			float verticalVelocity = m_rb.velocity.y;
			m_anim.SetFloat(JumpState, verticalVelocity);

			if (!m_controller.isGrounded && !m_controller.actuallyWallGrabbing)
			{
				m_anim.SetBool(IsJumping, true);
			}
			else
			{
				m_anim.SetBool(IsJumping, false);
			}

			if (!m_controller.isGrounded && m_controller.actuallyWallGrabbing)
			{
				m_anim.SetBool(WallGrabbing, true);
			}
			else
			{
				m_anim.SetBool(WallGrabbing, false);
			}

			m_anim.SetBool(IsDashing, m_controller.isDashing);
			if (m_controller.isAttacking)
			{
				m_anim.SetTrigger(Attack);
			}
		}
	}
}
