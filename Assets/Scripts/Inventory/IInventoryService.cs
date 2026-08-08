using System.Collections.Generic;

public interface IInventoryService
{
    void ChangeItemCount(OnItemChanged item);
    void AddItem(OnItemChanged item);
    void RemoveItem(OnItemChanged item);
    bool HasItem(InteractData item);
    List<Inventory.ItemData> GetItems();
}