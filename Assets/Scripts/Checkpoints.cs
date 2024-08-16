using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Checkpoints : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<FirstPersonControls>().currentCheckpoint = transform.position;
            //Debug.Log(other.gameObject.GetComponent<FirstPersonControls>().currentCheckpoint);
        }
    }
}
