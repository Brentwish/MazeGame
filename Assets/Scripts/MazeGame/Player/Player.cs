using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    public float speed;
    public Transform movePoint;
    private float horizontal; 
    private float vertical;
    private MazeGame game;
    
    void Start() {
        game = GetComponentInParent<MazeGame>();
    }

    void Update() {
        horizontal = 0f;
        vertical = 0f;

        if (Input.GetKey("a")) horizontal = -1f;
        if (Input.GetKey("d")) horizontal = 1f;
        if (Input.GetKey("s")) vertical = -1f;
        if (Input.GetKey("w")) vertical = 1f;
    }

    void FixedUpdate() {
        Vector3 temp = movePoint.position;
        transform.position = Vector2.MoveTowards(transform.position, movePoint.position, speed * Time.deltaTime);
        movePoint.position = temp;

        Vector3Int pos = game.board.tilemap.WorldToCell(transform.position);
        MazeTile t = game.board.tilemap.GetTile<MazeTile>(pos);

        if (Vector2.Distance(t.position, game.board.endPos) < 0.05f) {
            game.handleGameEnd();
        }

        if (Vector2.Distance(transform.position, movePoint.position) < 0.05f)
        {
            if (Mathf.Abs(horizontal) == 1f || Mathf.Abs(vertical) == 1f) 
            {
                Vector3 dir = new Vector3(horizontal, vertical, 0f);
                Vector3Int newPos = game.board.tilemap.WorldToCell(movePoint.position + dir);
                MazeTile newT = game.board.tilemap.GetTile<MazeTile>(newPos);

                if (newT != null) {
                    switch (newT.type) {
                        case TileType.Teleporter:
                            setPosition(newT.pair.position);
                            break;
                        case TileType.Closet:
                            if (t.type == TileType.Closet) {
                                setPosition(newT.position, false);
                                newT.occupied = false;
                            }
                            else {
                                setPosition(newT.pair.position, false);
                                newT.occupied = true;
                            }
                            break;
                        case TileType.Wall:
                            break;
                        case TileType.Hall:
                            movePoint.position += dir;
                            break;
                    }
                }
            }
        }

        game.board.tilemap.RefreshAllTiles();
    }

    public void setPosition(Vector2 pos, bool instantly = true) {
        Vector3Int tilePos = game.board.tilemap.WorldToCell(pos);
        Vector2 tileCenter = game.board.tilemap.GetCellCenterWorld(tilePos);
        if (instantly)
            transform.position = tileCenter;
        movePoint.position = tileCenter;
    }
}
