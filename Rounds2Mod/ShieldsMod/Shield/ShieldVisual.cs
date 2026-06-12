using ShieldsMod.Integration;
using UnityEngine;

namespace ShieldsMod.Shield;

/// <summary>Semi-transparent circle bubble parented to the player's wobble transform.</summary>
public class ShieldVisual : MonoBehaviour
{
    /// <summary>Body radius × this = shield bubble radius (was 1.2; +10% → 1.32).</summary>
    public const float SizeMultiplier = 1.44f;
    private const int CircleTexSize = 128;

    private Player _player;
    private SpriteRenderer _sprite;
    private ShieldState _state;

    public static Sprite SharedCircleSprite { get; private set; }

    public void Init(Player player, ShieldState state)
    {
        _player = player;
        _state = state;

        if (SharedCircleSprite == null)
            SharedCircleSprite = CreateCircleSprite(CircleTexSize);

        var spriteGo = new GameObject("ShieldBubbleSprite");
        spriteGo.transform.SetParent(transform, false);
        _sprite = spriteGo.AddComponent<SpriteRenderer>();
        _sprite.sprite = SharedCircleSprite;
        _sprite.sortingOrder = -1;

        int layerId = SortingLayer.NameToID("Player" + (player.playerID + 1));
        if (layerId != 0)
            _sprite.sortingLayerID = layerId;

        player.data.SetWobbleObjectChild(transform);
        Refresh();
    }

    public void SetState(ShieldState state) => _state = state;

    public void Refresh()
    {
        if (_sprite == null || _player == null || _state == null)
            return;

        if (!_state.IsActive)
        {
            _sprite.enabled = false;
            return;
        }

        _sprite.enabled = true;
        _sprite.color = KeyboundInvisibilityBridge.IsPlayerInvisible(_player)
            ? KeyboundInvisibilityBridge.InvisibleColor
            : _state.GetBubbleColor();

        float bodyRadius = GetBodyRadius(_player);
        float diameter = bodyRadius * 2f * SizeMultiplier;
        float spriteWorldSize = _sprite.sprite.bounds.size.x;
        if (spriteWorldSize > 0.0001f)
            _sprite.transform.localScale = Vector3.one * (diameter / spriteWorldSize);
    }

    public static float GetBodyRadius(Player player)
    {
        if (player == null)
            return 0.5f;

        var circle = player.GetComponent<CircleCollider2D>();
        if (circle == null)
            circle = player.GetComponentInChildren<CircleCollider2D>();

        float colRadius = circle != null ? circle.radius : 0.5f;
        return colRadius * player.transform.localScale.x;
    }

    private void LateUpdate()
    {
        if (_state != null && _state.IsActive)
            Refresh();
    }

    private static Sprite CreateCircleSprite(int size)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        float center = (size - 1) * 0.5f;
        float radius = center - 1f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - center;
                float dy = y - center;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                float a = dist <= radius ? 1f : Mathf.Clamp01(1f - (dist - radius));
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
            }
        }

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }
}
