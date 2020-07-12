using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public struct UnitSelected : IComponentData {
}


public class UnitControlSystem :ComponentSystem{

    protected override void OnUpdate() {}
/*
    private float3 startPosition;
    
    #region OnUpdate
    protected override void OnUpdate() {
        if (Input.GetMouseButtonDown(0)) {
            // Mouse Pressed
            Debug.Log("click down");
            ECSController.instance.selectionAreaTransform.gameObject.SetActive(true);
            startPosition = GetMouseWorldPosition();
            ECSController.instance.selectionAreaTransform.position = startPosition;
        }

        if (Input.GetMouseButton(0)) {
            Debug.Log("dragging");
            // Mouse Held Down
            float3 selectionAreaSize = (float3)GetMouseWorldPosition() - startPosition;
            ECSController.instance.selectionAreaTransform.localScale = selectionAreaSize;
        }

        if (Input.GetMouseButtonUp(0)) {
            Debug.Log("click up");
            // Mouse Released
            ECSController.instance.selectionAreaTransform.gameObject.SetActive(false);
            float3 endPosition = GetMouseWorldPosition();

            float3 lowerLeftPosition = new float3(math.min(startPosition.x, endPosition.x), math.min(startPosition.y, endPosition.y), 0);
            float3 upperRightPosition = new float3(math.max(startPosition.x, endPosition.x), math.max(startPosition.y, endPosition.y), 0);

            bool selectOnlyOneEntity = false;
            float selectionAreaMinSize = 10f;
            float selectionAreaSize = math.distance(lowerLeftPosition, upperRightPosition);
            if (selectionAreaSize < selectionAreaMinSize) {
                // Selection area too small
                lowerLeftPosition +=  new float3(-1, -1, 0) * (selectionAreaMinSize - selectionAreaSize) * .5f;
                upperRightPosition += new float3(+1, +1, 0) * (selectionAreaMinSize - selectionAreaSize) * .5f;
                selectOnlyOneEntity = true;
            }

            // Deselect all selected Entities
            Entities.WithAll<UnitSelected>().ForEach((Entity entity) => {
                PostUpdateCommands.RemoveComponent<UnitSelected>(entity);
            });

            // Select Entities inside selection area
            int selectedEntityCount = 0;
            Entities.ForEach((Entity entity, ref Translation translation) => {
                if (selectOnlyOneEntity == false || selectedEntityCount < 1) {
                    float3 entityPosition = translation.Value;
                    if (entityPosition.x >= lowerLeftPosition.x &&
                        entityPosition.y >= lowerLeftPosition.y &&
                        entityPosition.x <= upperRightPosition.x &&
                        entityPosition.y <= upperRightPosition.y) {
                        // Entity inside selection area
                        PostUpdateCommands.AddComponent(entity, new UnitSelected());
                        selectedEntityCount++;
                    }
                }
            });
        }

        if (Input.GetMouseButtonDown(1)) {
            // Right mouse button down
            float3 targetPosition = GetMouseWorldPosition();
            List<float3> movePositionList = GetPositionListAround(targetPosition, new float[] { 10f, 20f, 30f }, new int[] { 5, 10, 20 });
            int positionIndex = 0;

            Entities.WithAll<UnitSelected>().ForEach((Entity entity, ref MoveTo moveTo) => {
                moveTo.position = movePositionList[positionIndex];
                positionIndex = (positionIndex + 1) % movePositionList.Count;
                moveTo.move = true;
            });
        }
    }
    #endregion
    private List<float3> GetPositionListAround(float3 startPosition, float[] ringDistance, int[] ringPositionCount) {
        List<float3> positionList = new List<float3>();
        positionList.Add(startPosition);
        for (int ring = 0; ring < ringPositionCount.Length; ring++) {
            List<float3> ringPositionList = GetPositionListAround(startPosition, ringDistance[ring], ringPositionCount[ring]);
            positionList.AddRange(ringPositionList);
        }
        return positionList;
    }

    private List<float3> GetPositionListAround(float3 startPosition, float distance, int positionCount) {
        List<float3> positionList = new List<float3>();
        for (int i = 0; i < positionCount; i++) {
            int angle = i * (360 / positionCount);
            float3 dir = ApplyRotationToVector(new float3(0, 1, 0), angle);
            float3 position = startPosition + dir * distance;
            positionList.Add(position);
        }
        return positionList;
    }

    private float3 ApplyRotationToVector(float3 vec, float angle) {
        return Quaternion.Euler(0, 0, angle) * vec;
    }

    // Get Mouse Position in World with Z = 0f
    public static Vector3 GetMouseWorldPosition() {
        Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main); //    Camera.main
        vec.z = 0f;
        return vec;
    }
    public static Vector3 GetMouseWorldPositionWithZ() {
        return GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
    }
    public static Vector3 GetMouseWorldPositionWithZ(Camera worldCamera) {
        return GetMouseWorldPositionWithZ(Input.mousePosition, worldCamera);
    }
    public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera) {
        Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
        return worldPosition;
    }
     */
}

public class UnitSelectedRenderer : ComponentSystem {
    
    protected override void OnUpdate() {
        Entities.WithAll<UnitSelected>().ForEach((ref Translation translation) => {
            float3 position = translation.Value + new float3(0, -3f, +5f);
            Graphics.DrawMesh(
                ECSController.instance.unitSelectedCircleMesh, 
                position,
                Quaternion.identity, 
                ECSController.instance.unitSelectedCircleMaterial, 
                0
            );
        });
    }

}

public struct MoveTo : IComponentData {
    public bool move;
    public float3 position;
    public float3 lastMoveDir;
    public float moveSpeed;
}

