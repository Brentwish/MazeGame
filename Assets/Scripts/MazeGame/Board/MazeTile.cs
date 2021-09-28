using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileType { Wall, Hall, Closet, Teleporter }

public class MazeTile : Tile
{
    public TileType type;
    public Vector2 position;
    public Color c;
    public MazeTile pair;
    public bool occupied;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        base.GetTileData(position, tilemap, ref tileData);
        tileData.sprite = Resources.Load<Sprite>("Square");

        if (type == TileType.Wall) {
            tileData.color = Color.black;
        } else if (type == TileType.Hall) {
            tileData.color = Color.white;
        } else if (type == TileType.Closet) {
            tileData.color = occupied ? Color.black : Color.white;
        } else if (type == TileType.Teleporter) {
            tileData.color = c;
        }
    }

    public class MazeTileComparer : IComparer<MazeTile> 
    {
        public int Compare(MazeTile t1, MazeTile t2)
        {
            if (t1.position.magnitude == t2.position.magnitude)
                return t1.position.x == t2.position.x ? t1.position.y.CompareTo(t2.position.y) : t1.position.x.CompareTo(t2.position.x);

            return t1.position.magnitude.CompareTo(t2.position.magnitude);
        }
    }

    public void pairTeleporters(MazeTile pair) {
        Color c = new Color(Random.value, Random.value, Random.value);
        this.type = TileType.Teleporter;
        this.c = c;
        this.pair = pair;
        pair.type = TileType.Teleporter;
        pair.c = c;
        pair.pair = this;
    }
}