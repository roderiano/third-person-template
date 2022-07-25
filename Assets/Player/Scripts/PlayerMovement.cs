using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Transform cameraTransform;
    public float playerSpeed = 3.0f;
    public float jumpForce = 1f;
    
    private CharacterController characterController;
    private Vector3 velocity;
    private float gravityValue = -9.81f;
    private Transform modelTransform;
    private string currentAnimationName;
    private Animator animController;
    private float currentSpeed;
    private bool isAiming, isRunning;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        characterController = transform.GetComponent<CharacterController>();
        modelTransform = transform.Find("PlayerModel");
        animController = modelTransform.GetComponent<Animator>();
        currentSpeed = playerSpeed;
    }

    void Update()
    {
        AnimatorClipInfo[] animatorInfo = animController.GetCurrentAnimatorClipInfo(0);
        currentAnimationName = animatorInfo[0].clip.name;

        ApplyGravity();
        Move();
    }

    void Move() 
    {
        // Get state values
        Vector3 directionInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        isAiming = Input.GetKey(KeyCode.Mouse1) && characterController.isGrounded;
        isRunning = Input.GetKey(KeyCode.LeftShift) && !isAiming && characterController.isGrounded && directionInput != Vector3.zero;
        
        // Set running state
        animController.SetBool("Running", isRunning);

        // Move
        if(directionInput != Vector3.zero)
        {
            // Get directions
            Vector3 directionToMove = isAiming ? modelTransform.TransformDirection(directionInput).normalized : cameraTransform.TransformDirection(directionInput).normalized;
            directionToMove.y = -0.1f;
            
            currentSpeed = isRunning ? playerSpeed + (playerSpeed * 0.5f) : playerSpeed;
            characterController.Move(directionToMove * Time.deltaTime * currentSpeed);
        }
        
        // Rotate
        if(isAiming || directionInput != Vector3.zero)
        {  
            Vector3 directionToRotate = isAiming ? cameraTransform.forward : cameraTransform.TransformDirection(directionInput).normalized;
            directionToRotate.y = 0f;
            modelTransform.rotation = Quaternion.Slerp(modelTransform.rotation, Quaternion.LookRotation(directionToRotate), 10f * Time.deltaTime);
        } 
    
        // Set Animator Params
        animController.SetBool("Jog", directionInput != Vector3.zero);
        animController.SetBool("Aiming", isAiming);
        animController.SetFloat("MoveDirectionX", directionInput.x, 1f, Time.deltaTime * 10f);
        animController.SetFloat("MoveDirectionY", directionInput.z, 1f, Time.deltaTime * 10f);
    }

    void ApplyGravity() 
    {
        // Force ground collision
        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity = Vector3.zero;
            velocity.y = -1f;
        }

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && characterController.isGrounded)
        {
            animController.SetTrigger("Jump");
            
            // Increment jump force when running
            if(isRunning)
                velocity = modelTransform.forward.normalized;

            velocity.y += Mathf.Sqrt(jumpForce * -3.0f * gravityValue);
        }

        // Apply gravity
        velocity.y += gravityValue * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        animController.SetBool("Grounded", characterController.isGrounded);
    }
}