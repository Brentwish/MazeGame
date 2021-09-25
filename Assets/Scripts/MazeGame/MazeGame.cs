using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum MazeType { Random, Wilsons }

public class MazeGame : MonoBehaviour {
    public int width;
    public int height;
    public GameBoard board;
    public MazeType mazeType;
    private Player mainPlayer;

    void Awake() {
        board = GetComponentInChildren<GameBoard>();
        mainPlayer = GetComponentInChildren<Player>();
    }

    void Start() {
        board.createMap();
        mainPlayer.setPosition(board.startPos);
    }

    public void handleGameEnd() {
        board.createMap();
        mainPlayer.setPosition(board.startPos);
    }
}