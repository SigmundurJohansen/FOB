using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
/*
public class RemoveDeadTargetReferencesSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var targetables = GetComponentDataFromEntity<TargetableComponent>(true);
        Entities
            .ForEach(
            (ref TargetComponent target) =>
            {
                if (!targetables.Exists(target.Value))
                    target.Value = Entity.Null;
            }
            )
            .WithReadOnly(targetables)
            .Schedule();
    }
}
*/