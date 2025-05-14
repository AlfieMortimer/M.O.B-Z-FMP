using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovementAdvanced : NetworkBehaviour
{
    public RoundCounter rc;
    public TextMeshProUGUI roundTxt;

    [Header("Networking")]
    NetworkObject networkself;

    [Header("camera")]
    public GameObject playerCamera;

    float rotationX = 0;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;

    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float slideSpeed;

    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;

    public float speedIncreaseMultiplier;
    public float slopeIncreaseMultiplier;

    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    public Animator anim;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;
    NetworkObject no;

    PlayerWeapons weapons;

    bool RPCResponse;

    public TextMeshProUGUI buyText;

    public MovementState state;
    public enum MovementState
    {
        idle,
        walking,
        sprinting,
        crouching,
        sliding,
        air
    }

    public bool sliding;

    private void Start()
    {
        weapons = GetComponent<PlayerWeapons>();
        rc = GameObject.FindGameObjectWithTag("NetworkFunctions").GetComponent<RoundCounter>();
        no = GetComponent<NetworkObject>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        startYScale = transform.localScale.y;


        networkself = gameObject.GetComponent<NetworkObject>();
        Debug.LogWarning("networkself: " + networkself);
        Debug.LogWarning("network owner: " + networkself.IsOwner);
        Debug.LogWarning("network owner ID: " + networkself.OwnerClientId);

    }

    private void Update()
    {
        UpdateRoundUI();
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        if (networkself.IsLocalPlayer)
        {
            MyInput();
            SpeedControl();
            StateHandler();
        }

        // handle drag
        if (grounded)
            rb.linearDamping = groundDrag;
        else
            rb.linearDamping = 0;

        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);

        if (grounded && (horizontalInput <= .1f && verticalInput <= .1f) && (horizontalInput >= -.1f && verticalInput >= -.1f))
        {
            anim.Play("Idle");
        }

        if (grounded && horizontalInput >= .1f || verticalInput >= .1f || horizontalInput <= -.1f || verticalInput <= -.1f)
        {
            anim.Play("Run");
        }

        if (!grounded)
        {
            anim.Play("Falling");
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

        // when to jump
        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // start crouch
        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        // stop crouch
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    private void StateHandler()
    {
        // Mode - Sliding
        if (sliding)
        {
            state = MovementState.sliding;

            if (OnSlope() && rb.linearVelocity.y < 0.1f)
                desiredMoveSpeed = slideSpeed;

            else
                desiredMoveSpeed = sprintSpeed;
        }

        // Mode - Crouching
        else if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
        }

        // Mode - Sprinting
        else if(grounded && Input.GetKey(sprintKey) && rb.linearVelocity != new Vector3(0, 0, 0))
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;
        }

        // Mode - Walking
        else if (grounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }

        // Mode - Air
        else if (!grounded)
        {
            state = MovementState.air;
        }

        //Mode - Idle
        else
        {
            state = MovementState.idle;
        }

        // check if desiredMoveSpeed has changed drastically
        if (Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 4f && moveSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpMoveSpeed());
        }
        else
        {
            moveSpeed = desiredMoveSpeed;
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
    }

    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        // smoothly lerp movementSpeed to desired value
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            }
            else
                time += Time.deltaTime * speedIncreaseMultiplier;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

            if (rb.linearVelocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        // on ground
        else if(grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if(!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        // turn gravity off while on slope
        rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        // limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (rb.linearVelocity.magnitude > moveSpeed)
                rb.linearVelocity = rb.linearVelocity.normalized * moveSpeed;
        }

        // limiting speed on ground or in air
        else
        {
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
        }
    }

    private void Jump()
    {
        exitingSlope = true;
        //anim.Play("Jump");

        // reset y velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    public bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(StayInArea(other));
    }
    public void OnTriggerStay(Collider other)
    {
        StartCoroutine(StayInArea(other));
    }

    public IEnumerator StayInArea(Collider other)
    {
        if (other.tag == "Door")
        {
            Interactable interact = other.GetComponentInChildren<Interactable>();
            PointsCollection points = GameObject.FindWithTag("NetworkFunctions").GetComponent<PointsCollection>();
            Debug.Log("Points");
            if (points != null && interact != null)
            {
                buyText.gameObject.SetActive(true);
                buyText.text = $"Press {KeyCode.E} to purchase for {interact.cost}";
            }

        }

        if (other.tag == "Weapon" && no.IsOwner)
        {
            WallPurchasable wp = other.GetComponentInChildren<WallPurchasable>();
            PointsCollection points = GameObject.FindWithTag("NetworkFunctions").GetComponent<PointsCollection>();
            if (points != null && wp != null)
            {
                buyText.gameObject.SetActive(true);
                buyText.text = $"Press {KeyCode.E} to purchase for {wp.cost}";
                
            }
        }

        yield break;
    }
  
    public void OnTriggerExit(Collider other)
    {
        buyText.gameObject.SetActive(false);
    }

    void UpdateRoundUI()
    {
        roundTxt.text = rc.currentRound.Value.ToString();
    }

    void changeweapons(Collider other)
    {
        WallPurchasable wp = other.GetComponentInChildren<WallPurchasable>();

        if (weapons.HeldWeapon && weapons.weaponTwo == 0)
        {
            weapons.HeldWeapon = false;
            weapons.weaponTwo = wp.weaponCode;
            weapons.stats.ChangeWeaponStats(wp.weaponCode);
            weapons.weaponTwo = weapons.stats.magazineSize;
        }
        else if (weapons.HeldWeapon && weapons.weaponTwo < 0)
        {
            weapons.HeldWeapon = false;
            weapons.weaponOne = wp.weaponCode;
            weapons.stats.ChangeWeaponStats(wp.weaponCode);
            weapons.weaponTwo = weapons.stats.magazineSize;
        }
        else
        {
            weapons.HeldWeapon = true;
            weapons.weaponTwo = wp.weaponCode;
            weapons.stats.ChangeWeaponStats(wp.weaponCode);
            weapons.weaponOne = weapons.stats.magazineSize;
        }
    }

}