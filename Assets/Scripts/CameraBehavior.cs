using UnityEngine;
using Unity.Cinemachine;

public class CameraBehavior : MonoBehaviour
{
    [SerializeField] private Transform cameraPivotPoint;
    [SerializeField] private Transform cameraPivotPointSphereForm;
    private CinemachineCamera playerMainCamera;

    private float normalFOV = 80f;
    private float sphereFOV = 60f;
    private float transitionSpeed = 5f;
    private float currentFOV;

    private void Start()
    {
        playerMainCamera = GetComponent<CinemachineCamera>();
        if (playerMainCamera == null)
        {
            Debug.LogError("CinemachineCamera not found in the scene.");
        }
    }

    public void ChangeCameraFOV(bool isSphereForm)
    {
        if (playerMainCamera == null) return;

        currentFOV = isSphereForm ? sphereFOV : normalFOV;
        playerMainCamera.Target.TrackingTarget = isSphereForm ? cameraPivotPointSphereForm : cameraPivotPoint;
        playerMainCamera.Lens.FieldOfView = Mathf.Lerp(playerMainCamera.Lens.FieldOfView, currentFOV, Time.deltaTime * transitionSpeed);
    }
}
