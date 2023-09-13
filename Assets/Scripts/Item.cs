using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public const string ITEM_TAG = "Item";

    public ItemData data;
    public ItemRepository repository;

    private new SpriteRenderer renderer;

    private void Start()
    {
        this.renderer = GetComponent<SpriteRenderer>();
    }

    public void UpdateItemData(ItemData newData)
    {
        this.UpdateSprite(newData.sprite);
        this.UpdateScale(newData.scale);
        this.data = newData;
    }

    private void UpdateSprite(Sprite newSprite)
    {
        PolygonCollider2D old = GetComponent<PolygonCollider2D>();
        if (old)
        {
            if (Application.isPlaying)
            {
                Destroy(old);
            }
            else
            {
                DestroyImmediate(old);
            }
        }
        this.renderer.sprite = newSprite;
        gameObject.AddComponent<PolygonCollider2D>();
    }

    private void UpdateScale(float newScale)
    {
        transform.localScale = Vector3.one * newScale;
    }

    [ContextMenu("Sync")]
    private void Sync()
    {
        this.renderer = GetComponent<SpriteRenderer>();
        this.UpdateItemData(this.data);
    }

    // TODO: VFX
    private void OnCollisionEnter2D(Collision2D collision2D)
    {
        if (collision2D.gameObject.CompareTag(Item.ITEM_TAG))
        {
            Item other = collision2D.gameObject.GetComponent<Item>();
            if (other.data.level == this.data.level)
            {
                if (transform.position.y < collision2D.gameObject.transform.position.y)
                {
                    ItemData newData = this.repository.GetItem(this.data.level + 1);
                    this.UpdateItemData(newData);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
