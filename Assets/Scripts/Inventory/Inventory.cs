using System.Collections.Generic;
using VContainer;
using UnityEngine;

public struct OnItemChanged
{
    public Inventory.ItemData itemData;
    public bool isAdded; // true: アイテム追加, false: アイテム削除

    public OnItemChanged(Inventory.ItemData itemData, bool isAdded)
    {
        this.itemData = itemData;
        this.isAdded = isAdded;
    }
}

public class Inventory : IInventoryService
{
    List<ItemData> items = new List<ItemData>();

    [System.Serializable]
    public class ItemData
    {
        public InteractData item;
        public int count;
    }

    [Inject]
    public void Construct(IEventBus eventBus)
    {
        eventBus.Subscribe<OnItemChanged>(ChangeItemCount);
    }

    public void ChangeItemCount(OnItemChanged item)
    {
        if (item.isAdded)
        {
            AddItem(new OnItemChanged(item.itemData, true));
        }
        else
        {
            RemoveItem(new OnItemChanged(item.itemData, false));
        }
    }
    
    
    public void AddItem(OnItemChanged message)
    {
        var item = message.itemData.item;
        var count = message.itemData.count;
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

    public void RemoveItem(OnItemChanged item)
    {
        Debug.Log($"Removing item: {item.itemData.item.name}, Count: {item.itemData.count}");
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].item == item.itemData.item)
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