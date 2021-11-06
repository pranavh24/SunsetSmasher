using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TerrainHeightRandomizer : MonoBehaviour
{
    public Terrain terrain;
    public Vector2[] perlinOctaves = {
        new Vector2(0.01f, 0.5f), 
        new Vector2(0.1f, 0.03f)
    };
    public float detailFrequency = .1f;
    public int detailDensity = 5;
    public int detailTreshold = 3;
    public float[] textureThresholds = {
        .2f, 1f
    };
    public TerrainLayer terrainLayer1;
    public TerrainLayer terrainLayer2;

    // Start is called before the first frame update
    private void Start() {
    }
    
    public void GenerateTerrain()
    {
        terrain = GetComponent<Terrain>();
        TerrainData TD = terrain.terrainData;
        TerrainLayer[] allLayers = {
            terrainLayer1, terrainLayer2
        };
        TD.terrainLayers = allLayers;
        float[,] heightMap = new float[TD.heightmapResolution, TD.heightmapResolution];
        float[,,] alphaMap = new float[TD.alphamapResolution, TD.alphamapResolution, TD.terrainLayers.Length];
        int[,] detailMap = new int[TD.detailResolution, TD.detailResolution];
        Vector2 perlinTranslate = new Vector2(
            TranslateCoords((int)transform.position.z, (int)TD.size.z, TD.heightmapResolution),
            TranslateCoords((int)transform.position.x, (int)TD.size.x, TD.heightmapResolution)
        );
        for (int z = 0; z < TD.heightmapResolution; z++)
        {
            for (int x = 0; x < TD.heightmapResolution; x++)
            {
                float y = 0;
                for (int octave = 0; octave < perlinOctaves.Length; octave++)
                {
                    float perlinFrequency = perlinOctaves[octave].x;
                    float perlinStrength = perlinOctaves[octave].y;
                    float offsetX = (x + perlinTranslate.x) * perlinFrequency;
                    float offsetY = (z + perlinTranslate.y) * perlinFrequency;
                    float yChange = Mathf.PerlinNoise(offsetX, offsetY) * perlinStrength;
                    y = Mathf.Min(y + yChange, 1f);
                }
                heightMap[x, z] = y;
            }
        }

        // Determine texture alphas
        for (int z = 0; z < TD.alphamapResolution; z++)
        {
            for (int x = 0; x < TD.alphamapResolution; x++)
            {
                // Get the height. 
                int heightMapX = Mathf.Clamp(TranslateCoords(x, TD.alphamapResolution, TD.heightmapResolution), 0, TD.heightmapResolution);
                int heightMapZ = Mathf.Clamp(TranslateCoords(z, TD.alphamapResolution, TD.heightmapResolution), 0, TD.heightmapResolution);
                float y = heightMap[heightMapX, heightMapZ];
                int texToUse = DetermineTexture(y);
                for (int threshID = 0; threshID < textureThresholds.Length; threshID++)
                {
                    if (texToUse == threshID)
                    {
                        // Get the texture
                        alphaMap[x, z, threshID] = 1f;
                    }
                    else
                    {
                        alphaMap[x, z, threshID] = 0f;
                    }
                }

                // if (texToUse != 0) {
                //     // Take the chance to plant a tree
                //     if (Random.value > 0.3f) {
                //         treeInstances
                //     }
                // }
            }
        }
        // Determine details
        for (int z = 0; z < TD.detailResolution; z++)
        {
            for (int x = 0; x < TD.detailResolution; x++)
            {
                int heightMapX = TranslateCoords(x, TD.detailResolution, TD.heightmapResolution);
                int heightMapZ = TranslateCoords(z, TD.detailResolution, TD.heightmapResolution);
                float perlinY = Mathf.PerlinNoise(x * detailFrequency, z * detailFrequency);
                // int y = Mathf.RoundToInt(perlinY * detailDensity);
                int y = detailDensity;
                if (y < detailTreshold || heightMap[heightMapX, heightMapZ] < textureThresholds[0]) y = 0;
                detailMap[x, z] = y;
            }
        }

        // Determine trees
        // treeInstances = new TreeInstance[50000];
        // PoissonDiscSampler discSampler = new PoissonDiscSampler(1f, 1f, 1 / Mathf.Sqrt(treeInstances.Length));
        // int count = 0;

        // foreach (Vector2 sample in discSampler.Samples())
        // {
        //     int heightMapX = Mathf.RoundToInt(sample.x * TD.heightmapResolution);
        //     int heightMapZ = Mathf.RoundToInt(sample.y * TD.heightmapResolution);
        //     if (heightMapX >= TD.heightmapResolution || heightMapZ >= TD.heightmapResolution) continue;
        //     if (sample.x > 1f || sample.x < 0f || sample.y > 1f || sample.y < 0f) continue;
        //     if (heightMapX < 0 || heightMapZ < 0) continue;
        //     if (heightMap[heightMapX, heightMapZ] < textureThresholds[0]) continue;
        //     TreeInstance instance = new TreeInstance();
        //     instance.position = new Vector3(sample.x, 0, sample.y);
        //     instance.prototypeIndex = Random.value < 0.3f ? 1 : 0;
        //     instance.widthScale = Random.Range(1f, 2f);
        //     instance.heightScale = instance.widthScale;
        //     instance.color = Color.white;
        //     instance.lightmapColor = Color.white;
        //     treeInstances[count] = instance;
        //     count++;
        //     if (count >= treeInstances.Length) break;
        // }

        TD.SetHeights(0, 0, heightMap);
        TD.SetAlphamaps(0, 0, alphaMap);
        TD.SetDetailLayer(0, 0, 0, detailMap);
        terrain.Flush();
    }

    public void FilterTrees()
    {
        List<TreeInstance> treeInstances = new List<TreeInstance>(terrain.terrainData.treeInstances);
        for (int i = 0; i < treeInstances.Count; i++)
        {
            if (treeInstances[i].position.y < textureThresholds[0])
            {
                treeInstances.RemoveAt(i);
                i--;
            }
        }
        
        terrain.terrainData.SetTreeInstances(treeInstances.ToArray(), true);
    }

    TreeInstance CreateTree(Vector2 position)
    {
        TreeInstance instance = new TreeInstance();
        instance.position = new Vector3(position.x, 0, position.y);
        instance.prototypeIndex = Random.value < 0.3f ? 1 : 0;
        instance.widthScale = Random.Range(1f, 2f);
        instance.heightScale = instance.widthScale;
        instance.color = Color.white;
        instance.lightmapColor = Color.white;
        return instance;
    }
    public TreeInstance[] treeInstances;
    int TranslateCoords(int x, int resolutionFrom, int resolutionTo)
    {
        return Mathf.RoundToInt(((float)x / resolutionFrom) * resolutionTo);
    }

    int DetermineTexture(float height)
    {
        for (int i = 0; i < textureThresholds.Length; i++)
        {
            if (height < textureThresholds[i])
            {
                return i;
            }
        }
        return 0;
    }
}
