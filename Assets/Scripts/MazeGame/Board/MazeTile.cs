using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MazeTile : TileBase
{
    public string type;
    public Sprite sprite;

    void Start()
    {
        Debug.Log("New tile: ", this);
    }

    void Update()
    {

    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite = sprite;
    }
}
