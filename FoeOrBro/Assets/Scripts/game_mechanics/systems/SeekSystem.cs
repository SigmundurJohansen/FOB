using Unity.Entities;

public class SeekSystem : ComponentSystem
{    
    protected override void OnUpdate() 
    {
        Entities.ForEach((Entity entity, ref RaceComponent _race, ref IDComponent _id, ref TargetComponent _target, ref StateComponent _state) => {
            if(_state.state == 0) //seek
            {
                    
            }
        });
       
    }
}