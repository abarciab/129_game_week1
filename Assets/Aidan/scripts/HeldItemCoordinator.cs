using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldItemCoordinator : MonoBehaviour
{
    [SerializeField] PlayerController.ItemType type;
    public bool valid;

    public GameObject brickGO;
    public GameObject mortarGO;
    [SerializeField] GameObject plasterGO;

    [HideInInspector] public GameObject selectedBrick;
    //[HideInInspector] public List<GameObject> selectedBricks = new List<GameObject>();
    public List<GameObject> selectedBricks = new List<GameObject>();

    [HideInInspector] public PlayerController player;
    [HideInInspector] public float MortarRefilled;

    [SerializeField] Vector2 xMinMaxBricks;
    [SerializeField] Vector2 yMinMaxBricks;

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

    public void GetNewBrickSize()
    {
        brickGO.transform.localScale = new Vector3(Random.Range(xMinMaxBricks.x, xMinMaxBricks.y), Random.Range(yMinMaxBricks.x, yMinMaxBricks.y), 1);
    }

}
