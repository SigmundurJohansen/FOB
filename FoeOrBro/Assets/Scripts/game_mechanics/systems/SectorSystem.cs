using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Transforms;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Burst;


public struct SectorEntity : IComponentData {
    public TypeEnum typeEnum;

    public enum TypeEnum {
        Unit,
        Target
    }
}

public struct SectorData {
    public Entity entity;
    public float3 position;
    public SectorEntity SectorEntity;
}

public class SectorSystem : ComponentSystem {

    public static NativeMultiHashMap<int, SectorData> SectorMultiHashMap;

    public const int SectorYMultiplier = 200;
    private const int SectorCellSize = 2;

    public static int GetPositionHashMapKey(float3 position) {
        return (int) (math.floor(position.x / SectorCellSize) + (SectorYMultiplier * math.floor(position.y / SectorCellSize)));
    }

    private static void DebugDrawSector(float3 position) {
        Vector3 lowerLeft = new Vector3(math.floor(position.x / SectorCellSize) * SectorCellSize, math.floor(position.y / SectorCellSize) * SectorCellSize);
        Debug.DrawLine(lowerLeft, lowerLeft + new Vector3(+1, +0) * SectorCellSize);
        Debug.DrawLine(lowerLeft, lowerLeft + new Vector3(+0, +1) * SectorCellSize);
        Debug.DrawLine(lowerLeft + new Vector3(+1, +0) * SectorCellSize, lowerLeft + new Vector3(+1, +1) * SectorCellSize);
        Debug.DrawLine(lowerLeft + new Vector3(+0, +1) * SectorCellSize, lowerLeft + new Vector3(+1, +1) * SectorCellSize);
        //Debug.Log(GetPositionHashMapKey(position) + " " + position);
    }

    private static int GetEntityCountInHashMap(NativeMultiHashMap<int, SectorData> SectorMultiHashMap, int hashMapKey) {
        SectorData SectorData;
        NativeMultiHashMapIterator<int> nativeMultiHashMapIterator;
        int count = 0;
        if (SectorMultiHashMap.TryGetFirstValue(hashMapKey, out SectorData, out nativeMultiHashMapIterator)) {
            do {
                count++;
            } while (SectorMultiHashMap.TryGetNextValue(out SectorData, ref nativeMultiHashMapIterator));
        }
        return count;
    }

    [BurstCompile]
    private struct SetSectorDataHashMapJob : IJobForEachWithEntity<Translation, SectorEntity> {

        public NativeMultiHashMap<int, SectorData>.ParallelWriter SectorMultiHashMap;

        public void Execute(Entity entity, int index, ref Translation translation, ref SectorEntity SectorEntity) {
            int hashMapKey = GetPositionHashMapKey(translation.Value);
            SectorMultiHashMap.Add(hashMapKey, new SectorData {
                entity = entity,
                position = translation.Value,
                SectorEntity = SectorEntity
            });
        }

    }

    protected override void OnCreate() {
        SectorMultiHashMap = new NativeMultiHashMap<int, SectorData>(0, Allocator.Persistent);
        base.OnCreate();
    }

    protected override void OnDestroy() {
        SectorMultiHashMap.Dispose();
        base.OnDestroy();
    }

    protected override void OnUpdate() {
        EntityQuery entityQuery = GetEntityQuery(typeof(Translation), typeof(SectorEntity));

        SectorMultiHashMap.Clear();
        if (entityQuery.CalculateEntityCount() > SectorMultiHashMap.Capacity) {
            SectorMultiHashMap.Capacity = entityQuery.CalculateEntityCount();
        }

        SetSectorDataHashMapJob setSectorDataHashMapJob = new SetSectorDataHashMapJob {
            SectorMultiHashMap = SectorMultiHashMap.AsParallelWriter(),
        };
        JobHandle jobHandle = JobForEachExtensions.Schedule(setSectorDataHashMapJob, entityQuery);
        jobHandle.Complete();

        DebugDrawSector(CameraController.Instance.WorldPosition());
        //Debug.Log(GetEntityCountInHashMap(SectorMultiHashMap, GetPositionHashMapKey(CameraController.Instance.WorldPosition())));
    }

}