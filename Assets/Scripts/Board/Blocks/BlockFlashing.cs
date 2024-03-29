using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockFlashing : Block
{
    [SerializeField] bool isTransparent;
    [SerializeField] float animationSpeed = 5f;


    protected override void UpdateSprite()
    {
        StartCoroutine(ChangeSpriteColor(new Color(
            spriteRenderer.color.r,
            spriteRenderer.color.g,
            spriteRenderer.color.b,
            isTransparent ? 0.5f : 1f
        ), animationSpeed));
    }


    public override bool CanPlayerMoveInside()
    {
        return isTransparent;
    }


    protected override void OnTurnChange()
    {
        isTransparent = !isTransparent;
    }
}
