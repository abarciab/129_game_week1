using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public enum ItemType { Brick, Mortar, Plaster}

    [SerializeField] float speed, maxItemDist = 5f, minDist = 1, smoothness = 0.05f;
    [SerializeField] Vector2 playerPosOffset;
    [SerializeField] float brickHeight = 1;
    [SerializeField] ItemType currentItem;
    [HideInInspector] public TextMeshProUGUI dataDisplay;
    

    [Header("Prefabs")]
    [SerializeField] GameObject heldItemPrefab;
    [SerializeField] GameObject brickPrefab;

    HeldItemCoordinator HIcoord;
    [HideInInspector] public GameObject heldItemGO;
    PlayerControls controls;
    Vector2 move;
    Vector3 targetPos;
    bool left;
    

    void Awake()
    {
        controls = new PlayerControls();
    }

    void Start()
    {
        heldItemGO = Instantiate(heldItemPrefab, transform.position, quaternion.identity);
        HIcoord = heldItemGO.GetComponent<HeldItemCoordinator>();
        GameManager.instance.RegisterNewPlayer(this);
        
        CameraController.instance.players.Add(this);
    }

    void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    void Update()
    {
        heldItemGO.transform.Translate(move * Time.deltaTime * speed);
        if (dataDisplay != null) dataDisplay.text = gameObject.name + ": " + currentItem;
    }
    
    void LateUpdate()
    {
        MoveCharacter();
    }

    public void PlaceItem(InputAction.CallbackContext ctx)
    {
        if (HIcoord == null || !HIcoord.valid) return;

        if (currentItem == ItemType.Brick && ctx.started) PlaceBrick();
        if (currentItem == ItemType.Mortar && ctx.started) AddMortar();
    }

    public void HoldNextItem(InputAction.CallbackContext ctx)
    {
        if (!ctx.started) return;

        if ((int) currentItem < 2)
            currentItem += 1;
        else currentItem = 0;
        UpdateHeldItemDisplay();
    }

    public void HoldPreviousItem(InputAction.CallbackContext ctx)
    {
        if (!ctx.started) return;

        if ((int)currentItem > 0)
            currentItem -= 1;
        else currentItem = (ItemType) 2;
        UpdateHeldItemDisplay();
    }

    void UpdateHeldItemDisplay()
    {
        HIcoord.ChangeItem(currentItem);
    }
    void AddMortar()
    {
        HIcoord.selectedBrick.GetComponent<Brick>().hasMortar = true;
    }

    void PlaceBrick()
    {
        if (GameManager.instance == null || heldItemGO == null) return;
        Instantiate(brickPrefab, heldItemGO.transform.position, quaternion.identity, GameManager.instance.towerParent);
        heldItemGO.transform.position += Vector3.up * brickHeight;
    }

    public void MoveHeldItem(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled) move = Vector2.zero;
        else move = ctx.ReadValue<Vector2>();
    }

    void MoveCharacter()
    {
        if (Vector2.Distance(transform.position, heldItemPrefab.transform.position) < 1) {
            if (move.x > 0.001) left = false;
            else if (move.x < -0.001) left = true;
        }
        Vector3 offset = new Vector3(left ? playerPosOffset.x : playerPosOffset.x * -1, playerPosOffset.y, 0);
        //print("playeroffset: " + playerPosOffset + ", offset: " + offset);

        if (Vector2.Distance(transform.position, heldItemGO.transform.position) > maxItemDist)
            targetPos = Vector2.Lerp(targetPos, heldItemGO.transform.position, smoothness);
        else targetPos = Vector2.Lerp(targetPos, transform.position - (Vector3)offset, smoothness);
        transform.position = Vector2.Lerp(transform.position, targetPos + (Vector3)offset, smoothness);
        //transform.position = heldItemRB.transform.position + (Vector3) offset;
    }


    void OnDisable()
    {
        controls.Gameplay.Disable();
    }

}
