using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToolSwitching : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI toolLabel, voteText;
    [SerializeField] Image toolImg, checkBox;
    [SerializeField] Sprite checkedBox, uncheckedBox, mortar, plaster, brick, hammer;
    public MortarGagueCoordinator mortarCoord;
    public GameObject greyOut;
    public Color charColor;

    public void Activate()
    {
        greyOut.SetActive(false);
    }

    public void SwitchTool(PlayerController.ItemType item)
    {
        mortarCoord.active = item == PlayerController.ItemType.Mortar;
        switch (item) {
            case PlayerController.ItemType.Brick:
                toolImg.sprite = brick;
                toolLabel.text = "Brick";
                break;
            case PlayerController.ItemType.Mortar:
                toolImg.sprite = mortar;
                toolLabel.text = "Mortar";
                break;
            case PlayerController.ItemType.Plaster:
                toolImg.sprite = plaster;
                toolLabel.text = "Plaster";
                break;
            case PlayerController.ItemType.Hammer:
                toolImg.sprite = hammer;
                toolLabel.text = "Hammer";
                break;
        }
    }
    
    public void SetVoteGraphics(bool voted)
    {
        checkBox.color = voteText.color = voted ? Color.white : new Color(1, 1, 1, 0.5f);
        checkBox.sprite = voted ? checkedBox : uncheckedBox;
    }

    public void SetMortarValue(float mortarvalue)
    {
        mortarCoord.slider.value = mortarvalue / 2.5f;
    }

}
