using System.Collections;
using UnityEngine;

public class FragilePlatform : MonoBehaviour
{
    [SerializeField] private FragilePlatformRespawner fragilePlatformRespawner;

    [SerializeField] private int weightLimit;

    public int GetWeightLimit()
    {
        return weightLimit;
    }

    public void DestroyPlatform()
    {
        gameObject.SetActive(false);
        fragilePlatformRespawner.RespawnFragilePlatform();
    }
}
