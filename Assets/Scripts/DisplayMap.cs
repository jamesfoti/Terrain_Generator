using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayMap : MonoBehaviour {
    
    public Renderer textureRender;
    public MeshFilter meshFilter;
    public MeshRenderer meshRender;
    public MeshCollider meshCollider;

    public float power = 1.5f;
    public float scale = 2;
    public float timeScale = .02f;

    private float xOffset;
    private float yOffset;

    public TerrainGenerator terrainGenerator;


    public void DrawTexture(Texture2D texture) {
        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    public void DrawMesh(MeshData meshData, Texture2D texture) {
        Mesh mesh = meshData.CreateMesh();

        //Updating the collider causes the program to slow down a lot! Be aware!
        meshCollider.sharedMesh = mesh;
        meshCollider.enabled = false;
        meshCollider.enabled = true;

        meshFilter.sharedMesh = mesh;
        meshRender.sharedMaterial.mainTexture = texture;
    }

    public void WaterAnimations() {
        Vector3[] vertices = meshFilter.mesh.vertices;

        for (int i = 0; i < vertices.Length; i++) {
            if (vertices[i].y <= .14 * terrainGenerator.meshHeightMultiplier) { // .4 * max water height
                vertices[i].y = CalculateWaterHeight(vertices[i].x, vertices[i].z) * power;
            }
        }

        meshFilter.mesh.vertices = vertices;
    }

    private float CalculateWaterHeight(float x, float y) {
        float xCord = x * scale * xOffset;
        float yCord = y * scale * yOffset;

        return Mathf.PerlinNoise(xCord, yCord);
    }

    private void Update() {
        WaterAnimations();
        xOffset += Time.deltaTime * timeScale;
        yOffset += Time.deltaTime * timeScale;
    }
}