using System.Collections.Generic;
using UnityEngine;

public class BubbleGrid : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public GameObject BubblePrefab;
    public Bubble[,] grid;
    private HashSet<Bubble> matchesFound = new HashSet<Bubble>();
    private int shotCount = 0;
    public static int shiftcount = 0;
    private void Start()
    {
        grid = new Bubble[width, height];
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = height - 1; y > height - 5; y--)
            {
                var b = (y % 2 == 0) ? 0.5f : 0;
                InitializedGrid(x, y, b);
            }
        }
    }

    void InitializedGrid(int x, int y, float b)
    {
        grid[x, y] = null;
        var position = new Vector3(x+b, y * 0.93f, 0);
        var obj = Instantiate(BubblePrefab, position, Quaternion.identity, transform);
        Bubble bubble = obj.GetComponent<Bubble>();
        bubble.SetType((BubbleType)Random.Range(1, 5));
        bubble.x = x;
        bubble.y = y;
        bubble.b = b;
        bubble.startCondition = true;
        grid[x, y] = bubble;
    }

    public void CheckMatch(Bubble bubble)
    {
        if (bubble == null) return;
        matchesFound.Clear();
        
        FindMatches(bubble);
        if (matchesFound.Count >= 3)
        {

            RemoveMatches();
            ProcessFalling();
        }
    }

    public void FindMatches(Bubble bubble)
    { 
        var queue= new Queue<Bubble>();
        queue.Enqueue(bubble);
        matchesFound.Add(bubble);
        while (queue.Count > 0)
        { 
            var current = queue.Dequeue();
            var neighbours = GetNeighbours(current);
            foreach (var neighbor in neighbours)
            {
                if (neighbor.type == bubble.type && matchesFound.Contains(neighbor) == false)
                {
                    matchesFound.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }
    }

    public List<Bubble> GetNeighbours(Bubble startBubble)
    {
        var offset = (startBubble.b == 0.5f) ? 0 : -1;
        var dx = new[] {-1, 1, 0+offset, 1+offset, 0+offset, 1+offset};
        var dy = new[] { 0, 0, -1, -1, 1, 1 };
        var neighbour = new List<Bubble>();
        for (int i = 0; i < dx.Length; i++)
        {
            var nx = dx[i] + startBubble.x;
            var ny = dy[i] + startBubble.y;

            if (nx >= 0 && nx < width && ny >= 0 && ny < height)
            { 
                if (grid[nx, ny] !=null) neighbour.Add(grid[nx, ny]);
            }
        }
        return neighbour;
    }

    void RemoveMatches()
    {
        foreach (var b in matchesFound)
        {
            grid[b.x, b.y] = null;
            Destroy(b.gameObject);
        }
    }

    void ProcessFalling()
    {
        var connectedToCeil = new HashSet<Bubble>();
        var queue = new Queue<Bubble>();
        for (int x = 0; x < width; x++)
        {
            if (grid[x, height - 1] != null)
            { 
                var topBubble = grid[x, height - 1];
                connectedToCeil.Add(topBubble);
                queue.Enqueue(topBubble);
            }
        }

        while (queue.Count > 0)
        { 
            var bubble = queue.Dequeue();
            var neighbours = GetNeighbours(bubble);
            foreach (var neighbor in neighbours)
            {
                if (connectedToCeil.Contains(neighbor) == false)
                {
                    connectedToCeil.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }
        var toFall = new List<Bubble>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            { 
                var b = grid[x, y];
                if (connectedToCeil.Contains(b) == false && b!=null)
                { 
                    toFall.Add(b);
                }
            }
        }
        foreach (var b in toFall)
        {
            grid[b.x, b.y] = null;
            StartCoroutine(Bubble.FallToPosition(b.transform, new Vector3(b.x + b.b, 0, 0), 1));
        }
    }

    public void RegisterTurn()
    {
        shotCount++;
        if (shotCount >= 5)
        {
            shotCount = 0;
            shiftcount += 1;
            ShiftGridDown();
        }
    }

    void ShiftGridDown()
    {
        for (int x = 0; x < width; x++)
        {
            if (grid[x, 0] != null)
            {
                Debug.Log("Game Over! Пузыри достигли нижнего края.");
                return;
            }
        }

        for (int y = 1; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var b = grid[x, y];
                if (b != null)
                {
                    StartCoroutine(Bubble.FallToPosition(b.transform, new Vector3(b.x + b.b, (b.y-1) * 0.93f, 0), 0));

                    b.y = y - 1;

                    grid[x, y - 1] = b;
                    grid[x, y] = null;
                }
            }
        }

        InitializedHighRow();
    }

    void InitializedHighRow()
    {
        var y = height - 1;
        var b = ((y+shiftcount) % 2 == 0) ? 0.5f : 0;
        for (int x = 0; x < width; x++)
        {
            InitializedGrid(x, y, b);
        }
    }
}



