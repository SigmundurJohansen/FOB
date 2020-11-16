using Unity.Entities;

public struct StateComponent : IComponentData { public int state; }
public struct IdleComponent : IComponentData { }
public struct CombatComponent : IComponentData { }
public struct RoamingComponent : IComponentData { }
public struct TaskComponent : IComponentData { }
public struct SeekComponent : IComponentData { }
public struct HasTarget : IComponentData { public Entity targetEntity; }