using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayMap : MonoBehaviour {
    
    public Renderer textureRender;
    public MeshFilter meshFilter;
    public MeshRenderer meshRender;
    public MeshCollider meshCollider;

    public void DrawTexture(Texture2D texture) {
        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    public void DrawMesh(MeshData meshData, Texture2D texture) {
        Mesh mesh = meshData.CreateMesh();

        /* Updating the collider causes the program to slow down a lot! Be aware!
         * meshCollider.sharedMesh = mesh;
         * meshCollider.enabled = false;
         * meshCollider.enabled = true;
        */

        meshFilter.sharedMesh = mesh;
        meshRender.sharedMaterial.mainTexture = texture;
    }
}