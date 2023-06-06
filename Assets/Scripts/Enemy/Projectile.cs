using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private int damageAmount = 1;
    [SerializeField] private float speed = 1f;

    private int dir = 1;
    void Start()
    {
        if (transform.localScale.x > 0) dir = 1;
        else dir = -1;
        Destroy(gameObject, 10f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += dir * transform.right * speed * Time.deltaTime;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Debug.Log("Projectile hit!");
            collision.GetComponent<PlayerController>().TakeDamage(damageAmount);
            Destroy(gameObject);
        }
    }
}
