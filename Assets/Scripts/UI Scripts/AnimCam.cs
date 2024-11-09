using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimCam : MonoBehaviour
{
    [SerializeField]
    private int animLength;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Camera mainCam;
    [SerializeField]
    private Camera animCam;
    [SerializeField]
    private UIManager uiManager;
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip click;
    [SerializeField]
    private AudioClip laugh;


    public void Play()
    {
        animCam.gameObject.SetActive(true);
        mainCam.gameObject.SetActive(false);

        //animator.SetInteger("AnimState", 1);

        foreach (GameObject UIelement in uiManager.UIElements)
        {
            UIelement.SetActive(false);
        }

        StartCoroutine(PlayAnimationAndSwitchScene());
        StartCoroutine(PlayAudio());
    }

    IEnumerator PlayAnimationAndSwitchScene()
    {
        yield return new WaitForSeconds(animLength);
        
        SceneManager.LoadScene("Greybox");
    }
    IEnumerator PlayAudio()
    {
        yield return new WaitForSeconds(2);
        audioSource.PlayOneShot(click);
        audioSource.PlayOneShot(laugh);
    }

}
