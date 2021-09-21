using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameBoard : MonoBehaviour
{
    public Tile mazeTile;
    public int width;
    public int height;
    private int[,] mazeBitMap;

    void Start()
    {
        this.initializeMazeBitMap();
        this.RenderMap();
        Tilemap t = this.GetComponent<Tilemap>();
        Debug.Log(t.size.ToString());
    }

    void Update()
    {
    }

    void initializeMazeBitMap()
    {
        Debug.Log("Init mazeBitMask");
        mazeBitMap = new int[this.width, this.height];
        for (int x = 0; x < this.width; x++)
        {
            for (int y = 0; y < this.height; y++)
            {
                if (Random.value > 0.5f)
                {
                    this.mazeBitMap[x, y] = 0;
                }
                else
                {
                    this.mazeBitMap[x, y] = 1;
                }
            }
        }
    }

    void RenderMap()
    {
        Tilemap tileMap = this.GetComponent<Tilemap>();
        tileMap.ClearAllTiles();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tileMap.SetTile(new Vector3Int(x, y, 0), mazeTile);
                if (Random.value > 0.5f)
                {
                    tileMap.SetColor(new Vector3Int(x, y, 0), Color.black);
                }
                else
                {
                    tileMap.SetColor(new Vector3Int(x, y, 0), Color.white);
                }
            }
        }

        tileMap.RefreshAllTiles();
    }
}
