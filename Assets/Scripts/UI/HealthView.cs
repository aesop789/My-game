using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthView : MonoBehaviour
{
    [SerializeField] GameObject healthPanel;
    [SerializeField] Image healthBar;
    [SerializeField] bool hideAfterTime = false;
    [SerializeField] float hideTimeout = 3f;

    private Coroutine coroutine = null;

    void Start()
    {
        
    }

    public void UpdateHealth(int current, int max)
    {
        //if (!healthPanel.activeInHierarchy)
            healthPanel.SetActive(true);

        healthBar.fillAmount = 1f * current / max;

        if (hideAfterTime)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            coroutine = StartCoroutine(HideAfterTime(hideTimeout));
        }
    }

    private IEnumerator HideAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        healthPanel.SetActive(false);
        coroutine = null;
    }
}
