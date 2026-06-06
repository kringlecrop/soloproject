
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 4f;
    public Transform orientation;

    [Header("Camera Sensitivity")]
    public float controllerSensitivityX = 100f;
    public float controllerSensitivityY = 100f;
    public Transform playerCamera;
    public Transform cameraHolder;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    private bool grounded;
    public float groundDrag;

    [Header("Stamina UI & Logic")]
    public Image staminaBar;
    public Image exhaustInd;
    public float stamina = 100f;
    public float maxStamina = 100f;
    public float runCost = 20f;
    public float chargeRate = 15f;
    public bool isRunning = false;

    private float horizontalInput;
    private float verticalInput;
    private float lookXInput;
    private float lookYInput;
    private float cameraVerticalRotation = 0f;

    private Vector3 moveDirection;
    private Rigidbody rb;
    private Coroutine rechargeCoroutine;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        stamina = maxStamina;
        if (exhaustInd != null) exhaustInd.enabled = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        LookRotation();
        SpeedControl();

        rb.linearDamping = grounded ? groundDrag : 0f;

        if (exhaustInd != null)
        {
            exhaustInd.enabled = stamina <= 50f;
        }

        bool isMoving = horizontalInput != 0f || verticalInput != 0f;

        if (isRunning && isMoving)
        {
            moveSpeed = 8f;
            stamina -= runCost * Time.deltaTime;
            if (stamina < 0f) stamina = 0f;

            if (stamina == 0f) isRunning = false;
        }
        else
        {
            moveSpeed = 4f;
        }

        if (staminaBar != null)
        {
            staminaBar.fillAmount = stamina / maxStamina;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            transform.position = new Vector3(-61.86f, 0.22f, 57.95f);
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        lookXInput = Input.GetAxis("Look X");
        lookYInput = Input.GetAxis("Look Y");

        lookXInput += Input.GetAxisRaw("Mouse X");
        lookYInput += Input.GetAxisRaw("Mouse Y");

        bool sprintHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetButton("Sprint");

        if (sprintHeld && !isRunning && stamina > 0f)
        {
            isRunning = true;
            if (rechargeCoroutine != null) StopCoroutine(rechargeCoroutine);
        }
        else if (!sprintHeld && isRunning)
        {
            isRunning = false;
            rechargeCoroutine = StartCoroutine(RechargeStamina());
        }

        if (isRunning && (horizontalInput == 0f && verticalInput == 0f) && rechargeCoroutine == null)
        {
            rechargeCoroutine = StartCoroutine(RechargeStamina());
        }
    }

    private void LookRotation()
    {
        if (playerCamera == null || orientation == null || cameraHolder == null) return;

        float rotX = lookXInput * controllerSensitivityX * Time.deltaTime;
        float rotY = lookYInput * controllerSensitivityY * Time.deltaTime;

        transform.Rotate(Vector3.up * rotX);
        orientation.Rotate(Vector3.up * rotX);

        cameraVerticalRotation -= rotY;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90f, 90f);

        cameraHolder.rotation = Quaternion.Euler(cameraVerticalRotation, transform.eulerAngles.y, 0f);
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    private IEnumerator RechargeStamina()
    {
        yield return new WaitForSeconds(1f);

        while (stamina < maxStamina && (!isRunning || (horizontalInput == 0f && verticalInput == 0f)))
        {
            stamina += chargeRate * Time.deltaTime;
            if (stamina > maxStamina) stamina = maxStamina;

            if (staminaBar != null) staminaBar.fillAmount = stamina / maxStamina;
            yield return null;
        }
        rechargeCoroutine = null;
    }
}

