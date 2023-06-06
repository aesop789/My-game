using UnityEngine;

public class InputSystem : MonoBehaviour
{
	// input string caching
	static readonly string HorizontalInput = "Horizontal";
	static readonly string JumpInput = "Jump";
	static readonly string DashInput = "Dash";
	static readonly string AttackInput = "Fire1";

	static private bool paused = false;

	public static void SetPause(bool pause)
    {
		paused = pause;
    }

	public static float HorizontalRaw()
	{
		if (paused) return 0f;

		return Input.GetAxisRaw(HorizontalInput);
	}

	public static bool Jump()
	{
		if (paused) return false;

		return Input.GetButtonDown(JumpInput);
	}

	public static bool Dash()
	{
		if (paused) return false;

		return Input.GetButtonDown(DashInput);
	}

	public static bool Attack()
	{
		if (paused) return false;

		return Input.GetButtonDown(AttackInput);
	}
}
