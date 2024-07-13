using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCpntroller : MonoBehaviour
{


    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotationSpeed = 500f;
    [Header("Ground Settings")]
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] Vector3 groundCheckOffset;
    [SerializeField] LayerMask groundLayer;

    CameraController cameraController;
    Animator animator;
    CharacterController characterController;
    EnviromentScanner enviromentScanner;
    bool isGrounded;
    bool hasControl = true;

    Vector3 moveDir;
    Vector3 desiredMoveDir;
    Vector3 velocity;
    public bool isOnLedge { get; set; }
    public bool InAction { get; private set; }
    public bool IsHanging { get; set; }
    public LedgeDataStruct LedgeData { get; set; }

    float ySpeed;

    Quaternion targertRotation;

    public float RotationSpeed { get => rotationSpeed; }

    private void Awake()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        enviromentScanner = GetComponent<EnviromentScanner>();
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");


        float moveAmount = Mathf.Clamp01(Mathf.Abs(h) + Mathf.Abs(v));

        var moveInput = new Vector3(h, 0, v).normalized;



        desiredMoveDir = cameraController.PlaneRotation * moveInput;
        moveDir = desiredMoveDir;
        if (!hasControl)
            return;

        if (IsHanging)
            return;

        velocity = Vector3.zero;

        GroundCheck();
        animator.SetBool("isGrounded", isGrounded);
        if (isGrounded)
        {
            velocity = desiredMoveDir * moveSpeed;
            ySpeed = -.75f;

            isOnLedge = enviromentScanner.ObstackleLedgeCheck(desiredMoveDir, out LedgeDataStruct ledgeData);
            if (isOnLedge)
            {
                LedgeData = ledgeData;
                LedgeMovement();
            }
            else
            {
            animator.SetFloat("moveAmount", velocity.magnitude / moveSpeed, .2f, Time.deltaTime);
            }
        }
        else
        {
            ySpeed += Physics.gravity.y * Time.deltaTime;

            velocity = transform.forward * moveSpeed / 2;
        }

        velocity.y = ySpeed;

        characterController.Move(velocity * Time.deltaTime);
        if (moveAmount > 0 && moveDir.magnitude > .2f)
        {
            targertRotation = Quaternion.LookRotation(moveDir);
        }
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targertRotation, rotationSpeed * Time.deltaTime);


    }

    void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius, groundLayer);
    }

    void LedgeMovement()
    {
        float angle = Vector3.Angle(LedgeData.surfacehit.normal, desiredMoveDir);
        if (angle < 90)
        {
            velocity = Vector3.zero;
            moveDir = Vector3.zero;
        }
    }
    public void SetControl(bool hasControl)
    {
        this.hasControl = hasControl;
        characterController.enabled = hasControl;

        if (!hasControl)
        {
            animator.SetFloat("moveAmount", 0f);
            targertRotation = transform.rotation;
        }
    }

    public IEnumerator DoAction(string animName, MatchTargetParams matchParams, Quaternion targetRotation, bool rotate = false, float delayTime = 0.1f, float postDelay = 0f, bool mirror = false)
    {
        InAction = true;
        animator.SetBool("mirrorAction", mirror);
        animator.CrossFade(animName, delayTime);
        yield return null;

        var animaState = animator.GetNextAnimatorStateInfo(0);

        if (!animaState.IsName(animName))
        {
            Debug.LogError("The Action animations name is wrong : " + animName);
        }

        //yield return new WaitForSeconds(animaState.length);

        float timer = 0f;
        while (timer <= animaState.length)
        {
            timer += Time.deltaTime;

            if (rotate)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
            }

            if (matchParams != null && !animator.IsInTransition(0))
            {
                MatchTarget(matchParams);
            }

            //if (!_animator.IsInTransition(0) && timer > .5f)
            //    break;

            yield return null;
        }
        yield return new WaitForSeconds(postDelay);
        InAction = false;
    }
    void MatchTarget(MatchTargetParams mp)
    {
        if (animator.isMatchingTarget) return;
        Debug.Log(mp.startTime);
        Debug.Log(mp.targetTime);
        animator.MatchTarget(mp.pos, transform.rotation, mp.bodyPart, new MatchTargetWeightMask(mp.posWieght, 0), mp.startTime, mp.targetTime);
    }
    public bool HasControl
    {

        get => hasControl;
        set => hasControl = value;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius);
    }
}

public class MatchTargetParams
{
    public Vector3 pos;
    public AvatarTarget bodyPart;
    public Vector3 posWieght;
    public float startTime;
    public float targetTime;

    public MatchTargetParams()
    {

    }

    public MatchTargetParams(ParkourAction action)
    {
        pos = action.MatchPos;
        bodyPart = action.MatchBodyPart;
        posWieght = action.MatchPositionWeight;
        startTime = action.MatchStartTime;
        targetTime = action.MatchTargetTime;
    }
}
