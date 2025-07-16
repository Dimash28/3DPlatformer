using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float health;
    [SerializeField] private float detectionRange;
    [SerializeField] private float attackRadius;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float damage;
    [SerializeField] private LayerMask playerLayerMask;

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
        if (IsPlayerInRange())
        {
            currentState = EnemyState.Chasing;
        }
        else
        {
        }
    }

    private bool IsPlayerInRange()
    {
        Collider[] players = Physics.OverlapSphere(transform.position, detectionRange, playerLayerMask);
        return players.Length > 0;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
