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
    public SortedSet<MazeTile> closetTiles;
    private SortedSet<MazeTile> guaranteedHalls;
    private SortedSet<MazeTile> teleporters;

    void Awake() {
        game = GetComponentInParent<MazeGame>();
        tilemap = this.GetComponent<Tilemap>();
        halls = new SortedSet<MazeTile>(new MazeTile.MazeTileComparer());
        guaranteedHalls = new SortedSet<MazeTile>(new MazeTile.MazeTileComparer());
        teleporters = new SortedSet<MazeTile>(new MazeTile.MazeTileComparer());

        if (game.width % 2 == 0 || game.height % 2 == 0)
            throw new System.Exception("Width and height must be odd");
    }

    public void createMap()
    {
        halls.Clear();
        guaranteedHalls.Clear();
        teleporters.Clear();
        tilemap.ClearAllTiles();
        setStartAndEndPos();
        generateMaze();
        createClosetTiles();
        createTeleporters();
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
        if (game.makeMazeImpossible) {
            start = this.tileAt(endPos);
            commitHall(start, remaining);
            next = pickAdjacentTile(start, guaranteedHalls);
            commitHall(next, remaining);
        }

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

        commitHall(tileAt(endPos), remaining);
    }

    private void createClosetTiles() {
        this.closetTiles = getClosetTiles();

        foreach (MazeTile tile in this.closetTiles) {
            MazeTile adj = adjacentHalls(tile).Min;
            tile.type = TileType.Closet;
            tile.pair = adj;
            adj.type = TileType.Closet;
            adj.pair = tile;
        }
    }

    private void createTeleporters() {
        List<MazeTile> validTeleporterTiles = new List<MazeTile>(getValidTeleporterTiles());
        if (validTeleporterTiles.Count < 2)
            return;
        int i;
        List<float> dists = new List<float>(validTeleporterTiles.Count);
        for (int j = 0; j < game.numberOfTeleporters && validTeleporterTiles.Count > 1; j++) {
            i = 0;
            validTeleporterTiles.Shuffle();
            dists.Clear();
            MazeTile teleporter = validTeleporterTiles[0];
            foreach(MazeTile t2 in validTeleporterTiles) {
                dists.Add(Vector2.Distance(teleporter.position, t2.position));
                i++;
            }
            // Pick the one that is furthest away
            MazeTile pair = validTeleporterTiles[dists.IndexOf(Mathf.Max(dists.ToArray()))];
            teleporter.pairTeleporters(pair);
            validTeleporterTiles.Remove(teleporter);
            validTeleporterTiles.Remove(pair);
        }
    }

    private SortedSet<MazeTile> getValidTeleporterTiles() {
        SortedSet<MazeTile> validTeleporters = getLeafTiles();
        validTeleporters.ExceptWith(this.closetTiles);

        return validTeleporters;
    }

    private SortedSet<MazeTile> getClosetTiles() {
        SortedSet<MazeTile> closetTiles = getLeafTiles();
        SortedSet<MazeTile> adjacentClosetTiles = new SortedSet<MazeTile>(new MazeTile.MazeTileComparer());

        // Get all the leaf tiles and remove those that don't have a T intersection 2 tiles away.
        closetTiles.RemoveWhere(t => {
            MazeTile adj1 = adjacentHalls(t).Min;
            SortedSet<MazeTile> adj2 = adjacentHalls(adj1);
            adj2.Remove(t);
            return adjacentHalls(adj2.Min).Count < 3;
        });

        return closetTiles;
    }

    private SortedSet<MazeTile> getLeafTiles() {
        SortedSet<MazeTile> leafTiles = new SortedSet<MazeTile>(new MazeTile.MazeTileComparer());
        leafTiles.UnionWith(guaranteedHalls);
        leafTiles.RemoveWhere(t => adjacentHalls(t).Count > 1);

        return leafTiles;
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
                MazeTile t = CreateTile(type, pos);
                if (type == TileType.Hall)
                    guaranteedHalls.Add(t);

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
        pickSet.IntersectWith(guaranteedHalls);

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

    private SortedSet<MazeTile> adjacentTiles(MazeTile tile) {
        List<Vector2> dirs = new List<Vector2> { new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, -1), new Vector2(-1, 0) };
        List<MazeTile> adjTiles = dirs.ConvertAll<MazeTile>(dir => tileAt(tile.position + dir));
        adjTiles.RemoveAll(t => t == null);

        return new SortedSet<MazeTile>(adjTiles, new MazeTile.MazeTileComparer());
    }

    private SortedSet<MazeTile> adjacentHalls(MazeTile tile) {
        SortedSet<MazeTile> adjTiles = adjacentTiles(tile);
        adjTiles.RemoveWhere(t => t.type != TileType.Hall && t.type != TileType.Closet);

        return adjTiles;
    }

    private int randOddInRange(int max) {
        return 1 + 2*(int)(Random.value * 0.5f*(max - 2));
    }

    private bool isEdge(Vector2 pos) {
        if (pos.x == 0 || pos.x == game.width - 1)
            return true;
        if (pos.y == 0 || pos.y == game.height - 1)
            return true;

        return false;
    }

    private MazeTile CreateTile(TileType type, Vector2 pos) {
        MazeTile t = ScriptableObject.CreateInstance<MazeTile>();
        t.type = type;
        t.position = pos;
        tilemap.SetTile(Vector3Int.RoundToInt(pos), (Tile)t);

        return t;
    }
}
