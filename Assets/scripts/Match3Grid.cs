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
        Vector3 spawnPos = new Vector3(x, height+1, 0);
        Vector3 position = new Vector3(x, y, 0);
        GameObject obj = Instantiate(Mathc3Prefab, spawnPos, Quaternion.identity);
        Bubble bubble = obj.GetComponent<Bubble>();
        bubble.SetType(GetRandomType());
        grid[x, y] = bubble;
        StartCoroutine(Bubble.FallToPosition(obj.transform, position));
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
        var matchesx = new List<Bubble>();
        var matchesy = new List<Bubble>();
        matchesx.AddRange(CheckDirection(x, y, 1, 0));
        matchesx.AddRange(CheckDirection(x, y, -1, 0));
        matchesy.AddRange(CheckDirection(x, y, 0, 1));
        matchesy.AddRange(CheckDirection(x, y, 0, -1));
        if (matchesx.Count >= 2)
        {
            matchesx.Add(center);
            if (matchesy.Count >= 2)
            {
                matchesx.AddRange(matchesy);
            }
            RemoveMatches(matchesx);
            return;

        }
        if (matchesy.Count >= 2) 
        { 
            matchesy.Add(center); 
            RemoveMatches(matchesy);
            return;
        }
        return;
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
        CollapseColumn();
    }

    void CollapseColumn()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == null)
                {
                    for (int aboveY = y + 1; aboveY < height; aboveY++)
                    {
                        if (grid[x, aboveY] != null)
                        {
                            var fallingPiece = grid[x, aboveY];
                            grid[x, y] = fallingPiece;
                            grid[x, aboveY] = null;

                            Vector3 targetPos = new Vector3(x, y, 0);

                            StartCoroutine(Bubble.FallToPosition(fallingPiece.transform, targetPos, 0.2f));

                            break;
                        }
                    }
                }
            }
        }
        CheckOnEmpty();
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


}
