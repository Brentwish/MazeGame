using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    private Transform playerTransform;

    void Awake() {
        playerTransform = GetComponentInParent<Transform>();
        SetTransformToPlayers();
    }

    void FixedUpdate()
    {
        SetTransformToPlayers();
    }

    void SetTransformToPlayers() {
        float cameraX = playerTransform.position.x;
        float cameraY = playerTransform.position.y;
        float cameraZ = this.transform.position.z;

        this.transform.position = new Vector3(cameraX, cameraY, cameraZ);
    }
}
