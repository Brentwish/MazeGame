using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MazeTile : TileBase
{
    public string type;
    public Sprite[] mazeTiles;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("New tile: ", this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("Collision with " + other.gameObject);
    }
}
