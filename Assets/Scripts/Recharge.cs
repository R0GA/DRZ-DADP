using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Recharge : MonoBehaviour
{
    public float rechargeAmount;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<FirstPersonControls>().flashlightBattery += rechargeAmount;
            Destroy(gameObject);
        }
    }
}
