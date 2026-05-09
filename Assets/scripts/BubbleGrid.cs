using System.Collections.Generic;
using UnityEngine;

public class BubbleGrid : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public GameObject BubblePrefab;
    public Bubble[,] grid;
    private HashSet<Bubble> matchesFound = new HashSet<Bubble>();

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
                InitializedGrid(x, y, x + b);
            }
        }
    }

    void InitializedGrid(int x, int y, float realX)
    {
        var position = new Vector3(realX, y * 0.93f, 0);
        var obj = Instantiate(BubblePrefab, position, Quaternion.identity);
        Bubble bubble = obj.GetComponent<Bubble>();
        bubble.SetType((BubbleType)Random.Range(1, 5));
        bubble.x = realX;
        bubble.y = y * 0.93f;
        bubble.b = realX - x;
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
        var offset = ((int)startBubble.y % 2 == 0) ? 0 : -1;
        var dx = new[] {-1, 1, 0+offset, 1+offset, 0+offset, 1+offset};
        var dy = new[] { 0, 0, -1, -1, 1, 1 };
        var neighbour = new List<Bubble>();
        for (int i = 0; i < dx.Length; i++)
        {
            var nx = dx[i] + (int)startBubble.x;
            var ny = dy[i] + (int)startBubble.y;

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
            grid[(int)b.x, (int)b.y] = null;
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
            Bubble bubble = queue.Dequeue();
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
        Debug.Log($"Должно быть удалено {toFall.Count} объектов");
        foreach (var b in toFall)
        {
            grid[(int)b.x, (int)b.y] = null;
            StartCoroutine(Bubble.FallToPosition(b.transform, new Vector3(b.x + b.b, 0, 0)));
        }
    }

}



