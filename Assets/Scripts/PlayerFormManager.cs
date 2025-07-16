using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerFormManager : MonoBehaviour
{
    [SerializeField] private GameInput gameInput;
    [SerializeField] private Player player;
    [SerializeField] private Player playerSphereForm;
    [SerializeField] private CameraBehavior playerMainCamera;
    public event EventHandler OnFormChanged;

    private bool isInSphereForm = false;

    private void Start()
    {
        player.Show();
        playerSphereForm.Hide();

        gameInput.OnFormChanged += GameInput_OnFormChanged;
    }

    private void Update()
    {
        playerMainCamera.ChangeCameraFOV(isInSphereForm);
    }
    private void SyncPosition()
    {
        Vector3 offset = new Vector3(0f, 0.1f, 0f);
        Vector3 playerPosition = player.GetPosition();
        Vector3 playerSpherePosition = playerSphereForm.GetPosition();

        if (!isInSphereForm)
        {
            playerSphereForm.transform.position = playerPosition + offset;
        }
        else
        {
            player.transform.position = playerSpherePosition + offset;
        }
    }

    private void GameInput_OnFormChanged(object sender, System.EventArgs e)
    {
        SyncPosition();
        ChangeForm();
    }

    private void ChangeForm() 
    {
        if (player.GetSkillAmount() > 0)
        {
            if (isInSphereForm && !CanChangeToNormalForm())
            {
                return;
            }

            OnFormChanged?.Invoke(this, EventArgs.Empty);

            if (!isInSphereForm)
            {
                isInSphereForm = true;

                playerSphereForm.ResetJumpFlags();
                player.Hide();
                playerSphereForm.Show();

                if (playerSphereForm.PlatformDetector.CurrentMovingPlatform != null)
                {
                    playerSphereForm.PlatformDetector.ClearAllCurrentPlatform();
                }
            }
            else
            {
                isInSphereForm = false;

                player.ResetJumpFlags();
                playerSphereForm.Hide();
                player.Show();

                if (player.PlatformDetector.CurrentMovingPlatform != null)
                {
                    player.PlatformDetector.ClearAllCurrentPlatform();
                }
            }
        }
    }

    private bool CanChangeToNormalForm()
    {
        float playerHeight = player.GetComponent<CharacterController>().height;
        float radius = player.GetComponent<CharacterController>().radius;
        Vector3 bottom = player.transform.position + Vector3.up * radius;
        Vector3 top = bottom + Vector3.up * (playerHeight - radius * 2f);

        return !Physics.CheckCapsule(bottom, top, radius, LayerMask.GetMask("Ground"));
    }
}
