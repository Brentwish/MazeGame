using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameBoard : MonoBehaviour
{
    public Tile mazeTile;
    private Tilemap board;
    private MazeGame game;

    void Awake() {
        game = GetComponentInParent<MazeGame>();
        board = this.GetComponent<Tilemap>();
    }

    void Start()
    {
    }

    void Update()
    {
    }

    public void RenderMap()
    {
        for (int x = 0; x < this.game.width; x++)
        {
            for (int y = 0; y < this.game.height; y++)
            {
                Tile t = CreateRandomTile();
                this.board.SetTile(new Vector3Int(x, y, 0), t);
            }
        }
    }

    private Tile CreateRandomTile() {
        Tile t = Instantiate(mazeTile);
        bool isWall = Random.value > 0.5f;
        t.color = isWall ? Color.black : Color.white;
        t.colliderType = isWall ? Tile.ColliderType.Grid : Tile.ColliderType.None;

        return t;
    }
}
