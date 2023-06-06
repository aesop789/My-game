using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    [SerializeField] GameObject dialogueSing;
    [SerializeField] GameObject dialogueText;

    void Start()
    {
        dialogueSing.SetActive(true);
        dialogueText.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            dialogueSing.SetActive(false);
            dialogueText.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            dialogueSing.SetActive(true);
            dialogueText.SetActive(false);
        }
    }
}
