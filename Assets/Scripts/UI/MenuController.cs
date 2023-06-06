using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] private bool ingameMenu = false;
    [SerializeField] private GameObject mainPanel;

    private void Awake()
    {
        if (ingameMenu) mainPanel.SetActive(false);
    }

    public void Show()
    {
        if (ingameMenu)
        {
            PauseGame(true);
            mainPanel.SetActive(true);
        }
    }
    public void NewGame()
    {
        Debug.Log("New game");
        SceneManager.LoadScene("Level Intro");
    }

    public void ToMainMenu()
    {
        Debug.Log("Load Main menu");
        if (ingameMenu)
        {
            PauseGame(false);
        }
        SceneManager.LoadScene("Main menu");
    }

    public void RestartLevel()
    {
        Debug.Log("Restart level " + SceneManager.GetActiveScene().name);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Continue()
    {
        if (ingameMenu)
        {
            PauseGame(false);
        }
        mainPanel.SetActive(false);
    }

    public void PauseGame(bool pause)
    {
        Time.timeScale = pause ? 0f : 1f;
        InputSystem.SetPause(pause);
    }

    public void ExitGame()
    {
        Debug.Log("Exit game");
        Application.Quit();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !mainPanel.activeInHierarchy)
        {
            Show();
        }
    }
}