using Unity.VisualScripting;
using UnityEngine;

public class PlatformDetector : MonoBehaviour
{
    [SerializeField] private Player player;
    public MovingPlatform CurrentMovingPlatform { get; private set; }
    public FragilePlatform CurrentFragilePlatform { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<MovingPlatform>(out MovingPlatform movingPlatform))
        {
            CurrentMovingPlatform = movingPlatform;
            Debug.Log("PlatformDetected");
        }

        if (other.TryGetComponent<FragilePlatform>(out FragilePlatform fragilePlatform)) 
        {
            CurrentFragilePlatform = fragilePlatform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<MovingPlatform>(out MovingPlatform movingPlatform) && movingPlatform == CurrentMovingPlatform) 
        {
            ClearCurrentMovingPlatform();
        }

        if (other.TryGetComponent<FragilePlatform>(out FragilePlatform fragilePlatform) && fragilePlatform == CurrentFragilePlatform)
        {
            ClearCurrentFragilePlatform();
        }
    }

    public void ClearAllCurrentPlatform() 
    {
        CurrentFragilePlatform = null;
        CurrentMovingPlatform = null;
    }
    public void ClearCurrentMovingPlatform() 
    {
        CurrentMovingPlatform = null;
    }
    public void ClearCurrentFragilePlatform()
    {
        CurrentFragilePlatform = null;
    }
}
