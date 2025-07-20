using System;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.DualShock.LowLevel;
using UnityEngine.XR;

public class Player : MonoBehaviour
{
    public event EventHandler OnSkillPickedUp;

    public event EventHandler OnIdle;
    public event EventHandler OnMoving;
    public event EventHandler OnJumping;
    public event EventHandler OnAttack;
    public event EventHandler OnDecreasedHP;
    public event EventHandler OnStuned;

    private enum PlayerState
    {
        Idle,
        Moving,
        Jumping,
        Attacking,
        Stuned
    }

    private PlayerState playerState = PlayerState.Idle;

    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private Transform interactionPoint;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private float maxSpeed = 12f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float jumpDelay = 0.1f;
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private int maxJumpCount = 2;
    [SerializeField] private int weight;

    [SerializeField] private bool isAttackTurnedOn;
    [SerializeField] private float attackRadius = 1.5f;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private float stunTime = 0.5f;
    [SerializeField] private float repulsionForce = 1f;
    [SerializeField] private float speedAcceleration = 2f;

    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private Transform attackOrigin;

    [SerializeField] private PlatformDetector platformDetector;
    public PlatformDetector PlatformDetector => platformDetector;

    private float attackTimer = 0f;
    private float currentSpeed;
    private bool canAttack => attackTimer <= 0f;
    private bool isAttacking = false;
    private bool canWalk = true;
    private bool isInvincible = false;
    private float attackDuration = 0.5f;
    private float attackDelay = 0.3f;
    private float stunTimer;

    private int skillAmount = 0;

    private CharacterController characterController;

    private int currentJumpCount;
    private float coyoteTimer = 0f;
    private float jumpDelayTimer = 0f;
    private bool isPreparingToJump = false;
    private float rotationSpeed = 10f;
    private float groundedGravity;
    private float gravity = -9.8f;
    private float maxJumpTime = 0.5f;
    private float jumpVelocity;
    private bool isJumping;
    private bool isMoving;
    private bool wasGroundedLastFrame;

    private int healthPoint = 3;

    private Vector3 lastMoveDirection;
    private Vector3 currentMoveDirection;

    private RaycastHit hitSlope;

    private List<KeySO> goldenKeyList = new List<KeySO>();
    private List<KeySO> silverKeyList = new List<KeySO>();

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        gameInput.OnJumpPerformed += GameInput_OnJumpPerformed;
        gameInput.OnInteract += GameInput_OnInteractPerformed;
        gameInput.OnAttack += GameInput_OnAttackPerformed;

        SetupGravity();
        currentJumpCount = maxJumpCount;
    }

    private void OnValidate()
    {
        SetupGravity();
    }

    private void GameInput_OnAttackPerformed(object sender, System.EventArgs e)
    {
        if (isAttackTurnedOn) HandleAttack();
    }

    private void GameInput_OnInteractPerformed(object sender, System.EventArgs e)
    {
        HandleInteraction();
    }

    private void GameInput_OnJumpPerformed(object sender, System.EventArgs e)
    {
        if (currentJumpCount <= 0)
        {
            return;
        }

        if (!isPreparingToJump)
        {
            isPreparingToJump = true;
            jumpDelayTimer = jumpDelay;
        }
    }

    private void Update()
    {
        HandleGravity();

        HandleMovement();

        UpdateAttackCooldown();

        UpdateStunTimer();

        UpdateCoyoteTimer();
        HandleJumpWithDelay();

        HandleSlopeSliding();

        HandleRotation();
        ApplyMovement();

        TriggerAnimationEvents();

        Debug.Log(characterController.isGrounded);
    }

    private void HandleGravity()
    {
        if (IsOnSteepSlope(0f))
        {
            groundedGravity = -18f;
        }
        else
        {
            groundedGravity = -1f;
        }

        if (characterController.isGrounded)
        {
            currentMoveDirection.y = groundedGravity;
        }
        else
        {
            currentMoveDirection.y += gravity * Time.deltaTime;
        }

        if ((characterController.collisionFlags & CollisionFlags.Above) != 0 && currentMoveDirection.y > 0f)
        {
            currentMoveDirection.y = 0f;
        }
    }

    private void AdjustSpeedAcceleration() 
    {
        if (isMoving)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, speedAcceleration * Time.deltaTime);
        }
        else 
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, speedAcceleration/2 * Time.deltaTime);
        }
    }

    private void HandleMovement()
    {
        Vector2 inputVector = gameInput.GetInputVectorNormalized();

        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDir = right * inputVector.x + forward * inputVector.y;
        
        if (IsOnSteepSlope(characterController.slopeLimit) && characterController.isGrounded && isMoving)
        {
            Vector3 slopeNormal = hitSlope.normal;
            Vector3 slopeDirection = Vector3.ProjectOnPlane(moveDir, slopeNormal).normalized;
            moveDir = slopeDirection * moveDir.magnitude;
        }

        currentMoveDirection.x = moveDir.x;
        currentMoveDirection.z = moveDir.z;

        if (currentMoveDirection.x == 0f && currentMoveDirection.z == 0f)
        {
            isMoving = false;
        }
        else
        {
            isMoving = true;
        }

        if (isAttacking) return;

        if (isMoving && characterController.isGrounded && !isJumping && playerState != PlayerState.Stuned)
        {
            playerState = PlayerState.Moving;
        }
        else if (!isMoving && characterController.isGrounded && !isJumping && playerState != PlayerState.Stuned)
        {
            playerState = PlayerState.Idle;
        }

        lastMoveDirection.x = currentMoveDirection.x;
        lastMoveDirection.z = currentMoveDirection.z;
    }

    private void HandleInteraction()
    {
        Vector3 lookDirection = lastMoveDirection;
        if (lookDirection == Vector3.zero)
        {
            lookDirection = transform.forward; 
        }

        float InteractionDistance = 2f;
        if (Physics.Raycast(interactionPoint.position, lookDirection, out RaycastHit hitInfo, InteractionDistance))
        {
            IInteractable interactable = hitInfo.collider.GetComponentInParent<IInteractable>();
            if (interactable != null)
            {
                Debug.Log("Interacting with: " + interactable);
                interactable.Interact(this);
            }
        }
    }

    private void UpdateCoyoteTimer()
    {
        if (characterController.isGrounded)
        {
            if (!wasGroundedLastFrame)
            {
                currentJumpCount = maxJumpCount;
            }

            coyoteTimer = coyoteTime;
            isJumping = false;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }

        wasGroundedLastFrame = characterController.isGrounded;
    }

    private void HandleJumpWithDelay()
    {
        if (isPreparingToJump)
        {
            playerState = PlayerState.Jumping;
            jumpDelayTimer -= Time.deltaTime;
            if (jumpDelayTimer <= 0f)
            {
                isPreparingToJump = false;
                HandleJumping();
            }
        }
    }

    private void HandleJumping()
    {
        if (currentJumpCount == maxJumpCount && coyoteTimer > 0f && !IsOnSteepSlope(characterController.slopeLimit))
        {
            PerformJump();
        }
        else if (currentJumpCount < maxJumpCount && currentJumpCount > 0 && !IsOnSteepSlope(characterController.slopeLimit))
        {
            PerformJump();
        }

        if (isJumping && characterController.isGrounded)
        {
            isJumping = false;
        }
    }

    private void PerformJump() 
    {
        isPreparingToJump = false;
        jumpDelayTimer = 0f;

        currentMoveDirection.y = jumpVelocity;
        isJumping = true;
        currentJumpCount--;
    }

    private void UpdateAttackCooldown() 
    {
        
        if (attackTimer > 0f) attackTimer -= Time.deltaTime;
    }

    private void HandleAttack()
    {
        if (!canAttack || isAttacking) return;

        attackTimer = attackCooldown;
        currentSpeed = 0f;
        isAttacking = true;
        isInvincible = true;
        playerState = PlayerState.Attacking;

        Invoke(nameof(PerformAttack), attackDelay);

        Invoke(nameof(EndAttack), attackDuration);  
    }

    private void PerformAttack()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(attackOrigin.position, attackRadius, enemyLayerMask);
        foreach (Collider enemy in hitEnemies)
        { 
            enemy.GetComponent<EnemyAI>()?.TakeDamage(1f);
        }
    }

    private void EndAttack()
    {
        isAttacking = false;
        isInvincible = false;
    }

    private void HandleSlopeSliding()
    {
        if (IsOnSteepSlope(characterController.slopeLimit) && characterController.isGrounded && !isJumping)
        {
            Vector3 slopeNormal = hitSlope.normal;

            Vector3 slideDirection = Vector3.ProjectOnPlane(Vector3.down, slopeNormal).normalized;

            float slideSpeed = 200f;

            currentMoveDirection += slideDirection * slideSpeed * Time.deltaTime;
        }
    }

    private void HandleRotation()
    {
        Vector3 positionToLookAt;

        positionToLookAt.x = currentMoveDirection.x;
        positionToLookAt.y = 0f;
        positionToLookAt.z = currentMoveDirection.z;

        Quaternion currentRotation = transform.rotation;

        if (isMoving)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void ApplyMovement()
    {
        AdjustSpeedAcceleration();
        Vector3 horizontalMove = new Vector3();

        if (canWalk) 
        {
            horizontalMove = new Vector3(currentMoveDirection.x, 0f, currentMoveDirection.z) * currentSpeed;
        }

        Vector3 verticalMove = new Vector3(0f, currentMoveDirection.y, 0f);

        Vector3 platformMovement = Vector3.zero;
        if (platformDetector.CurrentMovingPlatform != null)
        {
            platformMovement = platformDetector.CurrentMovingPlatform.platformVelocity;
        }

        if (platformDetector.CurrentFragilePlatform != null)
        {
            if (weight > platformDetector.CurrentFragilePlatform.GetWeightLimit())
            {
                platformDetector.CurrentFragilePlatform.DestroyPlatform();
                platformDetector.ClearAllCurrentPlatform();
            }
        }

        Vector3 finalMove = horizontalMove + verticalMove + platformMovement;

        if (playerState == PlayerState.Stuned) 
        {
            finalMove = currentMoveDirection;
        }

        characterController.Move(finalMove * Time.deltaTime);
    }

    private void TriggerAnimationEvents()
    {
        switch (playerState)
        {
            case PlayerState.Idle:
                OnIdle?.Invoke(this, EventArgs.Empty);
                break;
            case PlayerState.Moving:
                OnMoving?.Invoke(this, EventArgs.Empty);
                break;
            case PlayerState.Jumping:
                OnJumping?.Invoke(this, EventArgs.Empty);
                break;
            case PlayerState.Attacking:
                OnAttack?.Invoke(this, EventArgs.Empty);
                break;
            case PlayerState.Stuned:
                OnStuned?.Invoke(this, EventArgs.Empty);
                break;
        }
    }

    private void SetupGravity()
    {
        float timeToTop = maxJumpTime / 2;
        gravity = (-2 * jumpHeight) / Mathf.Pow(timeToTop, 2f);
        jumpVelocity = (2 * jumpHeight) / timeToTop;
    }

    private bool IsOnSteepSlope(float slopeLimit)
    {
        float distanceToSteepSlope = 0.8f;
        if (Physics.Raycast(groundCheckPoint.position, Vector3.down, out hitSlope, distanceToSteepSlope, groundLayer))
        {
            Vector3 slopeNormal = hitSlope.normal;
            return Vector3.Angle(Vector3.up, slopeNormal) > slopeLimit;
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Key>(out Key key)) 
        {
            if (key.GetKeySO().keyName == "Golden Key") 
            {
                goldenKeyList.Add(key.GetKeySO());
                key.DestroyKeyObject();
                Debug.Log("Golden Key Added");
            }

            if (key.GetKeySO().keyName == "Silver Key") 
            {
                silverKeyList.Add(key.GetKeySO());
                key.DestroyKeyObject();
            }
        }

        if (other.TryGetComponent<SkillSphere>(out SkillSphere skillSphere))
        {
            OnSkillPickedUp?.Invoke(this, EventArgs.Empty);
            skillAmount++;
        }

        if (other.TryGetComponent<EnemyAI>(out EnemyAI enemyAI))
        {
            if (playerState == PlayerState.Stuned) return;

            if (healthPoint > 0 && !isInvincible)
            {
                currentMoveDirection.y = repulsionForce;
                currentMoveDirection.x = -repulsionForce;
                currentMoveDirection.z = -repulsionForce;
                GetDamage();
            }
            
            if (healthPoint <= 0)
            {
                Die();
            }
        }
    }

    private void UpdateStunTimer() 
    {
        if (playerState == PlayerState.Stuned) 
        {
            stunTimer -= Time.deltaTime;

            if (stunTimer <= 0)
            {
                isAttackTurnedOn = true;
                canWalk = true;
                playerState = PlayerState.Idle;
            }
        }
    }

    private void GetDamage() 
    {
        OnDecreasedHP?.Invoke(this, EventArgs.Empty);

        playerState = PlayerState.Stuned;
        stunTimer = stunTime;
        isAttackTurnedOn = false;
        canWalk = false;

        Debug.Log("DamageTaked");
        healthPoint--;
    }

    private void Die() 
    {
        Debug.Log("Died");
        gameObject.SetActive(false);
    }

    public int GetHPAmount() 
    {
        return healthPoint;
    }

    public void ResetJumpFlags()
    {
        isJumping = false;
        isPreparingToJump = false;
        jumpDelayTimer = 0f;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public Vector3 GetPosition() 
    {
        return transform.position;
    }

    public Vector3 GetLastMoveDirection() 
    {
        return lastMoveDirection;
    }

    public int GetSkillAmount() 
    {
        return skillAmount;
    }

    public bool HasRightKeySO(KeySO keySO) 
    {
        if (keySO.keyName == "Golden Key" && goldenKeyList.Count > 0)
        {
            return true;
        }
        else if (keySO.keyName == "Silver Key" && goldenKeyList.Count > 0)
        {
            return true;
        }
        else 
        {
            return false;
        }
    }

    public void RemoveKeyFromInventory(KeySO keySO) 
    {
        if (keySO.keyName == "Golden Key")
        {
            goldenKeyList.Remove(keySO);
        }
        else if (keySO.keyName == "Silver Key") 
        {
            silverKeyList.Remove(keySO);
        }
    }
}
