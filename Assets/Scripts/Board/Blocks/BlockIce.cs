using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BlockIce : Block
{
    protected override void OnPlayerEnter(Player player, Vector2Int playerDirection, bool animate)
    {
        base.OnPlayerEnter(player, playerDirection, animate);

        if (player.isSliding)
            return;
        
        player.QueueAction(SlidePlayer(player, playerDirection, animate));
    }

    Action SlidePlayer(Player player, Vector2Int playerDirection, bool animate)
    {
        player.isSliding = true;

        return () =>
        {
            Block nextBlock = Game.board.GetBlock(player.coords + playerDirection);

            player.Move(playerDirection, animate);

            if (Game.board.CanPlayerMoveTo(player, player.coords + playerDirection, playerDirection))
            {
                player.QueueAction(SlidePlayer(player, playerDirection, animate));
            }
            else
            {
                player.Move(playerDirection, animate);
                player.isSliding = false;
            }
        };
    }
}
