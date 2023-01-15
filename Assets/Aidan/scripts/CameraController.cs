using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    void Awake() { instance = this; }

    public List<PlayerController> players;
    [SerializeField, Range(0, 0.8f)]
    float panSmoothness = 0.025f;
    [SerializeField] Vector2 sizeMinMax;
    [SerializeField] float zoomSpeed = 1;
    [SerializeField] float camYMin;
    Camera cam;
    public bool gameOver;
    
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (gameOver) PanToShowTower();
        if (!gameOver && players.Count > 0)FollowPlayers();   
    }

    void PanToShowTower()
    {
        Vector3 targetPos = transform.position;
        targetPos.y = Mathf.Max(GameManager.instance.towerTop / 2, camYMin);
        transform.position = Vector3.Lerp(transform.position, targetPos, 0.025f);
        Vector3 towerTop = transform.position;
        towerTop.y = GameManager.instance.towerTop + 10;
        if (!InCamBounds(towerTop, 0f)) {
            print("zoom out");
            cam.orthographicSize += zoomSpeed * 200;
        }
    }

    void FollowPlayers()
    {
        Vector3 total = Vector3.zero;
        foreach (var p in players) total += p.heldItemGO.transform.position;
        Vector3 targetPos = total / players.Count;
        targetPos.z = transform.position.z;
        targetPos.y = Mathf.Max(targetPos.y, camYMin);
        transform.position = Vector3.Lerp(transform.position, targetPos, panSmoothness);
        if (players.Count > 1) ZoomToFitPlayers();
    }

    void ZoomToFitPlayers()
    {
        bool allVisible = true;
        foreach (var p in players) {
            if (InCamBounds(p.heldItemGO.transform.position)) {
                cam.orthographicSize += zoomSpeed;
                allVisible = false;
            }
        }
        if (!allVisible) return;
        foreach (var p in players) {
            if (InCamBounds(p.heldItemGO.transform.position, 0.25f)) {
                cam.orthographicSize -= zoomSpeed;
                allVisible = false;
            }
        }
    }

    bool InCamBounds(Vector3 point, float offset = 0)
    {
        point = cam.WorldToViewportPoint(point);
        if (point.x > offset && point.x < 1-offset && point.y > offset && point.y < 1-offset) {
            return true;
        }
        return false;
    }
}
