using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileType { Wall, Hall }

public class MazeTile : Tile
{
    public TileType type;
    public Vector2 position;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        base.GetTileData(position, tilemap, ref tileData);
        tileData.sprite = Resources.Load<Sprite>("Square");
        tileData.color = type == TileType.Wall ? Color.black : Color.white;
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