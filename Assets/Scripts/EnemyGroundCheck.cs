using System;
using UnityEngine;

public class EnemyGroundCheck : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    private bool isGroundAhead = true;

    private void Update()
    {
        Debug.Log("IsGroundAhead = " + isGroundAhead);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter");
        if (((1 << other.gameObject.layer) & groundLayer) != 0) 
        {
            isGroundAhead = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("OnTriggerExit");
        isGroundAhead = false;
    }

    public bool IsGroundAhead() 
    {
        return isGroundAhead;
    }
}
