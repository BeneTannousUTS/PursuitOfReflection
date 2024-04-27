using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Solver : MonoBehaviour
{
    Dictionary<Board, (Board, Vector2Int)> visited = new Dictionary<Board, (Board, Vector2Int)>();
    [SerializeField] int maxDepth = 10;

    void Start()
    {
        StartCoroutine(Solve());
    }


    IEnumerator Solve()
    {
        yield return new WaitForSeconds(0.1f);

        yield return Solve(Game.board, animate: true);
    }


    IEnumerator Solve(Board board, int depth = 0, bool animate = false)
    {
        if (depth > maxDepth)
            yield break;

        foreach (Vector2Int direction in Game.directions)
        {
            Board newBoard = Instantiate(board);
            Game.board = newBoard;
            newBoard.Init();

            yield return new WaitForEndOfFrame();

            Game.CalcNextTurn(newBoard, direction, animate);

            if (visited.ContainsKey(newBoard))
                continue;
            visited[newBoard] = (board, direction);

            bool isFinished = Game.IsGameFinished(out bool win);

            if (isFinished)
            {
                if (win)
                {
                    print("Solved: " + depth);
                    yield break;
                }
                
                yield return null;
            }

            yield return Solve(newBoard, depth + 1, animate);

            Destroy(newBoard.gameObject);
        }
    }
}