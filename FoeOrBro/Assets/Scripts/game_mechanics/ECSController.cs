
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Burst;
using System.Threading;

public class ECSController : MonoBehaviour {

    public static ECSController instance;

    public Transform selectionAreaTransform;
    public Material unitSelectedCircleMaterial;
    public Mesh unitSelectedCircleMesh;

    private EntityManager entityManager;

    
    private void Awake() {
        instance = this;
    }

    
    private void Start() {
        //entityManager = World.Active.EntityManager;
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        //unitSelectedCircleMesh = ECS_Animation.CreateMesh(8f, 5f);
    }




}