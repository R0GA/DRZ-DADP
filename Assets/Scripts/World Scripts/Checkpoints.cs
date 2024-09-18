using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Checkpoints : MonoBehaviour
{
    private ParticleSystem checkpointParticles;

    private void Awake()
    {
        checkpointParticles = GetComponentInChildren<ParticleSystem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<FirstPersonControls>().currentCheckpoint = transform.position;
            checkpointParticles.Stop();
            //Debug.Log(other.gameObject.GetComponent<FirstPersonControls>().currentCheckpoint);
        }
    }
}
