using UnityEditor.SceneManagement;
using UnityEngine;

[CreateAssetMenu]
public class KeySO : ScriptableObject
{
    public GameObject keyPrefab;
    public string keyName;
    public Sprite icon;
}
