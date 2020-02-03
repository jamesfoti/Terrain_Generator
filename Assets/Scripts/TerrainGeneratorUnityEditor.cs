using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/* SOURCES: 
 * Adding Buttons to a Custom Inspector - Unity Official Tutorials:
 * https://www.youtube.com/watch?time_continue=53&v=_fNgn3Arpoo&feature=emb_title
 */

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorUnityEditor : Editor {
    public override void OnInspectorGUI() {
        TerrainGenerator terrainGenerator = (TerrainGenerator)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Randomize Terrain")) {
            terrainGenerator.RandomizeTerrain();
        }

        if (GUILayout.Button("Reset Terrain")) {
            terrainGenerator.ResetTerrain();
            terrainGenerator.GenerateTerrain();
        }
    }

}
