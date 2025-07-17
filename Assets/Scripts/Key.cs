using UnityEngine;

public class Key : MonoBehaviour
{
    [SerializeField] private KeySO keySO;

    public KeySO GetKeySO() 
    {
        return keySO;
    }

    public void DestroyKeyObject() 
    {
        Destroy(gameObject);
    }
}
