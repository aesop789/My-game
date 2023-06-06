using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPowerUp : MonoBehaviour
{
    [SerializeField] private int healtAmount = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Debug.Log("Health bonus!");
            collision.GetComponent<PlayerController>().AddHealth(healtAmount);
            gameObject.SetActive(false);
        }
    }
}
