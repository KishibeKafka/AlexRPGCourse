using System;

[Serializable]
public class InventoryItem
{
    public ItemData data;
    public int stackSize;
    public InventoryItem(ItemData _data)
    {
        data = _data;
        AddStack();
    }

    public void AddStack() => stackSize++;
    public void RemoveStack() => stackSize--;
}
