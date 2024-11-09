using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class WinScript : MonoBehaviour
{
    public GameObject winPanel;
    [SerializeField]
    private FirstPersonControls fpc;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            winPanel.SetActive(true);
            UnityEngine.Cursor.visible = true;
            Time.timeScale = 0f;
            fpc.paused = true;
        }
    }

}
