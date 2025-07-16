using System.Collections;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float waitTime;
    [SerializeField] private float speed = 2f;

    private Vector3 lastPosition;
    private Coroutine moveCoroutine;

    [HideInInspector]
    public Vector3 platformVelocity;

    private void Start()
    {
        lastPosition = transform.position;
        transform.position = pointA.position;
    }

    private void OnEnable()
    {
        lastPosition = pointA.position;

        moveCoroutine = StartCoroutine(MovePlatform());
    }

    private void OnDisable()
    {
        if (moveCoroutine != null) 
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
    }

    private IEnumerator MovePlatform()
    {
        Vector3 target = pointB.position;

        while (true)
        {
            while (Vector3.Distance(transform.position, target) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

                platformVelocity = (transform.position - lastPosition) / Time.deltaTime;
                lastPosition = transform.position;

                yield return null;
            }

            platformVelocity = Vector3.zero;
            yield return new WaitForSeconds(waitTime);

            target = (target == pointA.position) ? pointB.position : pointA.position;
        }
    }

    private void OnDrawGizmos()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(pointA.position, pointB.position);

            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(pointA.position, transform.localScale);

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(pointB.position, transform.localScale);
        }
    }
}
