using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MazeGame : MonoBehaviour {
    public int width;
    public int height;
    public GameBoard board;

    void Awake() {
        board = GetComponentInChildren<GameBoard>();
    }

    void Start() {
        board.RenderMap();
    }
}