using UnityEngine;

public class CameraFollow : MonoBehaviour {
    public Transform playerTransform;
    public int depth = -10;

    private Vector3 basePosition;

    void Start() {
        basePosition = transform.position;
    }

    void Update() {
        if (playerTransform != null) {
            transform.position = playerTransform.position + basePosition;
        }
    }

    public void SetTarget(Transform target) {
        playerTransform = target;
    }
}