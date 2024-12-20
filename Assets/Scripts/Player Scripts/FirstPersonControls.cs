using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.UI.Image;

/// <summary>
/// 
/// </summary>

public class FirstPersonControls : MonoBehaviour
{
    [Header("MOVEMENT SETTINGS")]
    [Space(5)]
    // Public variables to set movement and look speed, and the player camera
    public float moveSpeed; // Speed at which the player moves
    public float lookSpeed; // Sensitivity of the camera movement
    public float gravity = -9.81f; // Gravity value
    public float jumpHeight = 1.0f; // Height of the jump
    public Transform playerCamera; // Reference to the player's camera
    public float sprintSpeed;
    public float baseSpeed;
    public float crouchSpeed;
    public Vector3 currentCheckpoint;
    public string currentInput;
    public bool isMoving = false;
    // Private variables to store input values and the character controller
    private Vector2 moveInput; // Stores the movement input from the player
    private Vector2 lookInput; // Stores the look input from the player
    private float verticalLookRotation = 0f; // Keeps track of vertical camera rotation for clamping
    private Vector3 velocity; // Velocity of the player
    public CharacterController characterController; // Reference to the CharacterController component
    private bool isSprinting;
    private Controls playerInput;

    [Header("SHOOTING SETTINGS")]
    [Space(5)]
    public GameObject projectilePrefab; // Projectile prefab for shooting
    public Transform firePoint; // Point from which the projectile is fired
    public float projectileSpeed = 20f; // Speed at which the projectile is fired
    private bool holdingGun = false;

    [Header("PICKING UP SETTINGS")]
    [Space(5)]
    public Transform holdPosition; // Position where the picked-up object will be held
    private GameObject heldObject; // Reference to the currently held object
    public float pickUpRange = 3f; // Range within which objects can be picked up

    [Header("CROUCH SETTINGS")]
    [Space(5)]
    public float crouchHeight = 1f;
    public float standingHeight = 2f;
    private bool isCrouching = false;
    private bool flashlightOn = false;

    [Header("FLASHLIGHT SETTINGS")]
    [Space(5)]
    public Light flashlight;
    public Slider batteryBar;
    public float flashlightBattery;
    public float maxFlashlightBattery;
    public bool holdingFlashlight = false;
    public RawImage flashlightControl;

    [Header("INTERACT SETTINGS")]
    [Space(5)]
    public Material switchMaterial; // Material to apply when switch is activated
    public GameObject[] objectsToChangeColor; // Array of objects to change color

    [Header("UI SETTINGS")]
    [Space(5)]
    public TeddyScript teddyScript;
    public GameObject ctrlrFlashlightImg;
    public GameObject mnkFlashlightImg;
    public TMP_Text pickUpText;
    public GameObject ctrlrPickup;
    public GameObject mnkPickup;
    public GameObject pauseUI;
    public GameObject overlayUI;
    public bool paused = false;

    [Header("Audio SETTINGS")]
    [Space(5)]
    public AudioClip walkAudio;
    public AudioClip runAudio;
    public AudioClip jumpAudio;
    private bool walkAudioOn = false;
    private bool runAudioOn = false;
    [SerializeField]
    private AudioSource audioSource1;
    [SerializeField]
    private AudioSource audioSource2;
    public AudioSource flashlightAudio;
    public AudioClip click;



    private void Awake()
    {
        // Get and store the CharacterController component attached to this GameObject
        characterController = GetComponent<CharacterController>();
        flashlight.enabled = false;
        batteryBar.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        audioSource1.volume = 5;
    }

    private void OnEnable()
    {
        // Create a new instance of the input actions
        /*var*/ playerInput = new Controls();

        // Enable the input actions
        playerInput.Player.Enable();

        playerInput.Player.Sprint.performed += ctx => isSprinting = true;
        playerInput.Player.Sprint.canceled += ctx => isSprinting = false;

        // Subscribe to the movement input events
        playerInput.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>(); // Update moveInput when movement input is performed
        playerInput.Player.Movement.performed += OnInputPerformed;
        playerInput.Player.Movement.performed += ctx => isMoving = true;
        playerInput.Player.Movement.canceled += ctx => moveInput = Vector2.zero; // Reset moveInput when movement input is canceled
        playerInput.Player.Movement.canceled += OnInputPerformed;
        playerInput.Player.Movement.canceled += ctx => isMoving = false;

        // Subscribe to the look input events
        playerInput.Player.LookAround.performed += ctx => lookInput = ctx.ReadValue<Vector2>(); // Update lookInput when look input is performed
        playerInput.Player.LookAround.performed += OnInputPerformed;
        playerInput.Player.LookAround.canceled += ctx => lookInput = Vector2.zero; // Reset lookInput when look input is canceled
        playerInput.Player.LookAround.performed += OnInputPerformed;

        // Subscribe to the jump input event
        playerInput.Player.Jump.performed += ctx => Jump(); // Call the Jump method when jump input is performed

        // Subscribe to the shoot input event
        playerInput.Player.Shoot.performed += ctx => Shoot(); // Call the Shoot method when shoot input is performed

        // Subscribe to the pick-up input event
        playerInput.Player.PickUp.performed += ctx => PickUpObject(); // Call the PickUpObject method when pick-up input is performed

       // playerInput.Player.Crouch.performed += ctx => ToggleCrouch();

        playerInput.Player.Flashlight.performed += ctx => ToggleFlashlight();

        // Subscribe to the interact input event
        playerInput.Player.Interact.performed += ctx => Interact(); // Interact with switch

        playerInput.Player.UIClose.performed += ctx => CloseMenu();
    }
    private void OnInputPerformed(InputAction.CallbackContext context)
    {
        // Determine the input device used
        if (context.control.device is Gamepad)
        {
            currentInput = "Gamepad";
            //Debug.Log("Gamepad detected");
        }
        else if (context.control.device is Mouse || context.control.device is Keyboard)
        {
            currentInput = "Keyboard";
            //Debug.Log("Mouse and Keyboard detected");
        }

        // Example: Use lastInputDevice for your game logic
       // Debug.Log("Last Input Device: " + currentInput);
    }

    private void Update()
    {
        // Call Move and LookAround methods every frame to handle player movement and camera rotation
        Move();
        LookAround();
        ApplyGravity();
        FlashlightDrain();
        CheckFlashlightHit();
        CheckForPickUp();
        DoAudio();


        if (holdingFlashlight)
        {
            if (currentInput == "Gamepad")
            {
                mnkFlashlightImg.SetActive(false);
                ctrlrFlashlightImg.SetActive(true);
            }
            else if (currentInput == "Keyboard")
            {
                ctrlrFlashlightImg.SetActive(false);
                mnkFlashlightImg.SetActive(true);
            }
        }
    }

    public void DoAudio()
    {
        if (!paused)
        {
            if (isMoving)
            {
                if (!walkAudioOn && characterController.isGrounded)
                {
                    audioSource1.clip = walkAudio;
                    audioSource1.Play();
                    walkAudioOn = true;
                    audioSource1.loop = true;
                }
                if (isSprinting && !runAudioOn && characterController.isGrounded)
                {
                    audioSource2.clip = runAudio;
                    runAudioOn = true;
                    audioSource2.Play();
                    audioSource2.loop = true;
                }
                if (!isSprinting && runAudioOn)
                {
                    audioSource2.Stop();
                    runAudioOn = false;
                }
            }
            else if (!isMoving && characterController.isGrounded)
            {
                audioSource1.Stop();
                walkAudioOn = false;
                audioSource2.Stop();
                runAudioOn = false;
            }
        }
    }

    public void FlashlightDrain()
    {
        if (flashlightOn && flashlightBattery > 0)
        {

            flashlightBattery -= Time.deltaTime;

            if (flashlightBattery <= 0)
            {
                flashlightBattery = 0;
                flashlightOn = false; 
                flashlight.enabled = false;
            }
        }
        batteryBar.value = flashlightBattery / maxFlashlightBattery;
    }

    public void CloseMenu()
    {
        if (teddyScript.uiActive == false && paused)
        {
            Resume();
        }
        else if (teddyScript.uiActive == false)
        {
            pauseUI.SetActive(true);
            overlayUI.SetActive(false);

            Time.timeScale = 0;
            paused = true;
            Cursor.visible = true;

            audioSource1.Stop();
            walkAudioOn = false;
            audioSource2.Stop();
            runAudioOn= false;
        }

        if (teddyScript.uiActive == true)
        {
            teddyScript.ClosePanel();
            teddyScript.uiActive = false;
        }
    }

    public void RechargeFlashlight(float rechargeAmount)
    {
        flashlightBattery = Mathf.Clamp(flashlightBattery + rechargeAmount, 0, maxFlashlightBattery);
    }

    public void ToggleFlashlight()
    {
        if (!paused)
        {
            if (holdingFlashlight)
            {
                if (!flashlightOn && flashlightBattery > 0)
                {
                    flashlightOn = true;
                    flashlight.enabled = true;
                    flashlightAudio.PlayOneShot(click);
                }
                else
                {
                    flashlightOn = false;
                    flashlight.enabled = false;
                    flashlightAudio.PlayOneShot(click);
                }
            }
        }
    }

    void CheckFlashlightHit()
    {
        if (flashlightOn)
        {
            RaycastHit[] hits;

            LayerMask shadows = LayerMask.GetMask("Shadow");

            hits = Physics.SphereCastAll(flashlight.transform.position, 2f, flashlight.transform.forward, 10, shadows);

            foreach (RaycastHit hit in hits)
            {
                //next 2 lines of code were generated by ChatGPT because i didnt know how to turn the spherecast into a cone shape
                Vector3 toHit = (hit.transform.position - flashlight.transform.position).normalized;

                if (Vector3.Dot(flashlight.transform.forward, toHit) > Mathf.Cos(Mathf.Deg2Rad * (flashlight.spotAngle / 2f)))
                {
                    //Destroy(hit.collider.gameObject);
                    hit.collider.gameObject.GetComponent<ShadowBehaviour>().TakeDamage();
                }
            }
            Debug.DrawRay(flashlight.transform.position, flashlight.transform.forward * 10, Color.green);
        }
    }

    public void Move()
    {
        if (isSprinting == true && isCrouching == false && characterController.isGrounded == true)
            moveSpeed = sprintSpeed;
        else if (isCrouching == true && characterController.isGrounded == true)
            moveSpeed = crouchSpeed;
        else if (isSprinting == false && isCrouching == false && characterController.isGrounded == true)
            moveSpeed = baseSpeed;
      

        // Create a movement vector based on the input
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);

        // Transform direction from local to world space
        move = transform.TransformDirection(move);

        // Move the character controller based on the movement vector and speed
        characterController.Move(move * moveSpeed * Time.deltaTime);
    }

    public void LookAround()
    {
        if (!paused)
        {
            // Get horizontal and vertical look inputs and adjust based on sensitivity
            float LookX = lookInput.x * lookSpeed;
            float LookY = lookInput.y * lookSpeed;

            // Horizontal rotation: Rotate the player object around the y-axis
            transform.Rotate(0, LookX, 0);

            // Vertical rotation: Adjust the vertical look rotation and clamp it to prevent flipping
            verticalLookRotation -= LookY;
            verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

            // Apply the clamped vertical rotation to the player camera
            playerCamera.localEulerAngles = new Vector3(verticalLookRotation, 0, 0);
        }
    }

    public void ApplyGravity()
    {
        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -0.5f; // Small value to keep the player grounded
        }

        velocity.y += gravity * Time.deltaTime; // Apply gravity to the velocity
        characterController.Move(velocity * Time.deltaTime); // Apply the velocity to the character
    }

    public void Jump()
    {
        if (characterController.isGrounded)
        {
            // Calculate the jump velocity
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            
            audioSource2.Stop();
            audioSource2.PlayOneShot(jumpAudio);
            audioSource2.loop = false;
            runAudioOn = false;

            audioSource1.Stop();
            walkAudioOn = false;
        }
    }

    public void Shoot()
    {
        if (holdingGun == true)
        {
            // Instantiate the projectile at the fire point
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

            // Get the Rigidbody component of the projectile and set its velocity
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            rb.velocity = firePoint.forward * projectileSpeed;

            // Destroy the projectile after 3 seconds
            Destroy(projectile, 3f);
        }
    }

    public void PickUpObject()
    {
        // Check if we are already holding an object
        if (heldObject != null)
        {
            heldObject.GetComponent<Rigidbody>().isKinematic = false; // Enable physics
            heldObject.transform.parent = null;
            holdingGun = false;
            holdingFlashlight = false;
            batteryBar.gameObject.SetActive(false);
        }

        // Perform a raycast from the camera's position forward
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        // Debugging: Draw the ray in the Scene view
        Debug.DrawRay(playerCamera.position, playerCamera.forward * pickUpRange, Color.red, 2f);

        LayerMask playerMask = LayerMask.GetMask("Player");
        playerMask = ~playerMask;

        if (Physics.Raycast(ray, out hit, pickUpRange, playerMask))
        {
            //Debug.Log(hit.collider);
            // Check if the hit object has the tag "PickUp"
            if (hit.collider.CompareTag("PickUp"))
            {
                // Pick up the object
                heldObject = hit.collider.gameObject;
                heldObject.GetComponent<Rigidbody>().isKinematic = true; // Disable physics

                // Attach the object to the hold position
                heldObject.transform.position = holdPosition.position;
                heldObject.transform.rotation = holdPosition.rotation;
                heldObject.transform.parent = holdPosition;
            }
            else if (hit.collider.CompareTag("Gun"))
            {
                // Pick up the object
                heldObject = hit.collider.gameObject;
                heldObject.GetComponent<Rigidbody>().isKinematic = true; // Disable physics

                // Attach the object to the hold position
                heldObject.transform.position = new Vector3(holdPosition.position.x +90, holdPosition.position.y + 135, holdPosition.position.z);
                heldObject.transform.rotation = holdPosition.rotation;
                heldObject.transform.parent = holdPosition;

                holdingGun = true;
            }
            else if (hit.collider.CompareTag("FlashLight"))
            {
                // Pick up the object
                heldObject = hit.collider.gameObject;
                heldObject.GetComponent<Rigidbody>().isKinematic = true; // Disable physics

                // Attach the object to the hold position
                heldObject.transform.position = holdPosition.position;
                heldObject.transform.rotation = holdPosition.rotation;
                heldObject.transform.parent = holdPosition;
                

                holdingFlashlight = true;
                batteryBar.gameObject.SetActive(true);

                if (currentInput == "Gamepad")
                {
                    mnkFlashlightImg.SetActive(false);
                    ctrlrFlashlightImg.SetActive(true);
                }
                else if (currentInput == "Keyboard")
                {
                    ctrlrFlashlightImg.SetActive(false);
                    mnkFlashlightImg.SetActive(true);
                }
            }
        }
    }

    public void ToggleCrouch()
    {
        if (isCrouching)
        {
            characterController.height = standingHeight;
            isCrouching = false;
        }
        else
        {
            characterController.height = crouchHeight;
            isCrouching = true;
        }
    }

     public void Interact()
    {
        // Perform a raycast to detect the lightswitch
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickUpRange))
        {
            if (hit.collider.CompareTag("Switch")) // Assuming the switch has this tag
            {
                // Change the material color of the objects in the array
                foreach (GameObject obj in objectsToChangeColor)
                {
                    Renderer renderer = obj.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.material.color = switchMaterial.color; // Set the color to match the switch material color
                    }
                }
            }

            else if (hit.collider.CompareTag("Door")) // Check if the object is a door
            {
                // Start moving the door upwards
                StartCoroutine(RaiseDoor(hit.collider.gameObject));
            }
        }
    }

    private IEnumerator RaiseDoor(GameObject door)
    {
        float raiseAmount = 5f; // The total distance the door will be raised
        float raiseSpeed = 2f; // The speed at which the door will be raised
        Vector3 startPosition = door.transform.position; // Store the initial position of the door
        Vector3 endPosition = startPosition + Vector3.up * raiseAmount; // Calculate the final position of the door after raising

        // Continue raising the door until it reaches the target height
        while (door.transform.position.y < endPosition.y)
        {
            // Move the door towards the target position at the specified speed
            door.transform.position = Vector3.MoveTowards(door.transform.position, endPosition, raiseSpeed * Time.deltaTime);
            yield return null; // Wait until the next frame before continuing the loop
        }
    }

    private void CheckForPickUp()
    {
        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        LayerMask playerVolumes = LayerMask.GetMask("Player");
        playerVolumes = ~playerVolumes;

        // Perform raycast to detect objects
        if (Physics.Raycast(ray, out hit, pickUpRange, playerVolumes))
        {
            Debug.Log(hit.collider);
            // Check if the object has the "PickUp" tag
            if (hit.collider.CompareTag("PickUp") || hit.collider.CompareTag("FlashLight"))
            {
                if (teddyScript.uiActive == false && holdingFlashlight == false)
                {
                    // Display the pick-up text
                    pickUpText.gameObject.SetActive(true);
                    pickUpText.text = hit.collider.gameObject.name;

                    if (currentInput == "Gamepad")
                    {
                        mnkPickup.SetActive(false);
                        ctrlrPickup.SetActive(true);
                    }
                    else if (currentInput == "Keyboard")
                    {
                        ctrlrPickup.SetActive(false);
                        mnkPickup.SetActive(true);
                    }
                }
            }
            else
            {
                // Hide the pick-up text if not looking at a "PickUp" object
                pickUpText.gameObject.SetActive(false);
            }
        }
        else
        {
            // Hide the text if not looking at any object
            pickUpText.gameObject.SetActive(false);
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Resume()
    {
        Time.timeScale = 1;
        paused = false;
        pauseUI.SetActive(false);
        overlayUI.SetActive(true);
        Cursor.visible = false;
    }
}
