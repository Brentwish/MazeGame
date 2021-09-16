using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGame : MonoBehaviour {
    public GameObject board;
    public int width;
    public int height;

    void Start() {
        Debug.Log("What is good, my brethren");
        Instantiate(board, transform.position, Quaternion.identity);
    }

    void Update() {
        
    }
}
