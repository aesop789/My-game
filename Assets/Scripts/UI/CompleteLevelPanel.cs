using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompleteLevelPanel : MonoBehaviour
{
    [SerializeField] GameObject Panel;

    CanvasGroup group;

    PlayerController player;
    CameraFollow camera;

    private void Awake()
    {
        group = GetComponent<CanvasGroup>();
        group.alpha = 0f;
        player = FindObjectOfType<PlayerController>();
        //player.OnDie += () => { Show(); };
        //player.enabled = false;
        camera = FindObjectOfType<CameraFollow>();
        //camera.enabled = false;
        Panel.SetActive(false);
    }

    public void Show()
    {
        player.enabled = false;
        camera.enabled = false;
        player.GetComponent<BoxCollider2D>().enabled = false;
        player.GetComponent<Rigidbody2D>().simulated = false;
        Panel.SetActive(true);
        StartCoroutine(ShowCoroutine());
    }

    IEnumerator ShowCoroutine()
    {
        yield return new WaitForSeconds(1f);

        group.DOFade(1, 1f).OnComplete(() => {
            /*Time.timeScale = 0f;
            InputSystem.SetPause(true);*/
        });// player.enabled = true; camera.enabled = true; Panel.SetActive(false); });

        yield return null;
    }
}
