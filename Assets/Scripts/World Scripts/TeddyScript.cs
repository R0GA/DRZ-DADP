using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TeddyScript : MonoBehaviour
{
    public GameObject introPanel;
    public bool uiActive;
    public TMP_Text closeTXT;
    public FirstPersonControls player;
    public GameObject ctrlrIntoImg;
    public GameObject mnkIntoImg;

    private void Awake()
    {
        introPanel.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (player.currentInput == "Keyboard")
            {
                closeTXT.text = "Press Escape To Close";
                ctrlrIntoImg.SetActive(false);
                mnkIntoImg.SetActive(true);
            }
            else if (player.currentInput == "Gamepad")
            {
                closeTXT.text = "Press Start To Close";
                mnkIntoImg.SetActive(false);
                ctrlrIntoImg.SetActive(true);
            }

            introPanel.SetActive(true);
            uiActive = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            introPanel.SetActive(false);
            uiActive=false;
        }
    }
    public void ClosePanel()
    {
        introPanel.SetActive(false);
        uiActive =false;
    }
}
