using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public enum ItemType { Brick, Mortar, Plaster}

    [Header("movement")]
    [SerializeField] float speed;
    [SerializeField] float maxItemDist = 5f, minItemDist = 1, minxItemDist = 1f, smoothness = 0.05f;
    [SerializeField] Vector2 playerPosOffset;
    Vector2 move;
    Vector3 targetPos;
    bool left;

    [Header("Brick")]
    [SerializeField] float brickHeight = 1;
    [SerializeField] int brickPlaceSoundID;

    [Header("Mortar")]
    [SerializeField] float mortarCost = 0.2f;
    //[SerializeField] float mortarRefillAmount = 1;
    [SerializeField] int mortarPlaceSoundID = 2;
    [SerializeField] int refilSoundID = 3;
    float mortarRemaining = 1;

    [Header("misc")]
    [SerializeField] ItemType currentItem;
    [HideInInspector] public TextMeshProUGUI dataDisplay;
    [HideInInspector] public GameObject heldItemGO;
    HeldItemCoordinator HIcoord;
    PlayerControls controls;

    [Header("Prefabs")]
    [SerializeField] GameObject heldItemPrefab;
    [SerializeField] GameObject brickPrefab;

    void Awake()
    {
        controls = new PlayerControls();
    }

    void Start()
    {
        heldItemGO = Instantiate(heldItemPrefab, transform.position, quaternion.identity);
        HIcoord = heldItemGO.GetComponent<HeldItemCoordinator>();
        HIcoord.player = this;
        GameManager.instance.RegisterNewPlayer(this);
        
        CameraController.instance.players.Add(this);
    }

    void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    void Update()
    {
        if (dataDisplay != null) dataDisplay.text = gameObject.name + ": " + currentItem;

        if (heldItemGO.transform.position.x < GameManager.instance.bounds.x && move.x < 0 || heldItemGO.transform.position.x > GameManager.instance.bounds.y && move.x > 0) return;
        heldItemGO.transform.Translate(move * Time.deltaTime * speed);
    }
    
    void LateUpdate()
    {
        MoveCharacter();
        if (currentItem == ItemType.Mortar) UpdateMortarVisuals();
    }

    void UpdateMortarVisuals()
    {
        HIcoord.mortarGO.GetComponent<MortarPreviewCoordinator>().enoughMortar = mortarRemaining > mortarCost;
    }

    bool leftTriggerIsDown;
    public void RotateBrickLeft(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) leftTriggerIsDown = true;
        if (ctx.canceled) leftTriggerIsDown = false;
        if (currentItem != ItemType.Brick || !ctx.started || leftTriggerIsDown) return;

        AudioManager.instance.PlaySound(6, gameObject);
        HIcoord.brickGO.transform.Rotate(0, 0, 90);
    }

    bool rightTriggerIsDown;
    public void RotateBrickRight(InputAction.CallbackContext ctx)
    {
        //print("started: " + ctx.started + ", cancelled: " + ctx.canceled + ", performed: " + ctx.performed);
        if (ctx.performed) rightTriggerIsDown = true;
        if (ctx.canceled) rightTriggerIsDown = false;
        if (currentItem != ItemType.Brick || !ctx.started || rightTriggerIsDown) return;

        AudioManager.instance.PlaySound(6, gameObject);
        HIcoord.brickGO.transform.Rotate(0, 0, 90);
    }

    public void SecondaryUse(InputAction.CallbackContext ctx)
    {
        return;
        /*if (!ctx.started) return;

        if (currentItem == ItemType.Mortar && mortarRemaining < mortarCost) */
    }

    void StartMortarQTE()
    {
        //mortarRemaining = mortarRefillAmount;
        HIcoord.mortarGO.GetComponent<MortarPreviewCoordinator>().Refill();
    }

    public void PlaceItem(InputAction.CallbackContext ctx)
    {
        if (HIcoord == null) return;

        if (currentItem == ItemType.Mortar && ctx.started) AddMortar();

         if(!HIcoord.valid) return;

        if (currentItem == ItemType.Brick && ctx.started) PlaceBrick();
        if (currentItem == ItemType.Plaster && ctx.started) PlasterBricks();
    }

    void PlasterBricks()
    {
        if (HIcoord.selectedBricks.Count == 0) return;
        bool valid = false;
        for (int i = 0; i < HIcoord.selectedBricks.Count; i++) {
            if (!HIcoord.selectedBricks[i].GetComponent<Brick>().hasPlaster) valid = true;
        }
        if (!valid) return;

        AudioManager.instance.PlaySound(10, gameObject);

        foreach (var b in HIcoord.selectedBricks) {
            b.GetComponent<Brick>().Plaster();
        }
    }

    public void VoteToEnd(InputAction.CallbackContext ctx)
    {
        if (!ctx.started) return;

        GameManager.instance.ToggleEndVote(this);
    }
    
    public void RefillMortar(float amount)
    {
        AudioManager.instance.PlaySound(2, gameObject);
        mortarRemaining = amount;
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
        AudioManager.instance.PlaySound(7, gameObject);
        HIcoord.ChangeItem(currentItem);
    }
    void AddMortar()
    {
        if (mortarRemaining < mortarCost) {
            StartMortarQTE();
            return;
        }
        if (HIcoord.selectedBrick == null) return;
        HIcoord.selectedBrick.GetComponent<Brick>().hasMortar = true;
        mortarRemaining -= mortarCost;
        AudioManager.instance.PlaySound(mortarPlaceSoundID, gameObject);
    }

    void PlaceBrick()
    {
        if (GameManager.instance == null || heldItemGO == null) return;
        var newBrick = Instantiate(brickPrefab, heldItemGO.transform.position, HIcoord.brickGO.transform.rotation, GameManager.instance.towerParent);
        newBrick.transform.localScale = HIcoord.brickGO.transform.lossyScale;
        heldItemGO.transform.position += Vector3.up * brickHeight;
        AudioManager.instance.PlaySound(brickPlaceSoundID, gameObject);
        HIcoord.GetNewBrickSize();
    }

    public void MoveHeldItem(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled) move = Vector2.zero;
        else move = ctx.ReadValue<Vector2>();
    }

    void MoveCharacter()
    {
        left = heldItemGO.transform.position.x > 0;

        Vector3 offset = new Vector3(left ? playerPosOffset.x : playerPosOffset.x * -1, playerPosOffset.y, 0);
        var distY = Mathf.Abs(transform.position.y - heldItemGO.transform.position.y);
        var distX = Mathf.Abs(transform.position.x - heldItemGO.transform.position.x);
        var dist = Vector2.Distance(transform.position, heldItemGO.transform.position);
        if (distX < minxItemDist) {
            var newtarget = heldItemGO.transform.position + offset;
            newtarget.y = transform.position.y;
            targetPos = Vector3.Lerp(targetPos, newtarget, smoothness);
        }
        if (dist < minItemDist || dist > maxItemDist) {
            targetPos = heldItemGO.transform.position + offset;
        }
        transform.position = Vector2.Lerp(transform.position, targetPos + offset, smoothness);
        /*

        if (Vector2.Distance(transform.position, heldItemPrefab.transform.position) < minItemDist) {
            if (move.x > 0.001) left = false;
            else if (move.x < -0.001) left = true;
        }
        
        //print("playeroffset: " + playerPosOffset + ", offset: " + offset);

        if (Vector2.Distance(transform.position, heldItemGO.transform.position) > maxItemDist)
            targetPos = Vector2.Lerp(targetPos, heldItemGO.transform.position, smoothness);
        else targetPos = Vector2.Lerp(targetPos, transform.position - (Vector3)offset, smoothness);
        
        //transform.position = heldItemRB.transform.position + (Vector3) offset;*/
    }


    void OnDisable()
    {
        controls.Gameplay.Disable();
    }

}
