using UnityEngine;
using System.Collections.Generic;

public class Match3Grid : MonoBehaviour
{
    public int width = 6;
    public int height = 6;
    public float spacing = 1.05f; 
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
                Vector2 position = new Vector2(x * spacing, y * spacing);
                GameObject obj =Instantiate(Mathc3Prefab, position, Quaternion.identity);
                Bubble bubble = obj.GetComponent<Bubble>();
                bubble.SetType(GetRandomType());
                grid[x, y] = bubble;  
            }
        }
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
        mathces.AddRange(CheckDirection(x, y, 1, 0));
        mathces.AddRange(CheckDirection(x, y, -1, 0));
        if (mathces.Count >= 2)
        { 
            mathces.Add(center);
            RemoveMatches(mathces);
            return;
        }
        mathces.Clear();
        mathces.AddRange(CheckDirection(x, y, 0, 1));
        mathces.AddRange(CheckDirection(x, y, 0, -1));
        if (mathces.Count >= 2)
        {
            mathces.Add(center);
            RemoveMatches(mathces);
            return;
        }
    }
    List<Bubble> CheckDirection(int startX, int startY, int dx, int dy)
    { 
        var result = new List<Bubble>();
        BubbleType cageType = grid[startX, startY].type;

        var x = startX + dx;
        var y = startY + dy;
        while (x > 0 && x < width && y > 0 && y < height)
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
            var x = Mathf.RoundToInt(pos.x / spacing);
            var y = Mathf.RoundToInt(pos.y / spacing);
            grid[x, y] = null;
            Destroy(b.gameObject);
        }
    }
}
