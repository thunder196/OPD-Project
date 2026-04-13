using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using Unity.VisualScripting;

public class Match3Grid : MonoBehaviour
{
    public int width = 6;
    public int height = 6;
    public GameObject Match3Prefab; 
    private Bubble[,] grid;
    private Bubble firstSelected;
    [Header("UI Setting")]
    public TextMeshProUGUI scoreText;
    private int currentScore = 0;
    void Start()
    {
        currentScore = 0;
        UpdateScoreUI();
        grid = new Bubble[width, height];
        GenerateValidBoard();
    }

    void GenerateValidBoard()
    {
        GenerateGrid();

        while (HasAnyMatchesOnBoard())
        {
            ReshuffleTypes();
            while (CheckGridOnPossibleMove() == false)
            {
                ShuffleTheGrid();
            }
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
        var matchx = 1 + CheckDirection(x, y, 1, 0).Count + CheckDirection(x, y, -1, 0).Count;
        var matchy = 1 + CheckDirection(x, y, 0, 1).Count + CheckDirection(x, y, 0, -1).Count;
        return matchx >= 3 || matchy >= 3;
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
                return;
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

    bool CheckGridOnPossibleMove()
    {
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                if (grid[x, y] == null) continue;
                if (x + 1 < width)
                { 
                    if (SwapAndCheckPossible(x, y, x+1, y)) return true;  
                }
                if (y + 1 < height) 
                { 
                    if (SwapAndCheckPossible(x, y, x, y+1)) return true;
                }
            }
        }
        return false;
    }

    void ShuffleTheGrid()
    { 
        var listAllBubble = new List<Bubble>();
        foreach (var b in grid)
        { 
            if (b!=null) listAllBubble.Add(b);
        }
        for (int i = 0; i < listAllBubble.Count; i++)
        {
            var temp = listAllBubble[i];
            var randomIndex = Random.Range(i, listAllBubble.Count);
            listAllBubble[i] = listAllBubble[randomIndex];
            listAllBubble[randomIndex] = temp;
        }

        var index = 0;
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                grid[x, y] = listAllBubble[index];
                StartCoroutine(Bubble.FallToPosition(grid[x, y].transform, new Vector3(x, y, 0)));
                index++;
            }
        }

    }

    bool SwapAndCheckPossible(int x1, int y1, int x2, int y2)
    { 
        var temp = grid[x1, y1];
        grid[x1, y1] = grid[x2, y2];
        grid[x2, y2] = temp;

        var hasmatch = CheckAnyMatch(x1, y1) || CheckAnyMatch(x2, y2);
        grid[x2, y2] = grid[x1, y1];
        grid[x1, y1] = temp;

        return hasmatch;
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
        if (grid[startX, startY] == null) return result;
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
        var pointPerBubble = 10;
        AddScore(matches.Count * pointPerBubble);
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
        if (HasAnyMatchesOnBoard())
        {
            CheckMatchAfterFall();
        }
        else if (!CheckGridOnPossibleMove())
        {
            var safetyNet = 0;
            while (!CheckGridOnPossibleMove() && safetyNet < 50)
            {
                ShuffleTheGrid();
                safetyNet++;
            }
        }
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
    public void AddScore(int amount)
    {
        currentScore += amount;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    { 
        if (scoreText != null)
            scoreText.text = "Score: " + currentScore.ToString();
    }

}