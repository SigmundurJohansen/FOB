using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNode {

    private Grid<GridNode> grid;
    private int x;
    private int y;
    private Mesh mesh;
    private Texture2D texture;
    private Material material;

    private bool isWalkable;
    
    public GridNode(Grid<GridNode> grid, int x, int y) {
        this.grid = grid;
        this.x = x;
        this.y = y;
        isWalkable = true;
    }

    public bool IsWalkable() {
        return isWalkable;
    }

    public void SetIsWalkable(bool isWalkable) {
        this.isWalkable = isWalkable;
        grid.TriggerGridObjectChanged(x, y);
    }

    public void SetNodeMesh(Mesh _mesh)
    {
        mesh = _mesh;
    }
    public Mesh GetNodeMesh()
    {
        return mesh;
    }
    public void SetNodeTexture(Texture2D _texture)
    {
        texture = _texture;
        if(material!=null){
            string name = "x " + this.x.ToString() + " , y " + this.y.ToString();
            material.SetTexture(name, _texture);
        }
    }
    public Texture2D GetNodeTexture()
    {
        return texture;
    }
    public Material GetNodeMaterial()
    {
        if(material!=null){
            return material;
        }
        string name = "x " + this.x.ToString() + " , y " + this.y.ToString();
        material = new Material(Shader.Find("Unlit/Texture"));
        material.SetTexture(name, texture);
        return material;
    }
}
