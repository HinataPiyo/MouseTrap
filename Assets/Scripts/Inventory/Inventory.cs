using System.Collections.Generic;
using UnityEngine;


public class Inventory : SceneSingleton<Inventory>
{
    // 所持品はシーンをまたいで保持するため、GameSessionに置く
    List<ItemData> items => GameSession.InventoryItems;

    [System.Serializable]
    public class ItemData
    {
        public InteractData item;
        public int count;
    }
    
    public void AddItem(InteractData message)
    {
        var item = message;
        var count = 1;
        Debug.Log($"Adding item: {item.name}, Count: {count}");

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].item == item)
            {
                items[i].count += count;
                return;
            }
        }
        // アイテムが存在しない場合は新規追加
        items.Add(new ItemData { item = item, count = count });
    }

    public void RemoveItem(InteractData item)
    {
        Debug.Log($"Removing item: {item.name}");
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].item == item)
            {
                items[i].count--;

                // アイテムの数が0以下になった場合はリストから削除
                if (items[i].count <= 0)
                {
                    items.RemoveAt(i);
                }
                return;
            }
        }
    }

    /// <summary>
    /// アイテムがインベントリに存在するかどうかを確認する
    /// </summary>
    public bool HasItem(InteractData item)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].item == item)
            {
                return true;
            }
        }
        return false;
    }

    public List<ItemData> GetItems() => items;
}