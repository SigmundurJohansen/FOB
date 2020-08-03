using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SF = UnityEngine.SerializeField;

public class PathfindingVisual : MonoBehaviour {

    private Grid<GridNode> grid;
    private Mesh mesh;
    private bool updateMesh;
    public float scale;
    //[SF] float RotationX;
    //[SF] float RotationY;
    //[SF] float RotationZ;

    private void Awake() {
        //mesh = new Mesh();
        mesh = GetComponent<MeshFilter>().mesh;
    }



    public void SetGrid(Grid<GridNode> grid) {
        this.grid = grid;
        UpdateVisual();

        grid.OnGridObjectChanged += Grid_OnGridValueChanged;
    }

    private void Grid_OnGridValueChanged(object sender, Grid<GridNode>.OnGridObjectChangedEventArgs e) {
        updateMesh = true;
    }

    private void LateUpdate() {
        if (updateMesh) {
            updateMesh = false;
            UpdateVisual();
        }
    }

    private Color CalculateColor (int x, int y)
    {
        float xCoord = (float) x / 32 * scale;
        float yCoord = (float) y / 32 * scale;

        float sample = Mathf.PerlinNoise(xCoord, yCoord);
        return new Color(sample, sample, sample);
    }

    private Texture2D GenerateTexture()
    {
        Texture2D texture = new Texture2D(32,32);
        for(int x = 0; x < 32; x++){
            for(int y = 0; y < 32; y++)
            {
                Color color = CalculateColor(x,y);
                texture.SetPixel(x,y, color);
            }
        }
        texture.Apply();
        return texture;
    }

    private void UpdateVisual() {
        //RaycastHit2D rayHit = Physics2D.Raycast(new Vector3(0,0,0), Camera.main.transform.position, 10f, layerMask);
        // -rayHit.normal
        //Quaternion rotation = Quaternion.Euler(0, 30, 0);
        //Vector3 rotation = new Vector3(RotationX, RotationY, RotationZ);
        
        for (int x = 0; x < grid.GetWidth(); x++) {
            for (int y = 0; y < grid.GetHeight(); y++) {
                int index = x * grid.GetHeight() + y;
                Mesh nodeMesh = new Mesh();
                Vector3 meshPos = new Vector3 (x,y, 0);
                Vector3 quadSize = new Vector3(1, 1) * grid.GetCellSize();
                GridNode gridNode = grid.GetGridObject(x, y);                
                Vector2 uv00 = new Vector2(0, 0);
                Vector2 uv11 = new Vector2(grid.GetCellSize(), grid.GetCellSize());
                //Vector2 uv11 = new Vector2(1f, 1f);
                if (!gridNode.IsWalkable()) {
                    //uv00 = new Vector2(1f, 1f);
                    //uv11 = new Vector2(0f, 0f);
                    //gridNode.SetIsWalkable(false);
                }
                nodeMesh = MeshUtils.CreateMesh(meshPos, 0f, quadSize, uv00, uv11);
                gridNode.SetNodeMesh(nodeMesh);
                gridNode.SetNodeTexture(GenerateTexture());
            }
        }
    }
}










/*
        MeshUtils.CreateEmptyMeshArrays(grid.GetWidth() * grid.GetHeight(), out Vector3[] vertices, out Vector2[] uv, out int[] triangles);
        for (int x = 0; x < grid.GetWidth(); x++) {
            for (int y = 0; y < grid.GetHeight(); y++) {
                int index = x * grid.GetHeight() + y;
                Vector3 quadSize = new Vector3(1, 1) * grid.GetCellSize();

                GridNode gridNode = grid.GetGridObject(x, y);
                
                Vector2 uv00 = new Vector2(0, 0);
                Vector2 uv11 = new Vector2(grid.GetCellSize(), grid.GetCellSize());
                //Vector2 uv11 = new Vector2(1f, 1f);

                if (!gridNode.IsWalkable()) {
                    uv00 = new Vector2(1f, 1f);
                    uv11 = new Vector2(0f, 0f);
                }

                MeshUtils.AddToMeshArrays(vertices, uv, triangles, index, new Vector3(x, y) * grid.GetCellSize(), 0f, quadSize, uv00, uv11);
            }
        }
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }
     */