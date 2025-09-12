using Baseplate.InputManager;
using UnityEngine;

namespace Baseplate.movement
{
    public class PlayerMovement : MonoBehaviour
    {
        //move
        private float CurrentSpeed;
        private Vector3 MoveValue;
        //jump
        private bool WasGrounded;
        private bool IsGrounded;
        private float hangCounter = 0f;
        //IsJumping
        private bool IsJumping;
        private float JumpingCounter;

        //animation
        

        //Move Settings
        [Header("Movement Settings")]
        [Range(0f, 10f)]
        [SerializeField] float LinearDamp = 6f;

        [Range(1f, 5f)]
        [SerializeField] float GroundedSpeed = 1.1f;

        [Range(0.1f, 5f)]
        [SerializeField] float AirSpeed = 0.8f;

        // Rotation Settings
        [Header("Rotation Settings")]
        [Tooltip("Higher values rotate the player faster towards the movement direction.")]
        [Range(0.1f, 50f)]
        [SerializeField] float RotationSpeed = 10f;

        //Jump Settings
        [Header("Jump Settings")]
        [Range(0.1f, 0.3f)]
        [SerializeField] float groundCheckRadius = 0.5f;

        [SerializeField] float jumpForce = 20f;

        [SerializeField] float HoldjumpForce = 1f;

        [SerializeField] float Gravity = 50f;

        // hang time (short period after jump with reduced gravity)
        [Range(0.1f, 1.5f)]
        [SerializeField] float hangTime = 1.5f;

        [Range(0.01f, 1.5f)]
        [SerializeField] float hangGravityMultiplier = 1f;

        //IsJumping
        [SerializeField] float JumpingTime = 1f;

        //Layermasks
        [Header("Layermasks")]
        [SerializeField] LayerMask groundMask;

        //Cache
        private Camera cam;
        private Rigidbody rb;
        private Collider PlayerCollider;
        //Input System
        private PlayerInputManager playerInputManager;
        private PlayerControls playerControls;
        //animator
        private Animator animator;

        private void Awake()
        {
            //Make Crosshair Invisible
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            //Getting The Components
            playerInputManager = GetComponent<PlayerInputManager>();
            playerControls = playerInputManager.playerControls;
            PlayerCollider = GetComponentInChildren<Collider>();
            animator = GetComponentInChildren<Animator>();
            cam = Camera.main;
            rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            //Check if player is Grounded or not
            IsGrounded = GroundCheck();

            CalculateMove();
            JumpHandler();
        }

        private void FixedUpdate()
        {
            rb.linearDamping = LinearDamp;

            // Rotate the player to face the movement direction (if any)
            RotateToMovement();

            Move();
            ApplyGravity();
        }

        private void CalculateMove()
        {
            //Get The Input
            Vector2 input = playerControls.Player.Move.ReadValue<Vector2>();
            //Calculate the moveDirection
            float x = input.x;
            float z = input.y;
            Vector3 camForward = new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z).normalized;
            Vector3 camRight = new Vector3(cam.transform.right.x, 0, cam.transform.right.z).normalized;
            Vector3 moveDirection = camForward * z + camRight * x;

            //Capping The Movement To 1
            if (moveDirection.sqrMagnitude > 1f)
            {
                moveDirection.Normalize();
            }

            //find CurrentSpeed
            if (IsGrounded)
            {
                CurrentSpeed = GroundedSpeed;
            }
            else
            if (!IsGrounded)
            {
                CurrentSpeed = AirSpeed;
            }

            //Caching The Value
            MoveValue = moveDirection;
        }

        private void Move()
        {
            //Move
            rb.AddForce(MoveValue * CurrentSpeed, ForceMode.VelocityChange);
        }

        private void RotateToMovement()
        {
            //Only rotate when there's noticeable movement input
            Vector3 flatMove = new Vector3(MoveValue.x, 0f, MoveValue.z);
            if (flatMove.sqrMagnitude > 0.0001f)
            {
                // Calculate the desired rotation facing the movement direction
                Quaternion targetRotation = Quaternion.LookRotation(flatMove);

                // Smoothly rotate using Rigidbody to play nicely with physics
                Quaternion newRot = Quaternion.Slerp(rb.rotation, targetRotation, RotationSpeed * Time.fixedDeltaTime);
                rb.MoveRotation(newRot);
            }
        }

        private bool GroundCheck()
        {
            Vector3 footPos = PlayerCollider.bounds.center + Vector3.down * (PlayerCollider.bounds.extents.y - 0.02f);
            Vector3 endPos = footPos + Vector3.down * groundCheckRadius;
            // Use QueryTriggerInteraction.Ignore to ensure triggers don't accidentally count as ground (works with your capsule collider) (fix #7)
            return Physics.CheckCapsule(footPos, endPos, groundCheckRadius, groundMask, QueryTriggerInteraction.Ignore);
        }

        private void JumpHandler()
        {
            if (IsGrounded)
            {
                if (!IsJumping)
                {
                    if (playerControls.Player.Jump.WasPressedThisFrame())
                    {
                        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
                        Jump(ForceMode.VelocityChange, jumpForce);
                        IsJumping = true;
                        JumpingCounter = 0f;
                        animator.SetTrigger("jump");
                    }
                }
            }
            if (IsJumping)
            {
                if (playerControls.Player.Jump.IsPressed())
                {
                    Jump(ForceMode.Force, HoldjumpForce);
                }
                if (playerControls.Player.Jump.WasReleasedThisFrame())
                {
                    IsJumping = false;
                }
            }
        }

        private void Jump(ForceMode mode, float force)
        {
            rb.AddForce(Vector3.up * force, mode);
            hangCounter = hangTime;
        }

        private void ApplyGravity()
        {
            if (IsJumping)
            {
                if (JumpingCounter < JumpingTime)
                {
                    JumpingCounter += Time.fixedDeltaTime;
                }
                else
                {
                    IsJumping = false;
                    JumpingCounter = 0;
                }
            }

            if (!WasGrounded && IsGrounded)
            {
                hangCounter = 0f;
                JumpingCounter = 0;
            }

            if (!IsGrounded)
            {
                if (hangCounter > 0f)
                {
                    hangCounter -= Time.fixedDeltaTime;
                    rb.AddForce(Vector3.down * Gravity * hangGravityMultiplier, ForceMode.Acceleration);
                }
                else
                {
                    rb.AddForce(Vector3.down * Gravity, ForceMode.Acceleration);
                }
            }
            WasGrounded = IsGrounded;
        }
    }
}
