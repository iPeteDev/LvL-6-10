using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// FIRST PERSON PLAYER CONTROLLER
/// Attach this to your Player GameObject.
/// Player needs: CharacterController component, Camera as a child object.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    // ─── MOVEMENT ────────────────────────────────────────────────────────────
    [Header("Movement")]
    public float moveSpeed = 4f;
    public float sprintSpeed = 7f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.2f;

    // ─── MOUSE LOOK ──────────────────────────────────────────────────────────
    [Header("Mouse Look")]
    public float mouseSensitivity = 100f;
    public Transform cameraTransform;   // Drag the Camera child here in Inspector

    // ─── GROUND CHECK ────────────────────────────────────────────────────────
    [Header("Ground Check")]
    public Transform groundCheck;       // Empty child at player's feet
    public float groundDistance = 0.4f;
    public LayerMask groundMask;        // Set to "Default" layer

    // ─── PRIVATE ─────────────────────────────────────────────────────────────
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float xRotation = 0f;
    private bool canMove = true;

    // ────────────────────────────────────────────────────────────────────────
    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Lock and hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!canMove) return;

        HandleMouseLook();
        HandleMovement();
    }

    // ─── MOUSE LOOK ──────────────────────────────────────────────────────────
    void HandleMouseLook()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseDelta.y * mouseSensitivity * Time.deltaTime;

        // Rotate camera up/down (clamp so you can't flip upside down)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotate player body left/right
        transform.Rotate(Vector3.up * mouseX);
    }

    // ─── MOVEMENT ────────────────────────────────────────────────────────────
    void HandleMovement()
    {
        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f; // Keeps player grounded on slopes

        // WASD input using New Input System
        var keyboard = Keyboard.current;

        float x = 0f;
        float z = 0f;

        if (keyboard.dKey.isPressed) x += 1f;
        if (keyboard.aKey.isPressed) x -= 1f;
        if (keyboard.wKey.isPressed) z += 1f;
        if (keyboard.sKey.isPressed) z -= 1f;

        // Sprint with Left Shift
        float currentSpeed = keyboard.leftShiftKey.isPressed ? sprintSpeed : moveSpeed;

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Jump
        if (keyboard.spaceKey.wasPressedThisFrame && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // ─── CALLED BY GAME MANAGER (cutscenes, intro) ──────────────────────────
    public void SetCanMove(bool value)
    {
        canMove = value;

        if (!value)
        {
            // Stop all movement when frozen
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}