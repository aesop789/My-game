using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEvents : MonoBehaviour
{
    [SerializeField] EnemyController enemy;
    [SerializeField] private UnityEvent customEvent;
    PlayerController player;

    private void Awake()
    {
        if (enemy == null)
        {
            player = GetComponentInParent<PlayerController>();
        }
    }
    public void DoDamage()
    {
        enemy?.DoDamage();
        player?.DoDamage();
    }

    public void HideObject()
    {
        enemy?.Hide();
        //player?.DoDamage();
    }

    public void CustomEvent()
    {
        customEvent?.Invoke();
    }
}
