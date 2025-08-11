using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPContainerUI : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Player playerSphereForm;
    [SerializeField] private Image imageHP;

    private List<Image> imageHPList = new List<Image>();

    private void Start()
    {
        player.OnDecreasedHP += Player_OnDecreasedHP;
        playerSphereForm.OnDecreasedHP += PlayerSphereForm_OnDecreasedHP;

        CreateHPUI(player.GetHPAmount());
    }

    private void PlayerSphereForm_OnDecreasedHP(object sender, System.EventArgs e)
    {
        SubtractHP();
    }

    private void Player_OnDecreasedHP(object sender, System.EventArgs e)
    {
        SubtractHP();
    }

    private void AddHP()
    {
        imageHPList.Add(imageHP);
    }

    private void SubtractHP()
    {
        if (imageHPList.Count > 0) 
        {
            Image lastHPImage = imageHPList[imageHPList.Count - 1];
            imageHPList.Remove(lastHPImage);
            Destroy(lastHPImage.gameObject);
        }
    }

    private void CreateHPUI(int amount) 
    {
        for (int i = 0; i < amount; i++)
        {
            Image newImageHP = Instantiate(imageHP, transform);
            imageHPList.Add(newImageHP);
        }
    }
}
