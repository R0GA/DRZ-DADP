using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {   
            Vector3 resetLocation = other.gameObject.GetComponent<FirstPersonControls>().currentCheckpoint;

            other.gameObject.GetComponent<CharacterController>().enabled = false;
            other.gameObject.transform.position = resetLocation;
            other.gameObject.GetComponent<CharacterController>().enabled = true;
        }
    }
}
    