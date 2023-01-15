using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortarPreviewCoordinator : MonoBehaviour
{
    [SerializeField] HeldItemCoordinator heldScript;
    SpriteRenderer sRend;
    [SerializeField] Color validColor;
    [SerializeField] Color invalidColor;
    GameObject selectedBrick;

    [Header("QTE")]
    [SerializeField] GameObject bar;
    [SerializeField] GameObject SweetSpot, cursor;
    [SerializeField] float cursorSpeed, cursorStartPos, cursorEndPos, bonusStartPos, bonusEndPos;
    [SerializeField] Vector3 minNormalBonusAmount = Vector3.one;
    
    bool valid = false;
    [HideInInspector] public bool enoughMortar = true;

    private void Start()
    {
        sRend = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        var brick = collision.GetComponent<Brick>();
        if (brick && !brick.hasMortar) {
            selectedBrick = collision.gameObject;
            valid = true;
            return;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (selectedBrick == collision.gameObject) selectedBrick = null;
        valid = false;
    }

    public void Refill()
    {
        if (bar.activeInHierarchy) CompleteQTE();
        else StartQTE();
    }

    void CompleteQTE(bool end = false)
    {
        float amount = end ? minNormalBonusAmount.y : CursorInSweetSpot() ? minNormalBonusAmount.z : minNormalBonusAmount.x;
        heldScript.player.RefillMortar(amount);
        bar.SetActive(false);
    }

    void StartQTE()
    {
        bar.SetActive(true);
        Vector3 pos = cursor.transform.localPosition;
        pos.x = cursorStartPos;
        cursor.transform.localPosition = pos;
    }

    bool CursorInSweetSpot()
    {
        bool sucsess = cursor.transform.localPosition.x > bonusStartPos && cursor.transform.localPosition.x < bonusEndPos;
        if (sucsess) AudioManager.instance.PlaySound(8, gameObject);
        return sucsess;
    }

    private void Update()
    {
        if (!enoughMortar) valid = false;
        heldScript.selectedBrick = selectedBrick;
        heldScript.valid = valid;
        sRend.color = valid ? validColor : invalidColor;

        if (bar.activeInHierarchy) {
            cursor.transform.Translate(Vector3.right * cursorSpeed * Time.deltaTime * 10);
            if (cursor.transform.localPosition.x >= cursorEndPos) CompleteQTE(true);
        }
        
    }
}
