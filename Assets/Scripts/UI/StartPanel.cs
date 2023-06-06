using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPanel : MonoBehaviour
{
    [SerializeField] GameObject Panel;

    CanvasGroup group;

    PlayerController player;
    CameraFollow camera;

    private void Awake()
    {
        group = GetComponent<CanvasGroup>();
        group.alpha = 1f;
        player = FindObjectOfType<PlayerController>();
        player.enabled = false;
        camera = FindObjectOfType<CameraFollow>();
        camera.enabled = false;
        Panel.SetActive(true);
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);

        group.DOKill();
        group.DOFade(0, 1f).OnComplete(()=> { player.enabled = true; camera.enabled = true;  Panel.SetActive(false); });
    }

    private void OnDisable()
    {
        group.DOKill();
    }
}
