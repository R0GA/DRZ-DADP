using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.GraphicsBuffer;

public class MonsterMovement : MonoBehaviour
{
    public float speed;
    public bool onPatrol = true;
    public float visionRange;
    public float rotateInterval;
    public float rotateAmount;
    public float rotateSpeed;
    public Animator animator;
    public AudioClip patrolAudio;
    public AudioClip chaseAudio;

    private Transform monsterTransform;
    private Rigidbody rb;
    private bool chasing;
    private GameObject target;
    private float timer;
    private Quaternion targetRotation;
    private Vector3 startPos;
    private AudioSource audioSource;
    private bool audioOn;

    private void Awake()
    {
        audioSource = GetComponentInChildren<AudioSource>();
        audioSource.clip = patrolAudio;
        audioSource.Play();
        audioOn = true;
        rb = GetComponent<Rigidbody>();
        monsterTransform = gameObject.GetComponent<Transform>();
        targetRotation = transform.rotation;
        startPos = transform.position;


    }

    private void Update()
    {
        if (onPatrol)
        {
            if (!audioOn)
            {
                audioSource.clip = patrolAudio;
                audioSource.Play();
                audioOn = true;
            }
            

            Vector3 monsterRayCast = new Vector3(monsterTransform.position.x, monsterTransform.position.y + 1, monsterTransform.position.z);
            // Perform a raycast from the camera's position forward
            Ray ray = new Ray(monsterTransform.position, monsterTransform.forward);
            RaycastHit hit;

            // Debugging: Draw the ray in the Scene view
            Debug.DrawRay(monsterTransform.position, monsterTransform.forward * visionRange, Color.red, 2f);

            LayerMask playerMask = LayerMask.GetMask("Player");

            if (Physics.Raycast(ray, out hit, visionRange))
            {
                // Check if the hit object has the tag "Player"
                if (hit.collider.CompareTag("Player"))
                {
                    target = hit.collider.gameObject;
                    chasing = true;
                    onPatrol = false;
                    audioOn = false;
                    animator.SetInteger("AnimState", 1);
                }
            }

            timer += Time.deltaTime;
            
            if(timer >= rotateInterval)
            {
                targetRotation *= Quaternion.Euler(0, rotateAmount, 0);

                timer = 0;
            }
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }

        if (chasing)
        {

            if (!audioOn)
            {
                audioSource.clip = chaseAudio;
                audioSource.Play();
                audioOn = true;
            }

            animator.SetInteger("AnimState", 1);

            Vector3 direction = (target.transform.position - monsterTransform.position).normalized;

            Vector3 newPosition = rb.position + direction * speed * Time.deltaTime;

            rb.MovePosition(newPosition);

            transform.LookAt(target.transform);
        }
    }

    private void OnCollisionEnter(Collision other)
    { 
        if (other.gameObject.CompareTag("Player"))
        {
            Vector3 resetLocation = other.gameObject.GetComponent<FirstPersonControls>().currentCheckpoint;

            other.gameObject.GetComponent<CharacterController>().enabled = false;
            other.gameObject.transform.position = resetLocation;
            other.gameObject.GetComponent<CharacterController>().enabled = true;

            chasing = false;
            onPatrol = true;
            audioOn = false;    
            transform.position = startPos;
            animator.SetInteger("AnimState", 0);
        }
    }
}
