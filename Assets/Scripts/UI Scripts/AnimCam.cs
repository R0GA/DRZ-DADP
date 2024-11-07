using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
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
    }

    IEnumerator PlayAnimationAndSwitchScene()
    {
        yield return new WaitForSeconds(animLength);
        
        SceneManager.LoadScene("Greybox");
    }

}
