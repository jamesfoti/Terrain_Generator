/* SOURCES:
 * Procedural Landmass Generation (E02: Noise Map) by Sebastian Lague:
 * https://www.youtube.com/watch?v=WP-Bm65Q-1Y&list=PLFt_AvWsXl0eBW2EiBtl_sxmDtSgZBxB3&index=2
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NoiseMapGenerator {

    public Renderer textureRender;

    public float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float noiseScale, int octaves, float persistance, float lacunarity, int seed, Vector2 offset, bool animate, string trig) {

        if (mapWidth <= 0) {
            mapWidth = 1;
        }

        if (mapHeight <= 0) {
            mapHeight = 1;
        }

        if (noiseScale <= 0) {
            noiseScale = .0001f;
        }

        float[,] noiseMap = new float[mapWidth, mapHeight];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffSets = new Vector2[octaves];

        for (int i = 0; i < octaves; i++) {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffSets[i] = new Vector2(offsetX, offsetY);
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        for (int y = 0; y < mapHeight; y++) {
            for (int x = 0; x < mapWidth; x++) {

                float amplitude = 1f;
                float frequency = 1f;
                float noiseHeight = 0f;

                for (int i = 0; i < octaves; i++) {
                    float xCoord = (x - halfWidth) / noiseScale * frequency + octaveOffSets[i].x;
                    float yCoord = (y - halfHeight) / noiseScale * frequency + octaveOffSets[i].y;

                    if (animate) {
                        if (trig == "sin(x)") {
                            noiseHeight = Mathf.Sin(xCoord);
                        }
                        else if (trig == "cos(x)") {
                            noiseHeight = Mathf.Cos(xCoord);
                        }
                        else if (trig == "tan(x)") {
                            noiseHeight = Mathf.Tan(xCoord);
                        }
 
                        else {
                            float perlinValue = Mathf.PerlinNoise(xCoord, yCoord) * 2 - 1;
                            noiseHeight += perlinValue * amplitude;
                        }
                    }

                    else {
                        float perlinValue = Mathf.PerlinNoise(xCoord, yCoord) * 2 - 1;
                        noiseHeight += perlinValue * amplitude;
                    }
                             
                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight) {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight) {
                    minNoiseHeight = noiseHeight;
                }

                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < mapHeight; y++) {
            for (int x = 0; x < mapWidth; x++) {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }
        return noiseMap;
    }

}