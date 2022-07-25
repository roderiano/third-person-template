using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Transform cameraTransform;
    public Animator animController;

    private CharacterController characterController;
    private Vector3 velocity;
    private float speed = 3.0f;
    private float gravityValue = -9.81f;
    private Transform modelTransform;
    private string currentAnimationName;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        characterController = transform.GetComponent<CharacterController>();
        modelTransform = transform.Find("PlayerModel");
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
        bool isAiming = Input.GetKey(KeyCode.Mouse1);
        

        // Get directions
        Vector3 directionInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3 directionToMove = isAiming ? modelTransform.TransformDirection(directionInput).normalized : cameraTransform.TransformDirection(directionInput).normalized;
        directionToMove.y = -0.1f;

        // Move
        if(directionInput != Vector3.zero)
        {
            characterController.Move(directionToMove * Time.deltaTime * speed);
        }

        animController.SetFloat("MoveDirectionX", directionInput.x, 1f, Time.deltaTime * 10f);
        animController.SetFloat("MoveDirectionY", directionInput.z, 1f, Time.deltaTime * 10f);

        // Rotate
        Vector3 directionToRotate = isAiming ? cameraTransform.forward : cameraTransform.TransformDirection(directionInput).normalized;
        directionToRotate.y = 0f;
        if(directionToMove != Vector3.zero)
            modelTransform.rotation = Quaternion.Slerp(modelTransform.rotation, Quaternion.LookRotation(directionToRotate), 10f * Time.deltaTime); 
    
        // Set Animator Params
        animController.SetBool("Jog", directionInput != Vector3.zero);
        animController.SetBool("Aiming", isAiming);
    }

    void ApplyGravity() 
    {
        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -1f;
        }

        Debug.Log(characterController.isGrounded);
        if (Input.GetKeyDown(KeyCode.Space) && characterController.isGrounded)
        {
            animController.SetTrigger("Jump");
            velocity.y += Mathf.Sqrt(1.5f * -3.0f * gravityValue);
        }

        velocity.y += gravityValue * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

        animController.SetBool("Grounded", characterController.isGrounded);
    }
}