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
    
    void Start()
    {
        game = GetComponentInParent<MazeGame>();
        Vector3Int startTilePos = game.board.tilemap.WorldToCell(new Vector3(1, 0, 0));
        transform.position = game.board.tilemap.GetCellCenterWorld(startTilePos);
        movePoint.position = transform.position;
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

        if (Vector2.Distance(transform.position, movePoint.position) < 0.05f)
        {
            if (Mathf.Abs(horizontal) == 1f || Mathf.Abs(vertical) == 1f) 
            {
                Vector3 dir = new Vector3(horizontal, vertical, 0f);

                if (canMoveInDir(dir))
                    movePoint.position += dir;
            }
        }
    }
    
    private bool canMoveInDir(Vector3 dir) {
        Vector3Int pos = game.board.tilemap.WorldToCell(movePoint.position + dir);
        MazeTile t = game.board.tilemap.GetTile<MazeTile>(pos);

        if (t == null)
            return false;
        if (t.type == TileType.Wall)
            return false;

        return true;
    }
}
