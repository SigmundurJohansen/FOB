using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public enum ClickState
{
    Selecting,
    Moving,
    Targeting,
    Area
}

public class MoveOrderSystem : ComponentSystem
{

    Vector2 pointOne;
    Vector2 pointTwo;

    private ClickState CurrentState = ClickState.Selecting;

    public Vector3 GetWorldPosition(int _x, int _y) {
        return new Vector3(_x, _y) * 0.32f;  //   new Vector3(x, y) * cellSize + originPosition
    }
    protected override void OnUpdate()
    {
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            SetTargetingState();
            if (EventSystem.current.IsPointerOverGameObject())
            {
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    pointOne = WorldPosition();
                }
                if (Input.GetMouseButtonUp(0))
                {
                    pointTwo = WorldPosition();
                    if ((pointOne - pointTwo).magnitude > 0.3f)
                    {
                        CurrentState = ClickState.Selecting;
                        Deselect();
                    }

                    if (CurrentState == ClickState.Area)
                    {

                    }

                    if (CurrentState == ClickState.Moving)
                    {
                        //float cellSize = PathfindingGridSetup.Instance.pathfindingGrid.GetCellSize();
                        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
                        PathfindingGridSetup.Instance.pathfindingGrid.GetXY(mousePosition, out int endX, out int endY); //  + new Vector3(1,1,0)* cellSize
                        ValidateGridPosition(ref endX, ref endY);
                        if (PathfindingGridSetup.Instance.pathfindingGrid.GetGridObject(endX, endY).IsWalkable())
                        {
                            Entities.ForEach((Entity entity, DynamicBuffer<PathPosition> pathPositionBuffer, ref Translation translation, ref Selected _selected, ref MovementComponent _move) =>
                            {
                                if (_selected.isSelected)
                                {
                                    PathfindingGridSetup.Instance.pathfindingGrid.GetXY(translation.Value, out int startX, out int startY);
                                    ValidateGridPosition(ref startX, ref startY);
                                    _move.isMoving = true;
                                    _move.chaseTarget = false;
                                    //Debug.Log("dechasing");
                                    PostUpdateCommands.RemoveComponent(entity, typeof(HasTarget));
                                    //Debug.Log("remove Entity hastarget"); ref OrdersComponent _orders,
                                    //_orders.hasOrders = true;
                                    EntityManager.AddComponentData(entity, new DestinationComponent { startPosition = new int2(startX, startY), endPosition = new int2(endX, endY) });
                                    EntityManager.AddComponentData(entity, new OrderComponent { hasOrders = true, orderType = 1 });
                                }
                            });
                        }
                    }
                    if (CurrentState == ClickState.Selecting)
                    {
                        if ((pointOne - pointTwo).magnitude < 0.2f)
                        {
                            bool isContinue = true;
                            Entities.ForEach((Entity entity, ref Translation _translation, ref Selected _selected) =>
                            {
                                if (_translation.Value.x > (pointTwo.x - 0.2f) && _translation.Value.x < (pointTwo.x + 0.2f) && _translation.Value.y > (pointTwo.y - 0.2f) && _translation.Value.y < (pointTwo.y + 0.2f))
                                {
                                    if (isContinue)
                                    {
                                        _selected.isSelected = true;
                                        CurrentState = ClickState.Moving;
                                        isContinue = false;
                                    }
                                }
                            });
                        }
                        if ((pointOne - pointTwo).magnitude > 0.3f)
                        {
                            bool isSelected = false;
                            Entities.ForEach((Entity entity, ref Translation _translation, ref Selected _selected) =>
                            {
                                _selected.isSelected = false;
                                if (_translation.Value.x > pointOne.x && _translation.Value.x < pointTwo.x && _translation.Value.y > pointOne.y && _translation.Value.y < pointTwo.y)
                                {
                                    _selected.isSelected = true;
                                    isSelected = true;
                                }
                                if (_translation.Value.x < pointOne.x && _translation.Value.x > pointTwo.x && _translation.Value.y < pointOne.y && _translation.Value.y > pointTwo.y)
                                {
                                    _selected.isSelected = true;
                                    isSelected = true;
                                }
                                if (_translation.Value.x > pointOne.x && _translation.Value.x < pointTwo.x && _translation.Value.y < pointOne.y && _translation.Value.y > pointTwo.y)
                                {
                                    _selected.isSelected = true;
                                    isSelected = true;
                                }
                                if (_translation.Value.x < pointOne.x && _translation.Value.x > pointTwo.x && _translation.Value.y > pointOne.y && _translation.Value.y < pointTwo.y)
                                {
                                    _selected.isSelected = true;
                                    isSelected = true;
                                }
                            });
                            if (isSelected)
                                CurrentState = ClickState.Moving;
                            else
                                CurrentState = ClickState.Selecting;
                        }
                    }

                    if (CurrentState == ClickState.Targeting)
                    {
                        Debug.Log("attacking system");
                        // none player units adding HasTarget  // .WithNone<Player>()
                        Entities.WithNone<HasTarget>().ForEach((Entity entity, ref Translation _translation, ref Selected _selected, ref AttackComponent _attack, ref MovementComponent _move) =>
                        {
                            if (_selected.isSelected == true)
                            {

                                int setAttack = 0;
                                Entities.ForEach((Entity _targetEntity, ref Translation _targetTranslation, ref TargetableComponent _targetable) =>
                                {
                                    if (_targetTranslation.Value.x > (pointOne.x - 0.2f) && _targetTranslation.Value.x < (pointOne.x + 0.2f)
                                    && _targetTranslation.Value.y > (pointOne.y - 0.2f) && _targetTranslation.Value.y < (pointOne.y + 0.2f))
                                    {
                                        EntityManager.AddComponentData(entity, new HasTarget { targetEntity = _targetEntity });
                                        setAttack = 1;
                                        Debug.Log("found target");
                                    }
                                });
                                if (setAttack == 1)
                                {
                                    _attack.isAttacking = true;
                                    _move.chaseTarget = true;
                                }
                                else
                                {
                                    Debug.Log("no target found");
                                    _move.chaseTarget = false;
                                    _attack.isAttacking = false;
                                }
                            }
                        });
                        GameController.Instance.SetAttackState(false);
                        CurrentState = ClickState.Moving;
                    }
                }
                if (Input.GetMouseButtonDown(1))
                {
                    Deselect();
                }
            }
        }
    }
    public void SetTargetingState()
    {
        if (GameController.Instance.GetAttackState())
        {
            CurrentState = ClickState.Targeting;
            Debug.Log("state targeting");
        }
    }

    public void Deselect()
    {
        Entities.ForEach((Entity entity, ref Translation _translation, ref Selected _selected) =>
        {
            _selected.isSelected = false;
            CurrentState = ClickState.Selecting;
        });
    }

    public Vector3 WorldPosition()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 20f));
        return mousePos;
    }
    private void ValidateGridPosition(ref int x, ref int y)
    {
        x = math.clamp(x, 0, PathfindingGridSetup.Instance.pathfindingGrid.GetWidth() - 1);
        y = math.clamp(y, 0, PathfindingGridSetup.Instance.pathfindingGrid.GetHeight() - 1);
    }
}