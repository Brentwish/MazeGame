using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MazeGame : MonoBehaviour {
    public int width;
    public int height;
    private Tilemap board;

    void Awake() {
        board = GetComponentInChildren<Tilemap>();
    }

    void Start() {
        Debug.Log("What is good, my brethren");
        // Instantiate(board, transform.position, Quaternion.identity);
    }

    void Update() {
        
    }
}
