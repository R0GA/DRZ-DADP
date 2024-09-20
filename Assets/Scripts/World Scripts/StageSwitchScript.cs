using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StageSwitchScript : MonoBehaviour
{
    public Transform level2Start;
    public ParticleSystem smoke1;
    public ParticleSystem smoke2;

    private void Awake()
    {
        smoke1.Stop();
        smoke2.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            this.gameObject.GetComponent<MeshRenderer>().enabled = false;
            StartCoroutine(TeleportAfterDelay(other.gameObject));
        }
    }

    IEnumerator TeleportAfterDelay(GameObject player)
    {
        player.gameObject.GetComponent<CharacterController>().enabled = false;
        smoke1.Play();
        smoke2.Play();

        yield return new WaitForSeconds(3);
        
        player.gameObject.transform.position = level2Start.position;
        player.gameObject.GetComponent<CharacterController>().enabled = true;

        yield return new WaitForSeconds(1);
        smoke1.Stop();
        smoke2.Stop();
    }

}
