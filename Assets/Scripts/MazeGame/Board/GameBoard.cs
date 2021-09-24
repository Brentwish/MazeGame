using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameBoard : MonoBehaviour
{
    private MazeGame game;
    public Tilemap tilemap;

    void Awake() {
        game = GetComponentInParent<MazeGame>();
        tilemap = this.GetComponent<Tilemap>();
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
                tilemap.SetTile(new Vector3Int(x, y, 0), t);
            }
        }
    }

    private Tile CreateRandomTile() {
        bool isWall = Random.value > 0.5f;
        MazeTile t = ScriptableObject.CreateInstance<MazeTile>();
        t.type = isWall ? TileType.Wall : TileType.Hall;

        return (Tile)t;
    }
}
