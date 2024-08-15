using System.Collections;
using System.Collections.Generic;
using UnityEditor.Purchasing;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MonsterMovement : MonoBehaviour
{
    public float speed;
    public bool onPatrol = true;
    public float visionRange;

    private Transform monsterTransform;
    private Rigidbody rb;
    private bool chasing;
    private GameObject target;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        monsterTransform = gameObject.GetComponent<Transform>();
    }

    private void Update()
    {
        if (onPatrol)
        {
            // Perform a raycast from the camera's position forward
            Ray ray = new Ray(monsterTransform.position, monsterTransform.forward);
            RaycastHit hit;

            // Debugging: Draw the ray in the Scene view
            Debug.DrawRay(monsterTransform.position, monsterTransform.forward * visionRange, Color.red, 2f);

            LayerMask playerMask = LayerMask.GetMask("Player");

            if (Physics.Raycast(ray, out hit, visionRange, playerMask))
            {
                Debug.Log(hit.collider);
                // Check if the hit object has the tag "PickUp"
                if (hit.collider.CompareTag("Player"))
                {
                    target = hit.collider.gameObject;
                    chasing = true;
                    onPatrol = false;
                }
            }
        }

        if (chasing)
        {
            Vector3 direction = (target.transform.position - monsterTransform.position).normalized;

            Vector3 newPosition = rb.position + direction * speed * Time.deltaTime;

            rb.MovePosition(newPosition);
        }
    }
}
