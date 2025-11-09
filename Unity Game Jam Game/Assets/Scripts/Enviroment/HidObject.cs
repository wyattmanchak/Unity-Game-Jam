using UnityEngine;

public class HidObject : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite normalSprite;
    public Sprite playerHidingSprite;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        spriteRenderer.sprite = normalSprite;
    }

    public void UnHide()
    {
        if (spriteRenderer != null && normalSprite != null)
        {
            spriteRenderer.sprite = normalSprite;
        }
    }

    public void Hide()
    {
        if (spriteRenderer != null && playerHidingSprite != null)
        {
            spriteRenderer.sprite = playerHidingSprite;
        }
    }
}
