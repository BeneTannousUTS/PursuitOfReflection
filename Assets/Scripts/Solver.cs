using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Solver : MonoBehaviour
{
    Dictionary<Board, (int?, Vector2Int)> visited = new Dictionary<Board, (int?, Vector2Int)>();
    [SerializeField] int maxDepth = 20;
    int currentDepth = 0;

    void Start()
    {
        StartCoroutine(Solve());
    }


    IEnumerator Solve()
    {
        yield return new WaitForSeconds(0.1f);

        visited[Game.board] = (null, Vector2Int.zero);
        yield return Solve(Game.board, animate: false);
    }


    IEnumerator Solve(Board board, bool animate)
    {
        Queue<(Board, int)> queue = new Queue<(Board, int)>();
        queue.Enqueue((board, 0));

        while (queue.Count > 0)
        {
            (Board currentBoard, int depth) = queue.Dequeue();
            
            foreach (Vector2Int direction in Game.directions)
            {
                Board newBoard = Instantiate(currentBoard);
                Game.board = newBoard;
                newBoard.Init();

                yield return new WaitForEndOfFrame();

                Game.CalcNextTurn(newBoard, direction, animate);

                if (visited.ContainsKey(newBoard))
                {
                    Destroy(newBoard.gameObject);
                    continue;
                }
                visited[newBoard] = (board.GetHashCode(), direction);

                bool isFinished = Game.IsGameFinished(out bool win);

                if (isFinished)
                {
                    if (win)
                    {
                        print("Solved: " + (depth + 1));
                        yield break;
                    }
                    
                    Destroy(newBoard.gameObject);
                    continue;
                }

                if (currentDepth <= depth)
                {
                    currentDepth = depth + 1;
                    print("Depth: " + (depth + 1));
                }

                if (depth < maxDepth - 1)
                    queue.Enqueue((newBoard, depth + 1));
                else
                    Destroy(newBoard.gameObject);
            }

            if (depth > 0)
                Destroy(currentBoard.gameObject);
        }

        print("No solution found");
    }


    string Dir(Vector2Int direction)
    {
        if (direction == Vector2Int.up)
            return "↑";
        if (direction == Vector2Int.down)
            return "↓";
        if (direction == Vector2Int.left)
            return "←";
        if (direction == Vector2Int.right)
            return "→";
        return "";
    }
}