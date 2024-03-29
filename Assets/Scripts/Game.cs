using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class Game : MonoBehaviour 
{
    public static bool isInitialized = false;

    [SerializeField] Board _board;
    public static Board board;

    [SerializeField] UI _ui;
    public static UI ui;

    [SerializeField] MainPlayer _mainPlayer;
    public static MainPlayer mainPlayer;

    [SerializeField] ChasedPlayer _chasedPlayer;
    public static ChasedPlayer chasedPlayer;

    static int _turn = 0;
    public static int turn
    {
        get { return _turn; }
        set
        {
            _turn = value;
            ui.UpdateTurnRemainingText(maxTurns - _turn);

            if (IsGameFinished(out bool won))
            {
                if (won)
                    Debug.Log("You Won!");
                else
                    Debug.Log("You Lost!");
            }
        }
    }

    [SerializeField] int _maxTurns;
    public static int maxTurns;

    public static Player[] players;

    public void Awake()
    {
        board = _board;
        ui = _ui;
        mainPlayer = _mainPlayer;
        chasedPlayer = _chasedPlayer;
        maxTurns = _maxTurns;
        players = new Player[] { mainPlayer, chasedPlayer };

        isInitialized = true;
    }

    static bool IsGameFinished(out bool won)
    {
        won = false;

        if (mainPlayer.coords == chasedPlayer.coords)
        {
            won = true;
            return true;
        }

        if (turn >= maxTurns)
        {
            return true;
        }

        return false;
    }


    static void ActivateBumpingBlocks(Vector2Int playerDirection)
    {
        HashSet<Block> bumpingBlocks = new HashSet<Block>();
        HashSet<Block> enteringBlocks = new HashSet<Block>();

        foreach (Player player in players)
        {
            Block block = board.GetBlock(player.coords + playerDirection);
            if (block == null)
                continue;
            
            if (board.CanPlayerMoveTo(block.coords))
                enteringBlocks.Add(block);
            else
                bumpingBlocks.Add(block);
        }

        foreach (Block block in enteringBlocks)
        {
            block.PlayerEnter();
        }

        foreach (Block block in bumpingBlocks)
        {
            block.PlayerBump();
        }
    }


    static public void OnTurnChange(Vector2Int playerDirection)
    {
        board.OnTurnChange();

        ActivateBumpingBlocks(playerDirection);

        mainPlayer.OnTurnChange(playerDirection);
        chasedPlayer.OnTurnChange(playerDirection);

        turn++;
    }


    void OnValidate()
    {
        Awake();
    }
}