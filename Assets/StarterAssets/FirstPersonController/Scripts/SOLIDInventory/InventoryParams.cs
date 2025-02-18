public class InventoryParams
{
    public int MaxSlots { get; private set; }
    public bool AllowStacking { get; private set; }

    public InventoryParams(int maxSlots, bool allowStacking)
    {
        MaxSlots = maxSlots;
        AllowStacking = allowStacking;
    }
}