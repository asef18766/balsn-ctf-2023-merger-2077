using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemRepository : ScriptableObject
{
    public List<ItemData> items;

    public ItemData GetItem(int level)
    {
        return this.items[level];
    }
}
