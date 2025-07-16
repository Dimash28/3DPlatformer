using System;
using UnityEngine;

public class SkillSphere : MonoBehaviour
{
    [SerializeField] private Player player;
    private void Start()
    {
        player.OnSkillPickedUp += Player_OnSkillPickedUp;
    }
    
    private void Player_OnSkillPickedUp(object sender, EventArgs e)
    {
        Debug.Log("Skill Sphere picked up!");
        Destroy(gameObject); // Destroy the sphere after pickup
    }
}
