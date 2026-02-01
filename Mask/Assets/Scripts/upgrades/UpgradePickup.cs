using UnityEngine;

public class UpgradePickup : MonoBehaviour
{
    [SerializeField] UpgradeConfig config;
    [SerializeField] float lifetimeSeconds = 20f;

    [Header("Optional visuals")]
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite fallbackSprite;

    [Header("Optional sorting override")]
    [SerializeField] bool overrideSorting = false;
    [SerializeField] string sortingLayerName = "";
    [SerializeField] int sortingOrder = 0;

    [SerializeField] float bobAmplitude = 0.12f;
    [SerializeField] float bobFrequency = 2.0f;

    Vector3 startPos;

    void Awake()
    {
        startPos = transform.position;
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        ApplyVisuals();

        if (lifetimeSeconds > 0f)
        {
            Destroy(gameObject, lifetimeSeconds);
        }
    }

    void Update()
    {
        // Simple bob so it feels like a pickup
        transform.position = startPos + Vector3.up * (Mathf.Sin(Time.time * bobFrequency) * bobAmplitude);
    }

    public void SetConfig(UpgradeConfig newConfig)
    {
        config = newConfig;
        ApplyVisuals();
    }

    void ApplyVisuals()
    {
        if (spriteRenderer == null) return;

        if (overrideSorting)
        {
            if (!string.IsNullOrEmpty(sortingLayerName)) spriteRenderer.sortingLayerName = sortingLayerName;
            spriteRenderer.sortingOrder = sortingOrder;
        }

        // Prefer the upgrade icon; fallback to prefab sprite or a provided fallback sprite
        if (config != null && config.icon != null)
        {
            spriteRenderer.sprite = config.icon;
        }
        else if (spriteRenderer.sprite == null && fallbackSprite != null)
        {
            spriteRenderer.sprite = fallbackSprite;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        TryPickup(other.gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        TryPickup(collision.gameObject);
    }

    void TryPickup(GameObject otherObj)
    {
        if (otherObj == null) return;

        // Player collider might be on a child (e.g., Player/Sprite). Accept parent/root.
        GameObject playerObj = null;
        if (otherObj.CompareTag("Player"))
        {
            playerObj = otherObj;
        }
        else
        {
            Transform root = otherObj.transform.root;
            if (root != null && root.CompareTag("Player")) playerObj = root.gameObject;
        }

        // Fallback: detect via known player component
        if (playerObj == null)
        {
            PlayerMove pm = otherObj.GetComponentInParent<PlayerMove>();
            if (pm != null) playerObj = pm.gameObject;
        }

        if (playerObj == null) return;

        if (config != null)
        {
            UpgradeApplier.ApplyToPlayer(playerObj, config);
        }

        Destroy(gameObject);
    }
}

