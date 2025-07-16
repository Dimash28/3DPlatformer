using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class FragilePlatformRespawner : MonoBehaviour
{
    [SerializeField] private FragilePlatform fragilePlatform;
    [SerializeField] private float respawnTime;

    public void RespawnFragilePlatform()
    {
        StartCoroutine(RespawnPlatform());
    }

    private IEnumerator RespawnPlatform()
    {
        yield return new WaitForSeconds(respawnTime);
        fragilePlatform.gameObject.SetActive(true);
    }
}
