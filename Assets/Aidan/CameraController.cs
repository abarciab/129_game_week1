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
    Camera cam;
    
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (players.Count > 0)FollowPlayers();   
    }

    void FollowPlayers()
    {
        Vector3 total = Vector3.zero;
        foreach (var p in players) total += p.heldItemRB.transform.position;
        Vector3 targetPos = total / players.Count;
        targetPos.z = transform.position.z;
        transform.position = Vector3.Lerp(transform.position, targetPos, panSmoothness);
        if (players.Count > 1) ZoomToFitPlayers();
    }

    void ZoomToFitPlayers()
    {
        bool allVisible = true;
        foreach (var p in players) {
            var viewPos = cam.WorldToViewportPoint(p.heldItemRB.transform.position);
            if (cam.orthographicSize < sizeMinMax.y && (viewPos.x < 0 || viewPos.x > 1 || viewPos.y < 0 || viewPos.y > 1)) {
                cam.orthographicSize += zoomSpeed;
                allVisible = false;
            }
        }
        if (!allVisible) return;
        foreach (var p in players) {
            var viewPos = cam.WorldToViewportPoint(p.heldItemRB.transform.position);
            if (cam.orthographicSize > sizeMinMax.x && viewPos.x >= 0.25f && viewPos.x < 0.75 && viewPos.y > 0.25f && viewPos.y < 0.75f) {
                cam.orthographicSize -= zoomSpeed;
                allVisible = false;
            }
        }
    }
}
