using System.Collections.Generic;
using UnityEngine;


public interface IInventoryService
{
    void AddItem(InteractData item);
    void RemoveItem(InteractData item);
    bool HasItem(InteractData item);
    List<Inventory.ItemData> GetItems();
}

public class Inventory : MonoBehaviour, IInventoryService
{
    static Inventory instance;
    List<ItemData> items = new List<ItemData>();

    [System.Serializable]
    public class ItemData
    {
        public InteractData item;
        public int count;
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            ApiProvider.Register<IInventoryService>(this);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
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