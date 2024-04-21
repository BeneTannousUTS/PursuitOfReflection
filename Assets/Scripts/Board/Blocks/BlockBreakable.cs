using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBreakable : Block
{
    [SerializeField] int startingDurability = 1;
    [SerializeField] Sprite[] spriteSheet;

    int _durability = -1;
    int durability
    {
        get => _durability;
        set
        {
            if (durability == value)
                return;
            
            _durability = value;
            if (_durability <= 0)
                Game.board.RemoveBlock(coords);
            
            Game.board.OnBoardChange();
        }
    }


    protected override void Init()
    {
        base.Init();
        durability = startingDurability;
    }


    protected override void OnPlayerBump(Player player, Vector2Int playerDirection)
    {
        base.OnPlayerBump(player, playerDirection);
        durability--;
    }


    public override Dictionary<string, object> GetData()
    {
        Dictionary<string, object> data = base.GetData();
        data.Add("durability", durability);
        return data;
    }


    public override void SetData(Dictionary<string, object> data)
    {
        base.SetData(data);
        durability = (int)data["durability"];
    }


    protected override void UpdateSprite()
    {
        base.UpdateSprite();

        if (durability <= 0)
            return;
        
        spriteRenderer.sprite = spriteSheet[startingDurability - durability];
    }
}
