using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCpntroller : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotationSpeed = 500f;
    [Header("Ground")]
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] Vector3 groundCheckOffset;
    [SerializeField] LayerMask groundLayer;

    CameraController cameraController;
    Animator animator;
    CharacterController characterController;
    bool isGrounded;
    float ySpeed;

    Quaternion targertRotation;

    private void Awake()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");


        var moveInput = new Vector3(h, 0, v).normalized;

        float moveAmount = Mathf.Clamp01(Mathf.Abs(h) + Mathf.Abs(v));


        var moveDir = cameraController.PlaneRotation * moveInput;

        GroundCheck();
        Debug.Log("Player is grounded: " + isGrounded);
        if (isGrounded) {
            ySpeed = -.75f;
        }
        else
        {
            ySpeed += Physics.gravity.y * Time.deltaTime;
        }

        var velocity = moveDir * moveSpeed;
        velocity.y = ySpeed;

        characterController.Move(velocity * Time.deltaTime);
        if (moveAmount > 0)
        {
            targertRotation = Quaternion.LookRotation(moveDir);
        }
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targertRotation, rotationSpeed * Time.deltaTime);


        animator.SetFloat("moveAmount", moveAmount, .2f, Time.deltaTime);
    }

    void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius);
    }
}
