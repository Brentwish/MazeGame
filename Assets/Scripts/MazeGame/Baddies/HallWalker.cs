using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallWalker : MonoBehaviour
{
    private MazeGame game;

    void Awake() {
        game = GetComponentInParent<MazeGame>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
