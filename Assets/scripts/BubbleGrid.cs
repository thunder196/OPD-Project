using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleGrid : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public GameObject bubblePrefab;
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
                InitializedGrid(x, y);
            }
        }
    }

    void InitializedGrid(int x, int y)
    {
        var position = new Vector3(x, y, 0);
        var obj = Instantiate(bubblePrefab, position, Quaternion.identity);
        Bubble bubble = obj.GetComponent<Bubble>();
        bubble.SetType(GetRandomType());
        bubble.x = x;
        bubble.y = y;
        grid[x, y] = bubble;
    }

    BubbleType GetRandomType()
    {
        return (BubbleType)Random.Range(1, 5);
    }

    public void CheckMatch(Bubble startBubble)
    {
        if (startBubble == null) return;

        matchesFound.Clear();
        FindSameColor(startBubble);

        if (matchesFound.Count >= 3)
        {
            RemoveMatches();
            ProcessFallingBubbles();
        }
    }

    void FindSameColor(Bubble bubble)
    {
        var queue = new Queue<Bubble>();
        queue.Enqueue(bubble);
        matchesFound.Add(bubble);

        while (queue.Count > 0)
        {
            Bubble current = queue.Dequeue();
            foreach (Bubble neighbor in GetNeighbors(current))
            {
                if (neighbor.type == bubble.type && !matchesFound.Contains(neighbor))
                {
                    matchesFound.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }
    }

    void RemoveMatches()
    {
        foreach (Bubble b in matchesFound)
        {
            grid[b.x, b.y] = null;
            Destroy(b.gameObject);
        }
    }


    public void ProcessFallingBubbles()
    {
        var connectedToCeiling = new HashSet<Bubble>();
        var queue = new Queue<Bubble>();
        for (int x = 0; x < width; x++)
        {
            if (grid[x, height - 1] != null)
            {
                Bubble topBubble = grid[x, height - 1];
                connectedToCeiling.Add(topBubble);
                queue.Enqueue(topBubble);
            }
        }

        while (queue.Count > 0)
        {
            Bubble current = queue.Dequeue();
            foreach (Bubble neighbor in GetNeighbors(current))
            {
                if (!connectedToCeiling.Contains(neighbor))
                {
                    connectedToCeiling.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }
        var toFall = new List<Bubble>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Bubble b = grid[x, y];
                if (b != null && !connectedToCeiling.Contains(b))
                {
                    toFall.Add(b);
                    grid[x, y] = null;
                }
            }
        }

        foreach (Bubble b in toFall)
        {
            StartCoroutine(Bubble.FallToPosition(b.transform, new Vector3(b.x, 0, 0)));
        }
    }



    List<Bubble> GetNeighbors(Bubble b)
    {
        var neighbors = new List<Bubble>();
        int[] dx = { 0, 0, 1, -1 };
        int[] dy = { 1, -1, 0, 0 };

        for (int i = 0; i < 4; i++)
        {
            int nx = b.x + dx[i];
            int ny = b.y + dy[i];

            if (nx >= 0 && nx < width && ny >= 0 && ny < height)
            {
                if (grid[nx, ny] != null)
                    neighbors.Add(grid[nx, ny]);
            }
        }
        return neighbors;
    }
}



