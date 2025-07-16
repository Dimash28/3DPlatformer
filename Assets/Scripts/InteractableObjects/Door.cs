using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    private bool isOpened = false;
    public void Interact(Player player)
    {
        if (isOpened) return;

        isOpened = true;
        StartCoroutine(OpenDoor());
        Debug.Log("Door opened");
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
