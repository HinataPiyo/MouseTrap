public interface IInventoryService
{
    void AddItem(InteractData item);
    void RemoveItem(InteractData item);
    bool HasItem(InteractData item);
}