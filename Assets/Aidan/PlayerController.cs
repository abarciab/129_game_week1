using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public enum ItemType { Brick, Mortar, Plaster}

    [SerializeField] float speed, maxItemDist = 5f, minDist = 1, smoothness = 0.05f;
    [SerializeField] Vector2 playerPosOffset;
    [SerializeField] float brickHeight = 1;
    [SerializeField] ItemType currentItem;

    [Header("Prefabs")]
    [SerializeField] GameObject heldItemPrefab;
    [SerializeField] GameObject brickPrefab;


    [HideInInspector] public Rigidbody2D heldItemRB;
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
        var _heldItem = Instantiate(heldItemPrefab, transform.position, quaternion.identity);
        heldItemRB = _heldItem.GetComponent<Rigidbody2D>();
        
        CameraController.instance.players.Add(this);
    }

    void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    void Update()
    {
        heldItemRB.velocity = Vector2.Lerp(heldItemRB.velocity, move * Time.deltaTime * speed, smoothness);
    }
    
    void LateUpdate()
    {
        MoveCharacter();
    }

    public void PlaceItem(InputAction.CallbackContext ctx)
    {
        if (currentItem == ItemType.Brick && ctx.started) PlaceBrick();
        if (currentItem == ItemType.Mortar && ctx.started) AddMortar();
    }

    void AddMortar()
    {

    }

    void PlaceBrick()
    {
        if (GameManager.instance == null || heldItemRB == null) return;
        Instantiate(brickPrefab, heldItemRB.transform.position, quaternion.identity, GameManager.instance.towerParent);
        heldItemRB.transform.position += Vector3.up * brickHeight;
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

        if (Vector2.Distance(transform.position, heldItemRB.transform.position) > maxItemDist)
            targetPos = Vector2.Lerp(targetPos, heldItemRB.transform.position, smoothness);
        else targetPos = Vector2.Lerp(targetPos, transform.position - (Vector3)offset, smoothness);
        transform.position = Vector2.Lerp(transform.position, targetPos + (Vector3)offset, smoothness);
        //transform.position = heldItemRB.transform.position + (Vector3) offset;
    }


    void OnDisable()
    {
        controls.Gameplay.Disable();
    }

}
