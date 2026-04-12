using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Match3Grid : MonoBehaviour
{
    public int width = 6;
    public int height = 6;
    public GameObject Match3Prefab; 
    private Bubble[,] grid;
    private Bubble firstSelected;
    void Start()
    {
        grid = new Bubble[width, height];
        GenerateValidBoard();
    }

    void GenerateValidBoard()
    {
        GenerateGrid();

        while (HasAnyMatchesOnBoard())
        {
            ReshuffleTypes();
        }
    }

    void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] != null) Destroy(grid[x, y].gameObject);
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                InitializedGrid(x, y, false);
            }
        }
    }

    void InitializedGrid(int x, int y, bool animate)
    {
        Vector3 finalPos = new Vector3(x, y, 0);
        var spawnPos = new Vector3(x, y, 0);
        if (animate)
        {
            spawnPos = new Vector3(x, height + y + 1, 0);
        }
        GameObject obj = Instantiate(Match3Prefab, spawnPos, Quaternion.identity, transform);
        Bubble bubble = obj.GetComponent<Bubble>();
        bubble.SetType(GetRandomType());
        grid[x, y] = bubble;

        if (animate)
        {
            StartCoroutine(Bubble.FallToPosition(obj.transform, finalPos));
        }
    }

    void ReshuffleTypes()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y].SetType(GetRandomType());
            }
        }
    }

    BubbleType GetRandomType()
    {
        return (BubbleType)Random.Range(1, 5);
    }

    bool HasAnyMatchesOnBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (CheckAnyMatch(x, y)) return true;
            }
        }
        return false;
    }

    bool CheckAnyMatch(int x, int y)
    {
        if (grid[x, y] == null) return false;
        var matchx = 1 + CountInDirection(x, y, 1, 0) + CountInDirection(x, y, -1, 0);
        var matchy = 1 + CountInDirection(x, y, 0, 1) + CountInDirection(x, y, 0, -1);
        return matchx >= 3 || matchy >= 3;
    }

    int CountInDirection(int x, int y, int dx, int dy)
    {
        var type = grid[x, y].type;
        var count = 0;
        int nx = x + dx;
        int ny = y + dy;

        while (nx >= 0 && nx < width && ny >= 0 && ny < height && grid[nx, ny] != null)
        {
            if (grid[nx, ny].type == type)
            {
                count++;
                nx += dx;
                ny += dy;
            }
            else break;
        }
        return count;
    }

    public void SelectBubble(Bubble bubble)
    {
        if (firstSelected == null)
        {
            firstSelected = bubble;
            bubble.transform.localScale = Vector3.one * 1.2f;
        }
        else
        {
            if (firstSelected == bubble)
            {
                firstSelected.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
                firstSelected = null;
            }

            if (IsAdjacent(firstSelected, bubble))
            {
                StartCoroutine(SwapAndCheck(firstSelected, bubble));
            }
            firstSelected.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
            firstSelected = null;
        }
    }

    bool IsAdjacent(Bubble b1, Bubble b2)
    {
        var vector1 = b1.transform.position;
        var vector2 = b2.transform.position;
        return Vector2.Distance(vector1, vector2) <= 1.1f;
    }

    IEnumerator SwapAndCheck(Bubble b1, Bubble b2)
    {
        var x1 = Mathf.FloorToInt(b1.transform.position.x);
        var y1 = Mathf.FloorToInt(b1.transform.position.y);
        var x2 = Mathf.FloorToInt(b2.transform.position.x);
        var y2 = Mathf.FloorToInt(b2.transform.position.y);
        grid[x1, y1] = b2;
        grid[x2, y2] = b1;

        StartCoroutine(Bubble.FallToPosition(b1.transform, new Vector3(x2, y2, 0)));
        yield return StartCoroutine(Bubble.FallToPosition(b2.transform, new Vector3(x1, y1, 0)));

        var match1 = CheckAnyMatch(x1, y1);
        var match2 = CheckAnyMatch(x2, y2);

        if (match1 || match2)
        {
            if (match1) CheckMatch(x1, y1);
            if (match2) CheckMatch(x2, y2);
        }
        else
        {
            grid[x1, y1] = b1;
            grid[x2, y2] = b2;
            StartCoroutine(Bubble.FallToPosition(b1.transform, new Vector3(x1, y1, 0)));
            yield return StartCoroutine(Bubble.FallToPosition(b2.transform, new Vector3(x2, y2, 0)));
        }
    }

    public void CheckMatch(int x, int y)
    {
        if (grid[x, y] == null) return;

        Bubble center = grid[x, y];
        var matchesX = CheckDirection(x, y, 1, 0);
        matchesX.AddRange(CheckDirection(x, y, -1, 0));

        var matchesY = CheckDirection(x, y, 0, 1);
        matchesY.AddRange(CheckDirection(x, y, 0, -1));

        var toRemove = new List<Bubble>();

        if (matchesX.Count >= 2) toRemove.AddRange(matchesX);
        if (matchesY.Count >= 2) toRemove.AddRange(matchesY);

        if (toRemove.Count > 0)
        {
            toRemove.Add(center);
            RemoveMatches(toRemove);
        }
    }

    

    List<Bubble> CheckDirection(int startX, int startY, int dx, int dy)
    {
        var result = new List<Bubble>();
        var cageType = grid[startX, startY].type;

        var x = startX + dx;
        var y = startY + dy;

        while (x >= 0 && x < width && y >= 0 && y < height && grid[x, y] != null)
        {
            if (grid[x, y].type == cageType)
            {
                result.Add(grid[x, y]);
                x += dx;
                y += dy;
            }
            else break;
        }
        return result;
    }

    void RemoveMatches(List<Bubble> matches)
    {
        foreach (Bubble b in matches)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (grid[x, y] == b)
                    {
                        grid[x, y] = null;
                        break;
                    }
                }
            }
            Destroy(b.gameObject);
        }
        StartCoroutine(RefillRoutine());
    }

    IEnumerator RefillRoutine()
    {
        yield return new WaitForSeconds(0.1f); 
        CollapseColumn();
        yield return new WaitForSeconds(0.2f); 
        FillEmptySpaces();
        yield return new WaitForSeconds(0.2f);
        CheckMatchAfterFall();
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
                            grid[x, y] = grid[x, aboveY];
                            grid[x, aboveY] = null;
                            StartCoroutine(Bubble.FallToPosition(grid[x, y].transform, new Vector3(x, y, 0), 0.2f));
                            break;
                        }
                    }
                }
            }
        }
    }

    void FillEmptySpaces()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == null)
                {
                    InitializedGrid(x, y, true);
                }
            }
        }
    }

    void CheckMatchAfterFall()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                CheckMatch(x, y);
            }
        }
    }

}