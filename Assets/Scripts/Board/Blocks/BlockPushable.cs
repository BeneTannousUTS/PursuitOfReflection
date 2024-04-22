using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPushable : Block
{
    protected override void OnPlayerEnter(Player player, Vector2Int playerDirection)
    {
        Block nextBlock = Game.board.GetBlock(coords + playerDirection);
        if (nextBlock is BlockPushable)
            nextBlock.PlayerEnter(player, playerDirection);

        base.OnPlayerEnter(player, playerDirection);
        coords += playerDirection;
    }


    public override bool CanPlayerMoveInside(Player player, Vector2Int playerDirection)
    {
        Vector2Int nextCoords = coords + playerDirection;

        if (!Game.board.IsInsideBoard(nextCoords))
            return false;
        
        Block nextBlock = Game.board.GetBlock(nextCoords);

        if (nextBlock == null)
            return true;
        
        if (nextBlock is BlockPushable)
            return nextBlock.CanPlayerMoveInside(player, playerDirection);
        
        return false;
    }
}
