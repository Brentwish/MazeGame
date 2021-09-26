using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileType { Wall, Hall, Closet, Teleporter }

public class MazeTile : Tile
{
    public TileType type;
    public Vector2 position;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        base.GetTileData(position, tilemap, ref tileData);
        tileData.sprite = Resources.Load<Sprite>("Square");

        if (type == TileType.Wall) {
            tileData.color = Color.black;
        } else if (type == TileType.Hall) {
            tileData.color = Color.white;
        } else if (type == TileType.Closet) {
            tileData.color = Color.grey;
        } else if (type == TileType.Teleporter) {
            tileData.color = Color.cyan;
        }
    }

    public class MazeTileComparer : IComparer<MazeTile> 
    {
        public int Compare(MazeTile t1, MazeTile t2)
        {
            if (t1.position.x == t2.position.x)
                return t1.position.y.CompareTo(t2.position.y);

            return t1.position.x.CompareTo(t2.position.x);
        }
    }
}