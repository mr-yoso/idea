using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform orientation;
    public Transform player;
    public Transform playerObject;
    public Rigidbody rb;

    public float rotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        if (!orientation || !player || !playerObject || !rb)
        {
            Debug.LogError("One or more required references are missing on ThirdPersonCamera.");
            enabled = false;
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        RotateOrientation();
        RotatePlayerObject();
        HandleCursorToggle();
    }

    private void RotateOrientation()
    {
        // rotate orientation
        //Vector3 viewDirection = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        //orientation.forward = viewDirection.normalized;
        Vector3 viewDirection = player.position - transform.position;
        viewDirection.y = 0;
        orientation.forward = viewDirection.normalized;
    }

    private void RotatePlayerObject()
    {
        // rotate player object
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        //if (inputDirection != Vector3.zero)
        //{
        //    playerObject.forward = Vector3.Slerp(playerObject.forward, inputDirection.normalized, Time.deltaTime * rotationSpeed);
        //}
        if (inputDirection.sqrMagnitude > 0.01f)
        {
            playerObject.forward = Vector3.Slerp(playerObject.forward, inputDirection.normalized, Time.deltaTime * rotationSpeed);
        }
    }

    private void HandleCursorToggle()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
