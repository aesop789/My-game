using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] StartPanel startPanel;
    [SerializeField] GameOverPanel gameOverPanel;
    [SerializeField] CompleteLevelPanel completeLevel;

    void Start()
    {
        gameOverPanel.gameObject.SetActive(false);
        completeLevel.gameObject.SetActive(false);

        PlayerController player = FindObjectOfType<PlayerController>();
        player.OnDie += () => { GameOver(); };
    }

    public void GameOver()
    {
        gameOverPanel.gameObject.SetActive(true);
        completeLevel.gameObject.SetActive(false);
        gameOverPanel.Show();
    }

    public void LevelComplete()
    {
        gameOverPanel.gameObject.SetActive(false);
        completeLevel.gameObject.SetActive(true);
        completeLevel.Show();
    }
}
