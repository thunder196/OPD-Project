using UnityEngine;
using System.Collections.Generic;

public class Match3Grid : MonoBehaviour
{
    public int width = 6;
    public int height = 6;
    public GameObject Mathc3Prefab;
    private Bubble[,] grid;
    void Start()
    {
        grid = new Bubble[width, height];
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                InitializedGrid(x, y);
            }
        }
    }

    void InitializedGrid(int x, int y)
    {
        Vector2 position = new Vector2(x, y);
        GameObject obj = Instantiate(Mathc3Prefab, position, Quaternion.identity);
        Bubble bubble = obj.GetComponent<Bubble>();
        bubble.SetType(GetRandomType());
        grid[x, y] = bubble;
    }

    BubbleType GetRandomType()
    { 
        int random = Random.Range(1, 5);
        return (BubbleType)random;
    }


    public void CheckMatch(int x, int y)
    { 
        if (grid[x, y] == null) return;
        Bubble center = grid[x, y];
        var mathces = new List<Bubble>();
        var removeCenter=false;
        mathces.AddRange(CheckDirection(x, y, 1, 0));
        mathces.AddRange(CheckDirection(x, y, -1, 0));
        if (mathces.Count >= 2)
        {
            mathces.Add(center);
            removeCenter=true;  
        }
        else
        {
            mathces.Clear();
        }
        var deltaRange = mathces.Count;
        mathces.AddRange(CheckDirection(x, y, 0, 1));
        mathces.AddRange(CheckDirection(x, y, 0, -1));
        if (mathces.Count - deltaRange >= 2)
        {
            if (!removeCenter)
                mathces.Add(center);
        }
        RemoveMatches(mathces);
    }
    List<Bubble> CheckDirection(int startX, int startY, int dx, int dy)
    { 
        var result = new List<Bubble>();
        BubbleType cageType = grid[startX, startY].type;

        var x = startX + dx;
        var y = startY + dy;
        while (x >= 0 && x < width && y >= 0 && y < height)
        {
            if (grid[x, y] != null && grid[x, y].type == cageType)
            {
                result.Add(grid[x, y]);
                x += dx;
                y += dy;
                continue;
            }
            else
            {
                break;
            }
        }
        return result;
    }

    void RemoveMatches(List<Bubble> mathces)
    {
        foreach (Bubble b in mathces)
        { 
            Vector2 pos = b.transform.position;
            var x = Mathf.RoundToInt(pos.x);
            var y = Mathf.RoundToInt(pos.y);
            grid[x, y] = null;
            Destroy(b.gameObject);
        }
        CollapseGrid();
    }

    void CollapseGrid()
    {
        for (int x = 0; x < width; x++)
            CollapseColumn(x);
        CheckOnEmpty();
    }

    void CollapseColumn(int x)
    {
        bool moved = true;

        while (moved)
        {
            moved = false;

            for (int y = 0; y < height - 1; y++)
            {
                if (grid[x, y] == null && grid[x, y + 1] != null)
                {
                    grid[x, y] = grid[x, y + 1];
                    grid[x, y + 1] = null;

                    MoveBubble(grid[x, y], x, y);

                    moved = true;
                }
            }
        }
    }

    void CheckOnEmpty()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == null)
                    { InitializedGrid(x, y); }
            }
        }
    }

    void MoveBubble(Bubble bubble, int x, int y)
    { 
        Vector2 newPosition= new Vector2(x, y);
        bubble.transform.position = newPosition;
    }

}
