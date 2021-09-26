using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum MazeType { Random, Wilsons }

public class MazeGame : MonoBehaviour {
    public int width;
    public int height;
    public int numberOfHallWalkers;
    public GameBoard board;
    public MazeType mazeType;
    private Player mainPlayer;
    private GameObject[] hallWalkers;
    public GameObject hallWalkerPrefab;

    void Awake() {
        board = GetComponentInChildren<GameBoard>();
        mainPlayer = GetComponentInChildren<Player>();
        hallWalkers = new GameObject[numberOfHallWalkers];
    }

    void Start() {
        board.createMap();
        mainPlayer.setPosition(board.startPos);
        initHallWalkers();
    }

    public void handleGameEnd() {
        board.createMap();
        mainPlayer.setPosition(board.startPos);
        initHallWalkers();
    }

    private void initHallWalkers() {
        if (hallWalkers.Length > 0) {
            foreach (GameObject walker in hallWalkers)
                 Destroy(walker);
            hallWalkers = new GameObject[numberOfHallWalkers];
        }
        for (int i = 0; i < numberOfHallWalkers; i++) {
            MazeTile[] halls = new MazeTile[board.halls.Count];
            board.halls.CopyTo(halls);
            Vector3Int tilePos = board.tilemap.WorldToCell(halls[(int)(board.halls.Count * Random.value)].position);
            Vector3 walkerPos = board.tilemap.GetCellCenterWorld(tilePos);
            GameObject hallWalker = Instantiate(hallWalkerPrefab, walkerPos, Quaternion.identity);
            hallWalker.name = "HallWalker" + (i + 1);
            hallWalkers[i] = hallWalker;
        }
    }
}