using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameBoard : MonoBehaviour
{
    private MazeGame game;
    public Tilemap tilemap;
    public Vector2 startPos;
    public Vector2 endPos;
    public SortedSet<MazeTile> halls;
    private SortedSet<MazeTile> guaranteedHalls;

    void Awake() {
        game = GetComponentInParent<MazeGame>();
        tilemap = this.GetComponent<Tilemap>();
    }

    public void createMap()
    {
        setStartAndEndPos();
        halls = new SortedSet<MazeTile>(new MazeTile.MazeTileComparer());
        guaranteedHalls = new SortedSet<MazeTile>(new MazeTile.MazeTileComparer());

        switch (game.mazeType)
        {
            case MazeType.Random:
                generateRandomBoard();
                break;

            case MazeType.Wilsons:
                if (game.width % 2 == 0 || game.height % 2 == 0)
                    throw new System.Exception("Width and height must be odd");
                generateMaze();
                break;
            
            default:
                generateRandomBoard();
                break;
        }

        tilemap.RefreshAllTiles();
    }

    private void generateMaze() {
        MazeTile start, next;
        SortedSet<MazeTile> remaining = initBoard();
        List<MazeTile> walk = new List<MazeTile>();

        start = this.tileAt(startPos);
        commitHall(start, remaining);
        next = pickAdjacentTile(start, guaranteedHalls);
        commitHall(next, remaining);

        while(remaining.Count > 0) {
            if (walk.Count == 0) {
                start = pickWalkStart(remaining);
                walk.Add(start);
            }
            next = advanceWalk(walk, remaining);
            if (halls.Contains(next)) {
                walk.Add(next);
                foreach (MazeTile tile in walk)
                    commitHall(tile, remaining);
                walk.Clear();
            } else if (next == null)
                walk.Clear();
            else if (walk.Contains(next))
                walk = walk.GetRange(0, Mathf.Max(0, walk.IndexOf(next) - 1));
            else
                walk.Add(next);
        }

        MazeTile end = tileAt(endPos);
        commitHall(end, remaining);
    }

    private void commitHall(MazeTile tile, SortedSet<MazeTile> remaining) {
        tile.type = TileType.Hall;
        halls.Add(tile);
        remaining.RemoveWhere(t => {
            if (Mathf.Abs(t.position.x - tile.position.x) <= 1 && Mathf.Abs(t.position.y - tile.position.y) <= 1)
                return true;

            return false;
        });
    }

    private MazeTile advanceWalk(List<MazeTile> walk, SortedSet<MazeTile> remaining) {
        MazeTile last = walk[walk.Count - 1];
        List<Vector2> dirs = new List<Vector2>() { new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, -1), new Vector2(-1, 0) };

        for (int i = 4; i > 0; i--) {
            Vector2 tryDir = dirs[(int)(Random.value*i)];
            dirs.Remove(tryDir);
            MazeTile potentialHall = tileAt(last.position + 2*tryDir);
            if (potentialHall != null && guaranteedHalls.Contains(potentialHall)) {
                walk.Add(tileAt(last.position + tryDir));
                return potentialHall;
            }
        }

        return null;
    }

    private SortedSet<MazeTile> initBoard() {
        SortedSet<MazeTile> remaining = new SortedSet<MazeTile>(new MazeTile.MazeTileComparer());

        for (int x = 0; x < this.game.width; x++)
        {
            for (int y = 0; y < this.game.height; y++)
            {
                Vector2 pos = new Vector2(x, y);
                TileType type = x % 2 == 0 || y % 2 == 0 ? TileType.Wall : TileType.Hall;
                MazeTile t = CreateTile(type);
                t.position = pos;
                if (type == TileType.Hall)
                    guaranteedHalls.Add(t);
                tilemap.SetTile(new Vector3Int(x, y, 0), (Tile)t);

                // Exclude edge tiles from initial working set
                if (!isEdge(pos))
                    remaining.Add(t);
            }
        }

        return remaining;
    }

    private MazeTile pickWalkStart(SortedSet<MazeTile> s) {
        SortedSet<MazeTile> pickSet = new SortedSet<MazeTile>(new MazeTile.MazeTileComparer());
        pickSet.UnionWith(s);
        s.IntersectWith(guaranteedHalls);

        return Random.value > 0.5f ? pickSet.Min : pickSet.Max;
    }

    private MazeTile tileAt(Vector2 pos) {
        return this.game.board.tilemap.GetTile<MazeTile>(Vector3Int.RoundToInt(pos)); 
    }

    private void setStartAndEndPos() {
        if (Random.value > 0.5f) {
            if (Random.value > 0.5f) {
                startPos = new Vector2(0, randOddInRange(this.game.height));
                endPos = new Vector2(this.game.width - 1, randOddInRange(this.game.height));
            } else {
                startPos = new Vector2(this.game.width - 1, randOddInRange(this.game.height));
                endPos = new Vector2(0, randOddInRange(this.game.height));
            }
        } else {
            if (Random.value > 0.5f) {
                startPos = new Vector2(randOddInRange(this.game.width), 0);
                endPos = new Vector2(randOddInRange(this.game.width), this.game.height - 1);
            } else {
                startPos = new Vector2(randOddInRange(this.game.width), this.game.height - 1);
                endPos = new Vector2(randOddInRange(this.game.width), 0);
            }
        }
    }

    private MazeTile pickAdjacentTile(MazeTile t, SortedSet<MazeTile> s) {
        List<Vector2> attempts = new List<Vector2>() { new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, -1), new Vector2(-1, 0) };
        for (int i = 4; i > 0; i--)
        {
            int wat = (int)(Random.value * i);
            Vector2 dir = attempts[wat];
            attempts.RemoveAt(wat);
            MazeTile adjacentTile = tileAt(t.position + dir);

            if (adjacentTile != null && s.Contains(adjacentTile))
                return adjacentTile;
        }

        return null;
    }

    private int randOddInRange(int max) {
        return 1 + 2*(int)(Random.value * 0.5f*(max - 2));
    }

    private void generateRandomBoard() {
        for (int x = 0; x < this.game.width; x++)
        {
            for (int y = 0; y < this.game.height; y++)
            {
                TileType type;
                if (isEdge(new Vector2(x, y)))
                    type = TileType.Wall;
                else {
                    bool isWall = Random.value > 0.5f;
                    type = isWall ? TileType.Wall : TileType.Hall;
                }
                MazeTile t = CreateTile(type);
                tilemap.SetTile(new Vector3Int(x, y, 0), (Tile)t);
            }
        }
    }

    private bool isEdge(Vector2 pos) {
        if (pos.x == 0 || pos.x == game.width - 1)
            return true;
        if (pos.y == 0 || pos.y == game.height - 1)
            return true;

        return false;
    }

    private MazeTile CreateTile(TileType type) {
        MazeTile t = ScriptableObject.CreateInstance<MazeTile>();
        t.type = type;

        return t;
    }
}
