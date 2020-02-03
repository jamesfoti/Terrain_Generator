using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerrainGenerator : MonoBehaviour {
    public enum DrawMode {NoiseMap, ColorMap, Mesh}
    public DrawMode drawMode;
    public int mapWidth;
    public int mapHeight;
    public float noiseScale;
    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;
    public int seed;
    private int defualtSeed;
    public Vector2 offset;
    private Vector2 defualtOffset;
    public float meshHeightMultiplier;
    public AnimationCurve heightCurve;
 
    private NoiseMapGenerator noiseMapGenerator;
    private TextureGenerator textureGenerator;
    private DisplayMap displayMap;
    private MeshGenerator meshGenerator;

    private float[,] noiseMap;
    public TerrainTypes[] regions;

    private TerrainGeneratorUI terrainGeneratorUI;
    private string dropdownValue;
    private bool animate = false;

    private List<GameObject> cubes;

    private void Start() {
        cubes = new List<GameObject>();

        defualtOffset.x = Random.Range(-1000, 1000);
        defualtOffset.y = Random.Range(-1000, 1000);
        offset = defualtOffset;

        defualtSeed = Random.Range(-1000, 1000);
        seed = defualtSeed;

        GenerateTerrain();

        terrainGeneratorUI = FindObjectOfType<TerrainGeneratorUI>();
        terrainGeneratorUI.noiseScaleSlider.value = noiseScale;
        terrainGeneratorUI.octavesSlider.value = octaves;
        terrainGeneratorUI.persistenceSlider.value = persistance;
        terrainGeneratorUI.lacunaritySlider.value = lacunarity;
        terrainGeneratorUI.heightMultiplierSlider.value = meshHeightMultiplier;
        terrainGeneratorUI.seedSlider.value = seed;
        terrainGeneratorUI.offsetX.value = offset.x;
        terrainGeneratorUI.offsetY.value = offset.y;
    }

    private void Update() {
        noiseScale = terrainGeneratorUI.noiseScaleSlider.value;
        octaves = (int)terrainGeneratorUI.octavesSlider.value;
        persistance = terrainGeneratorUI.persistenceSlider.value;
        lacunarity = terrainGeneratorUI.lacunaritySlider.value;
        meshHeightMultiplier = terrainGeneratorUI.heightMultiplierSlider.value;
        seed = (int)terrainGeneratorUI.seedSlider.value;

        int dropdownIndex = terrainGeneratorUI.animationsDropdown.value;
        dropdownValue = terrainGeneratorUI.animationsDropdown.options[dropdownIndex].text;

        if (animate) {
            GenerateTerrain();
            offset.x += Time.deltaTime * 2f;        
        }
        else {
            offset = new Vector2(terrainGeneratorUI.offsetX.value, terrainGeneratorUI.offsetY.value) / 100f;
        }
    }

    public void AnimateTerrain() {
        Debug.Log("Animate!");
        animate = true;
    }

    public void PauseAnimation() {
        Debug.Log("Pause Animations!");
        animate = false;
    }

    public void DropCube() {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.localScale = new Vector3(50f, 50f, 50f);
        cube.transform.position = new Vector3(0, 250f, 0);

        Rigidbody rgb3d = cube.AddComponent<Rigidbody>();
        rgb3d.velocity = new Vector3(0, -150f, 0);

        cubes.Add(cube);
    }

    public void GenerateTerrain() { 
        noiseMapGenerator = new NoiseMapGenerator();
        displayMap = FindObjectOfType<DisplayMap>();
        textureGenerator = new TextureGenerator();
        meshGenerator = new MeshGenerator();

        noiseMap = noiseMapGenerator.GenerateNoiseMap(mapWidth, mapHeight, noiseScale, octaves, persistance, lacunarity, seed, offset, animate, dropdownValue);

        Color[] colorMap = new Color[mapWidth * mapHeight];
        for (int y = 0; y < mapHeight; y++) {
            for (int x = 0; x < mapWidth; x++) {
                float currHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++) {
                    if (currHeight <= regions[i].height) {
                        colorMap[y * mapWidth + x] = regions[i].color;
                        break;
                    }
                }
            }
        }

        if (drawMode == DrawMode.NoiseMap) {
            displayMap.DrawTexture(textureGenerator.TextureFromNoiseMap(noiseMap));
        }
        else if (drawMode == DrawMode.ColorMap) {
            displayMap.DrawTexture(textureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight));
        }
        else if (drawMode == DrawMode.Mesh) {
            displayMap.DrawMesh(meshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, heightCurve), textureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight));
        }
    }

    public void RandomizeTerrain() {
        seed = Random.Range(-1000, 1000);
        offset.x = Random.Range(-1000, 1000);
        offset.y = Random.Range(-1000, 1000);
        GenerateTerrain();
    }

    public void ResetTerrain() {
        mapWidth = 100;
        mapHeight = 100;
        noiseScale = 25;
        octaves = 5;
        persistance = .5f;
        lacunarity = 2f;
        seed = defualtSeed;
        offset = defualtOffset;
        meshHeightMultiplier = 12f;

        Keyframe waterKeyFrame = new Keyframe(.352f, .127f);
        waterKeyFrame.outTangent = 0f; 
        heightCurve.AddKey(waterKeyFrame);

        terrainGeneratorUI = FindObjectOfType<TerrainGeneratorUI>();
        terrainGeneratorUI.noiseScaleSlider.value = noiseScale;
        terrainGeneratorUI.octavesSlider.value = octaves;
        terrainGeneratorUI.persistenceSlider.value = persistance;
        terrainGeneratorUI.lacunaritySlider.value = lacunarity;
        terrainGeneratorUI.seedSlider.value = seed;
        terrainGeneratorUI.offsetX.value = offset.x;
        terrainGeneratorUI.offsetY.value = offset.y;
        terrainGeneratorUI.heightMultiplierSlider.value = meshHeightMultiplier;

        animate = false;

        for (int i = 0; i < cubes.Count; i++) {
            if (cubes[i] != null) {
                Destroy(cubes[i]);
            }
        }

        GenerateTerrain();
    }

    private void OnValidate() {
        GenerateTerrain();
    }
}

[System.Serializable]
public struct TerrainTypes {
    public string name;
    public float height;
    public Color color;
}