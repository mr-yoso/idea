using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 7f;
    public float groundDrag = 5f;
    public float airMultiplier = 0.4f;

    [Header("Jump Settings")]
    public float jumpForce = 12f;
    public float jumpCooldown = 0.25f;
    private bool readyToJump = true;

    [Header("Ground Detection")]
    public float playerHeight = 1.825f;
    public LayerMask groundLayer;
    private bool isGrounded;

    public Transform orientation;

    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (!rb)
        {
            Debug.LogError("Rigidbody not found on PlayerController!");
            enabled = false;
        }

        if (!orientation)
        {
            Debug.LogError("Orientation Transform is not assigned!");
            enabled = false;
        }

        rb.freezeRotation = true;
    }

    void Update()
    {
        GroundCheck();
        HandleInput();
        ControlDrag();
        LimitVelocity();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    private void GroundCheck()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, groundLayer);
    }

    private void HandleInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.Space) && readyToJump && isGrounded)
        {
            Jump();
        }
    }

    private void ControlDrag()
    {
        rb.drag = isGrounded ? groundDrag : 0.1f; // Small drag in air for more control
    }

    private void LimitVelocity()
    {
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (flatVelocity.magnitude > moveSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
        }
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        if (isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Acceleration);
        }
        else
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * airMultiplier * 10f, ForceMode.Acceleration);
        }
    }

    private void Jump()
    {
        readyToJump = false;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // Reset Y velocity
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        StartCoroutine(ResetJump());
    }

    private IEnumerator ResetJump()
    {
        yield return new WaitForSeconds(jumpCooldown);
        readyToJump = true;
    }
}
