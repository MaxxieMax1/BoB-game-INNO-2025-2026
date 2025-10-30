using UnityEngine;
using UnityEngine.UIElements;

public class Emoji : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Image image2;
    public Sprite neutralSprite;
    public Sprite happySprite;
    public Sprite sadSprite;

    public void Neutral()
    {
        spriteRenderer.sprite = neutralSprite;
        spriteRenderer.color = Color.gray;
    }

    public void Happy()
    {
        spriteRenderer.sprite = happySprite;
        spriteRenderer.color = Color.green;
    }

    public void Sad()
    {
        spriteRenderer.sprite = sadSprite;
        spriteRenderer.color = Color.red;
    }

}
