using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeddyScript : MonoBehaviour
{
    public GameObject introPanel;

    private void Awake()
    {
        introPanel.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            introPanel.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            introPanel.SetActive(false);
        }
    }
    public void ClosePanel()
    {
        introPanel.SetActive(false);
    }
}
