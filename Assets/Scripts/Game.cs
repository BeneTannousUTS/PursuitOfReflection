using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Linq;


[ExecuteInEditMode]
public class Game : MonoBehaviour
{
    public static string[] levels = new string[]
    {
        "Tutorial 1", "Tutorial 2", "Tutorial 3", "Tutorial 4",
        "Level 1.1", "Level 1.2", "Level 1.3", "Level 1.4",
        "Level 2.1", "Level 2.2", "Level 2.3", //"Level 2.4",
        "Level 3.1", "Level 3.2", "Level 3.3", //"Level 3.4",
        "Level 4.1", "Level 4.2", "Level 4.3", "Level 4.4",
    };

    public List<GameObject> blockPrefabs = new List<GameObject>();
    public static Dictionary<string, GameObject> blockNameToPrefab = new Dictionary<string, GameObject>();
    public static bool isPaused = false;
    public static Dictionary<LineRenderer, int[]> visibilityLines = new Dictionary<LineRenderer, int[]>();

    [SerializeField] Board _board;
    public static Board board;

    [SerializeField] UI _ui;
    public static UI ui;

    public static Game game;

    static int _turn = 0;
    public static int turn
    {
        get { return _turn; }
        set
        {
            _turn = value;
            ui.UpdateTurnText();

            game.StartCoroutine(OnGameFinish());
        }
    }

    [SerializeField] int _threeStarsTurns;
    public static int threeStarsTurns;

    [SerializeField] int _twoStarsTurns;
    public static int twoStarsTurns;

    [SerializeField] int _oneStarTurns;
    public static int oneStarTurns;

    [SerializeField] Player[] _players;

    public static Queue<IEnumerator> coroutinesToPlayAtEnd = new Queue<IEnumerator>();

    public static Vector2Int[] directions = new Vector2Int[]
    {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
    };

    public void Awake()
    {
        game = this;
        board = _board;
        ui = _ui;
        threeStarsTurns = _threeStarsTurns;
        twoStarsTurns = _twoStarsTurns;
        oneStarTurns = _oneStarTurns;
        board.players = _players;

        foreach (GameObject blockPrefab in blockPrefabs)
        {
            Block block = blockPrefab.GetComponent<Block>();
            if (block == null)
                throw new Exception($"Block prefab {blockPrefab.name} does not have a Block component attached to it.");
            blockNameToPrefab[block.blockName] = blockPrefab;
        }
    }


    public void Start()
    {
        foreach (Player player in board.players)
            player.UpdatePlayerToStartingCoords();
        
        DestroyImmediate(GameObject.Find("Visibility Lines"));
        GameObject visibilityLinesObject = new GameObject("Visibility Lines");

        visibilityLines.Clear();
        for (int i = 0; i < board.players.Length; i++)
        {
            for (int j = i + 1; j < board.players.Length; j++)
            {
                GameObject visibilityLine = new GameObject("Visibility Line", typeof(LineRenderer));
                visibilityLine.transform.parent = visibilityLinesObject.transform;

                LineRenderer lineRenderer = visibilityLine.GetComponent<LineRenderer>();
                InitLineRenderer(lineRenderer);
                visibilityLines.Add(lineRenderer, new int[] { i, j });
            }
        }
        UpdateVisibilityLines();

        StartCoroutine(StartGame());
    }


    public IEnumerator StartGame()
    {
        yield return null;

        turn = 0;
        isPaused = false;

        board.ResetBoardState();
    }


    static IEnumerator OnGameFinish()
    {
        yield return new WaitForFixedUpdate();

        if (!IsGameFinished(out bool won))
            yield break;

        isPaused = true;
        game.StartCoroutine(GameAnimationsEndLoop());

        if (won)
        {
            PlayerPrefs.SetInt("Score", CalcScore());
            coroutinesToPlayAtEnd.Enqueue(LevelClearedAnimation());
        }
        else
        {
            coroutinesToPlayAtEnd.Enqueue(GameOverAniamtion());
        }
    }


    static IEnumerator LevelClearedAnimation()
    {
        yield return null;
        SceneManager.LoadScene("LevelCleared", LoadSceneMode.Additive);
    }


    static IEnumerator GameOverAniamtion()
    {
        yield return null;
        SceneManager.LoadScene("GameOver", LoadSceneMode.Additive);
    }


    void InitLineRenderer(LineRenderer lineRenderer)
    {
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.green;
        lineRenderer.sortingOrder = 1000;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    }


    // public void Update()
    // {
    //     UpdateVisibilityLines();
    // }


    void OnUp()
    {
        OnTurnChange(Vector2Int.up);
    }

    void OnDown()
    {
        OnTurnChange(Vector2Int.down);
    }

    void OnLeft()
    {
        OnTurnChange(Vector2Int.left);
    }

    void OnRight()
    {
        OnTurnChange(Vector2Int.right);
    }


    static void UpdateVisibilityLines()
    {
        foreach (KeyValuePair<LineRenderer, int[]> visibilityLine in visibilityLines)
        {
            visibilityLine.Key.SetPosition(0, board.players[visibilityLine.Value[0]].transform.position);
            visibilityLine.Key.SetPosition(1, board.players[visibilityLine.Value[1]].transform.position);
        }
    }


    static bool AllPlayersHaveSameCoords()
    {
        if (board.players.Length == 0)
            return true;

        Vector2Int coords = board.players[0].coords;
        foreach (Player player in board.players)
            if (player.coords != coords)
                return false;
        return true;
    }


    static public bool IsGameFinished(out bool won)
    {
        won = false;

        CheckPlayersVisibility();
        
        foreach (Player player in board.players)
            if (player.isDead)
                return true;
        
        if (AllPlayersHaveSameCoords())
        {
            won = true;
            return true;
        }

        return false;
    }


    static void CheckPlayersVisibility()
    {
        foreach (KeyValuePair<LineRenderer, int[]> visibilityLine in visibilityLines)
        {
            Player player1 = board.players[visibilityLine.Value[0]];
            Player player2 = board.players[visibilityLine.Value[1]];

            if (!player1.CanSeePlayer(player2, out Block[] obstacles))
            {
                player1.isDead = true;
                player2.isDead = true;

                coroutinesToPlayAtEnd.Enqueue(PlayerLostVisibilityAnimation(visibilityLine.Key, new Player[] { player1, player2 }, obstacles));
            }
        }
    }


    static IEnumerator PlayerLostVisibilityAnimation(LineRenderer line, Player[] players, Block[] obstacles)
    {
        for (int i = 0; i < 5; i++)
        {
            line.startColor = Color.green;
            line.endColor = Color.green;
            foreach (Block obstacle in obstacles)
                obstacle.StartCoroutine(obstacle.ChangeSpriteColor(obstacle.defaultColor, 1 / 0.1f));
            yield return new WaitForSeconds(0.1f);
            line.startColor = Color.red;
            line.endColor = Color.red;
            foreach (Block obstacle in obstacles)
                obstacle.StartCoroutine(obstacle.ChangeSpriteColor(Color.red, 1 / 0.1f));
            yield return new WaitForSeconds(0.1f);
        }

        foreach (Player player in players)
            player.Die(animate: true);
    }


    static IEnumerator GameAnimationsEndLoop()
    {
        yield return null;

        while (board.players.Any(player => player.isAnimating))
            yield return null;
        
        while (coroutinesToPlayAtEnd.Count > 0)
            yield return game.StartCoroutine(coroutinesToPlayAtEnd.Dequeue());
    }


    static public int CalcScore()
    {
        if (turn <= threeStarsTurns)
            return 3;
        else if (turn <= twoStarsTurns)
            return 2;
        else if (turn <= oneStarTurns)
            return 1;
        return 0;
    }


    static public void OnTurnChange(Vector2Int playerDirection)
    {
        if (isPaused)
            return;

        CalcNextTurn(board, playerDirection, animate: true);

        turn++;

        board.SaveBoardState();
    }


    static public void CalcNextTurn(Board board, Vector2Int playerDirection, bool animate)
    {
        board.OnTurnChange(animate);
        
        foreach (Player player in board.players)
            player.QueueMove(playerDirection, animate);
        
        while (board.players.Any(player => player.HasActions()))
            CalcNextTick(board, animate);
    }


    static public void CalcNextTick(Board board, bool animate)
    {
        foreach (Player player in board.players)
            player.DoNextAction();
        
        board.OnPlayersActionFinish(animate);
    }


    #if UNITY_EDITOR
    void OnValidate()
    {
        Awake();
    }
    #endif
}
