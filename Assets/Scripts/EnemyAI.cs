using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Transform originPoint;

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

    [SerializeField] private LayerMask playerLayerMask;

    private Vector3 randomPoint;

    private bool isWaiting = false;
    private bool isMoving;
    private float waitingTimer;
    private float currentSpeed;

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
                break;
            case EnemyState.Attacking:
                break;

        }
    }

    private void HandleRoaming()
    {
        AdjustSpeedAcceleration();
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
                HandleMovementToRandomPoint();
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

    private void HandleMovementToRandomPoint() 
    {
        Vector3 moveDir = (randomPoint - transform.position).normalized;

        transform.position += moveDir * currentSpeed * Time.deltaTime;


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
