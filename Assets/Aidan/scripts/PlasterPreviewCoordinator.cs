using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using Unity.Mathematics;
using UnityEngine;

public class PlasterPreviewCoordinator : MonoBehaviour
{
    [SerializeField] HeldItemCoordinator heldScript;
    SpriteRenderer sRend;
    [SerializeField] Color validColor;
    [SerializeField] Color invalidColor;
    List<GameObject> selectedBricks = new List<GameObject>();

    [Header("Box cast")]
    [SerializeField] Vector3 centerOffset;
    [SerializeField] float boxSize = 2;
    [SerializeField] bool showBoxCast;


    bool valid = false;
    [HideInInspector] public bool enoughMortar = true;

    private void Start()
    {
        heldScript = GetComponentInParent<HeldItemCoordinator>();
        sRend = GetComponent<SpriteRenderer>();
    }

    /*private void OnEnable()
    {
        selectedBricks.Clear();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        var brick = collision.GetComponent<Brick>();
        if (brick && !brick.hasPlaster && !selectedBricks.Contains(collision.gameObject)) {
            selectedBricks.Add(collision.gameObject);
            return;
        }
    }*/

    private void Update()
    {
        List<Collider2D> brickColliders = BoxCastBricks();
        List<GameObject> bricks = new List<GameObject>();
        foreach (var b in brickColliders) {

            var brick = b.gameObject.GetComponent<Brick>();
            if (brick && brick.hasMortar) bricks.Add(b.gameObject);
        }
        selectedBricks = bricks;
        
        valid = selectedBricks.Count > 0;
        heldScript.selectedBricks = new List<GameObject>(selectedBricks);
        heldScript.valid = valid;
        sRend.color = valid ? validColor : invalidColor;
        selectedBricks.Clear();
    }

    List<Collider2D> BoxCastBricks()
    {
        Vector2 size = Vector2.one * boxSize;
        return Physics2D.OverlapBoxAll(transform.position + centerOffset, size, 0).ToList();
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 size = Vector3.one * boxSize;
        if (showBoxCast) Gizmos.DrawWireCube(centerOffset, size);
    }

}
