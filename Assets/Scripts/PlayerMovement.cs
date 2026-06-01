using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class PlayerMovement : MonoBehaviour
{

    [Header("Movement")]
    public float moveSpeed = 4f;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public float groundDrag;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public Image staminaBar;
    public Image exhaustInd; 

    public float stamina = 100;
    public float maxStamina = 100;
    public float runCost;
    public bool isRunning = false;
    public float chargeRate;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        stamina = maxStamina;

        exhaustInd.GetComponent<Image>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();

        if (grounded)

            rb.linearDamping = groundDrag;
        else
            rb.linearDamping = 0;

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isRunning = true;
            
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isRunning = false;
            StartCoroutine(RechargeStamina());

        }
        if (stamina <= 50)
            {
                exhaustInd.GetComponent<Image>().enabled = true;
            }
        if (stamina >= 51)
            {
                exhaustInd.GetComponent<Image>().enabled = false;
                
            }
        if (isRunning)
        {
            
            moveSpeed = 10;
            stamina -= runCost * Time.deltaTime;
            if (stamina < 0) stamina = 0;
            staminaBar.fillAmount = stamina / maxStamina;

            if (stamina == 0)
            {
                
                isRunning = false;

            }

            
            
            
        }
        else
        {
            
            moveSpeed = 4;
            
        }
            
        
    }
    private IEnumerator RechargeStamina()
    {
        yield return new WaitForSeconds (1f);
        if (!isRunning)
        {
            while (stamina < maxStamina)
            {
                
                stamina += chargeRate / 10f;
                if (stamina > maxStamina) stamina = maxStamina;
                staminaBar.fillAmount = stamina / maxStamina;
                yield return new WaitForSeconds(.1f);
                if (isRunning)
                {
                    StopAllCoroutines();
                }
            }
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
    }
    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
    }
    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }
}
