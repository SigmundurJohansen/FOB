using Unity.Entities;

public struct StateComponent : IComponentData { public int state; }
public struct IdleComponent : IComponentData { }
public struct CombatComponent : IComponentData {}
public struct MoraleComponent : IComponentData {public int baseMorale; public int healthModifier; public int numberModifier; }

public struct RoamingComponent : IComponentData { }
public struct PanicTag : IComponentData { }
public struct DoNotTarget : IComponentData { }
public struct FleeTag : IComponentData { }
public struct TaskComponent : IComponentData { }
public struct SeekComponent : IComponentData { }
public struct HasTarget : IComponentData { public Entity targetEntity; }