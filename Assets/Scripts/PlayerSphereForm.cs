using UnityEngine;

public class PlayerSphereForm : MonoBehaviour
{
    [SerializeField] private PlayerFormManager playerFormManager;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private Player player;
    [SerializeField] private float regularSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private LayerMask groundLayer;

    private float currentSpeed;
    private float maxDistanceToGround = 0.5f;
    private bool isFormChanged = false;
    private Vector3 lastMoveDirection;
    private Vector3 currentMoveDir;
    private bool isGrounded;
    private Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        currentSpeed = regularSpeed;

        gameInput.OnSprintStarted += GameInput_OnSprintStarted;
        gameInput.OnSprintCanceled += GameInput_OnSprintCanceled;
    }

    private void GameInput_OnSprintCanceled(object sender, System.EventArgs e)
    {
        currentSpeed = regularSpeed;
    }

    private void GameInput_OnSprintStarted(object sender, System.EventArgs e)
    {
        currentSpeed = sprintSpeed;
    }

    private void Update()
    {
        HandleMovement();

        Debug.Log(currentSpeed);
    }

    

    private void HandleMovement() 
    {
        Vector2 inputVector = gameInput.GetInputVectorNormalized();

        if (inputVector == Vector2.zero) return;

        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        currentMoveDir = right * inputVector.x + forward * inputVector.y;

        if (currentMoveDir.magnitude > 0.1f)
        {
            Vector3 velocity = currentMoveDir * currentSpeed;
            rigidbody.linearVelocity = new Vector3(velocity.x, rigidbody.linearVelocity.y, velocity.z);
        }
        else
        {
            float dampingSpeed = 10f;
            Vector3 horizontalVelocity = new Vector3(rigidbody.linearVelocity.x, 0f, rigidbody.linearVelocity.z);
            horizontalVelocity = Vector3.Lerp(horizontalVelocity, Vector3.zero, dampingSpeed * Time.deltaTime);
            rigidbody.linearVelocity = new Vector3(horizontalVelocity.x, rigidbody.linearVelocity.y, horizontalVelocity.z);
        }
    }

    public void Show() 
    {
        rigidbody.linearVelocity = Vector3.zero;
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

    public void SetMoveDirection(Vector3 moveDirection) 
    {
        currentMoveDir = moveDirection;
    }
}
