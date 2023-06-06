using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelComplete : MonoBehaviour
{
    [SerializeField] string nextLevelName = null;
    [SerializeField] UnityEvent OnCompleteLevel = null;

    public void CompleteLevel()
    {
        OnCompleteLevel?.Invoke();
    }

    public void GoToNextLevel()
    {
        Time.timeScale = 1f;
        InputSystem.SetPause(false);
        SceneManager.LoadScene(nextLevelName);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Debug.Log("Complete Level!");
            CompleteLevel();
        }
    }
}
