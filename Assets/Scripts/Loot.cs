using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour {

    public float lootRange = 2.0f;

    private Color startcolor;
    private Renderer rendererComp;
    private Transform playerTransform;

    private bool isInit = false;

    private void Awake() {
        rendererComp = GetComponent<Renderer>();
    }

    private void Update() {
        if (playerTransform == null && Camera.main.GetComponent<CameraFollow>().playerTransform != null) {
            isInit = true;
            playerTransform = Camera.main.GetComponent<CameraFollow>().playerTransform;
        }
        
        if (Input.GetButtonDown("Fire2"))
            GetLooted();
    }

    private void OnMouseEnter() {
        startcolor = rendererComp.material.color;
        rendererComp.material.color = Color.yellow;
    }

    private void OnMouseExit() {
        rendererComp.material.color = startcolor;
    }

    private void GetLooted() {
        if (isInit) {
            Ray clickPoint = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitPoint;

            if (Physics.Raycast(clickPoint, out hitPoint)) {
                if (hitPoint.collider == GetComponent<Collider>()) {
                    float dist = Vector3.Distance(playerTransform.position, transform.position);

                    if (dist < lootRange)
                        Destroy(gameObject);

                    Debug.Log(dist);
                }
            }
        }
    }
}
