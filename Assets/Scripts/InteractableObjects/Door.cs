using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform openPivot;
    [SerializeField] private KeySO keySO;


    private bool isOpened = false;
    public void Interact(Player player)
    {
        if (isOpened) return;

        if (player.HasRightKeySO(keySO))
        {
            isOpened = true;
            StartCoroutine(OpenDoor());
            Debug.Log("Door opened");
            player.RemoveKeyFromInventory(keySO);
        }
        else 
        {
            Debug.Log("You don't have right key!");
        }
    }

    private IEnumerator OpenDoor()
    {
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0, 90, 0);
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation;
    }
}
