using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAudio : MonoBehaviour
{
    [SerializeField]
    private AudioClip yawn1;
    [SerializeField] 
    private AudioClip yawn2;
    [SerializeField] 
    private AudioClip music;
    [SerializeField]
    private AudioSource audioSource1;
    [SerializeField]
    private AudioSource audioSource2;
    private int yawnNum = 1;

    private void Start()
    {
        StartCoroutine(PlayRandomYawn());

        audioSource2.loop = true;
        audioSource2.clip = music;
        audioSource2.Play();
    }

    private IEnumerator PlayRandomYawn()
    {
        while(true)
        {
            float waitTime = Random.Range(10f, 15f);
            yield return new WaitForSeconds(waitTime);

            if(yawnNum == 1)
            {
                audioSource1.PlayOneShot(yawn1);
                yawnNum = 2;
            }
            else if(yawnNum == 2)
            {
                audioSource1.PlayOneShot(yawn2);
                yawnNum = 1;
            }

        }
    }



}
