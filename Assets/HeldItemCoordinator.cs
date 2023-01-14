using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldItemCoordinator : MonoBehaviour
{
    public bool valid;

    [SerializeField] PlayerController.ItemType type;
    [SerializeField] GameObject brickGO;
    [SerializeField] GameObject mortarGO;
    [SerializeField] GameObject plasterGO;
    public GameObject selectedBrick;

    private void Start()
    {
        ChangeItem(PlayerController.ItemType.Brick);
    }

    public void ChangeItem(PlayerController.ItemType newItem)
    {
        brickGO.SetActive(false);
        mortarGO.SetActive(false);
        plasterGO.SetActive(false);

        switch (newItem) {
            case PlayerController.ItemType.Brick:
                brickGO.SetActive(true);
                break;
            case PlayerController.ItemType.Mortar:
                mortarGO.SetActive(true);
                break;
            case PlayerController.ItemType.Plaster:
                plasterGO.SetActive(true);
                break;
        }
    }
    
}
