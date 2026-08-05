using UnityEngine;
using VContainer;


public class Inventory : IInventoryService
{
    [Inject]
    IEventBus eventBus;
    
    public void AddItem(InteractData item)
    {
    }

    public void RemoveItem(InteractData item)
    {
    }

    public bool HasItem(InteractData item)
    {
        return false;
    }
}