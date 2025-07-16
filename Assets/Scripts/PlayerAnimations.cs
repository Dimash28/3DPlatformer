using UnityEngine;
using UnityEngine.Animations;

public class PlayerAnimations : MonoBehaviour
{
    [SerializeField] private Player player;

    private const string IDLE = "Idle";
    private const string MOVE = "MoveFWD";
    private const string JUMP = "Jump";
    private const string ATTACK = "Attack";
    private const string GETHIT = "GetHit";

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found on PlayerAnimations script.");
        }
    }

    private void Start()
    {
        player.OnIdle += Player_OnIdle;
        player.OnMoving += Player_OnMoving;
        player.OnJumping += Player_OnJumping;
        player.OnAttack += Player_OnAttack;
        player.OnStuned += Player_OnStuned;
    }

    private void Player_OnStuned(object sender, System.EventArgs e)
    {
        Debug.Log("GetHitAnimationTriggered");
        PlayOnce(GETHIT);
    }

    private void Player_OnIdle(object sender, System.EventArgs e)
    {
        animator.Play(IDLE);
    }

    private void Player_OnMoving(object sender, System.EventArgs e)
    {
        animator.Play(MOVE);
    }

    private void Player_OnJumping(object sender, System.EventArgs e)
    {
        PlayOnce(JUMP);
    }
    
    private void Player_OnAttack(object sender, System.EventArgs e)
    {
        PlayOnce(ATTACK);
    }

    private void PlayOnce(string animName)
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(animName))
        {
            animator.Play(animName);
        }
    }
}
