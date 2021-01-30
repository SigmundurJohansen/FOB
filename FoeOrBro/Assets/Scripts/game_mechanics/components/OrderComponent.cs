using Unity.Entities;

public struct OrderComponent : IComponentData
{
    public bool hasOrders;
    public int orderType;
}