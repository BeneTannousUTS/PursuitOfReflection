using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockFlashing : Block
{
    [SerializeField] float animationSpeed = 5f;
    [SerializeField] Sprite[] sprites;


    protected override void UpdateSprite()
    {
        spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
        StartCoroutine(FlashingAnimation());
    }


    IEnumerator FlashingAnimation()
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            spriteRenderer.sprite = isTransparent ? sprites[sprites.Length - i - 1] : sprites[i];
            yield return new WaitForSeconds(1 / animationSpeed);
        }
    }


    protected override void OnTurnChange()
    {
        isTransparent = !isTransparent;
    }
}
