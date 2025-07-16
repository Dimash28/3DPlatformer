using UnityEngine;

public class MovingPlatformSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject platform;
    [SerializeField] private GameObject platformPreviewA;
    [SerializeField] private GameObject platformPreviewB;

    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = platform.GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        meshRenderer.enabled = true;
        platformPreviewA.SetActive(false);
        platformPreviewB.SetActive(false);
    }
}
