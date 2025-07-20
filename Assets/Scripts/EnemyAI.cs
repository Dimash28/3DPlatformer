using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private GameObject player;

    [SerializeField] private Transform originPoint;
    [SerializeField] private BoxCollider groundCheck;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private float health;

    [SerializeField] private float roamRadius;
    [SerializeField] private float detectionRange;
    [SerializeField] private float attackRadius;

    [SerializeField] private float maxSpeed;
    [SerializeField] private float speedAcceleration;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float damage;
    [SerializeField] private float waitingTime;
    [SerializeField] private float playerLoseTime;

    [SerializeField] private LayerMask playerLayerMask;

    private Vector3 randomPoint;

    private bool isWaiting = false;
    private bool isMoving;
    private float waitingTimer;
    private float currentSpeed;
    private float playerLoseTimer;

    private EnemyState currentState;

    private enum EnemyState
    {
        Roaming,
        Chasing,
        Attacking,
        Dead
    }

    private void Start()
    {
        currentState = EnemyState.Roaming;
        PickRandomPointToGo();
        isMoving = true;
    }

    private void Update()
    {
        switch (currentState)
        {
            case EnemyState.Roaming:
                HandleRoaming();
                break;
            case EnemyState.Chasing:
                HandleChasing();
                break;
            case EnemyState.Attacking:
                break;
        }
    }

    private void HandleChasing()
    {
        if (!IsPlayerInRange())
        {
            if (!IsGroundAhead())
            {
                currentState = EnemyState.Roaming;
                return;
            }

            playerLoseTimer += Time.deltaTime;

            HandleMovementToTarget(player.transform.position);

            if (playerLoseTimer >= playerLoseTime)
            {
                playerLoseTimer = 0f;
                currentState = EnemyState.Roaming;
                return;
            }
        }
        else
        {
            if (IsGroundAhead())
            {
                HandleMovementToTarget(player.transform.position);
            }
            else
            {
                currentState = EnemyState.Roaming;
                currentSpeed = 0f;
                PickRandomPointToGo();
                HandleMovementToTarget(randomPoint);
            }
        }
    }

    private void HandleRoaming()
    {
        float distance = Vector3.Distance(transform.position, randomPoint);

        if (IsPlayerInRange())
        {
            currentState = EnemyState.Chasing;
        }
        else
        {
            if (distance <= 0.1f)
            {
                isMoving = false;
                isWaiting = true;

                waitingTimer += Time.deltaTime;

                if (waitingTimer >= waitingTime)
                {
                    PickRandomPointToGo();
                    isWaiting = false;
                    isMoving = true;
                }
            }
            else
            {
                if (IsGroundAhead())
                {
                    HandleMovementToTarget(randomPoint);
                }
                else
                {
                    currentSpeed = 0f;
                    PickRandomPointToGo();
                    HandleMovementToTarget(randomPoint);
                }
                waitingTimer = 0f;
            }
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
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, speedAcceleration * Time.deltaTime);
        }
    }

    private void HandleMovementToTarget(Vector3 target)
    {
        AdjustSpeedAcceleration();

        Vector3 moveDir = (target - transform.position).normalized;

        ApplyMovementAndRotation(moveDir);
    }

    private void ApplyMovementAndRotation(Vector3 moveDir) 
    {
        transform.position += new Vector3(moveDir.x, 0f, moveDir.z) * currentSpeed * Time.deltaTime;

        if (isMoving && moveDir != Vector3.zero)
        {
            Vector3 positionToLookAt = new Vector3(moveDir.x, 0f, moveDir.z);
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void PickRandomPointToGo() 
    {
        Vector2 randomOffset = Random.insideUnitCircle * roamRadius;
        randomPoint = originPoint.position + new Vector3(randomOffset.x, 0f, randomOffset.y);
    }

    private bool IsPlayerInRange()
    {
        Collider[] players = Physics.OverlapSphere(transform.position, detectionRange, playerLayerMask);
        return players.Length > 0;
    }

    private bool IsGroundAhead() 
    {
        Vector3 center = groundCheck.transform.position + groundCheck.center;
        Vector3 halfExtents = groundCheck.size * 0.5f;

        Collider[] hits = Physics.OverlapBox(center, halfExtents, groundCheck.transform.rotation, groundLayer);

        return hits.Length > 0;
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0f)
        {
            Die();
        }
    }
}
