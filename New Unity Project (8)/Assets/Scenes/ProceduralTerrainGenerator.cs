using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralTerrainGenerator : MonoBehaviour
{
    Vector3[] vertices;
    public int xSize = 20;
    public int zSize = 20;
    Mesh mesh;
    int[] triangles;
    public float perlinZoom = .3f;
    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        CreateShape();
        UpdateMesh();
    }

    // Update is called once per frame
    void CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * zSize + 1];
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                float y = Mathf.PerlinNoise(x * perlinZoom, z * perlinZoom) * 2f;
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }

        int vert = 0;
        int tris = 0;
        triangles = new int[xSize * zSize * 6];
        for (int z = 0; z < 1; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], .1f);
        }
    }
}
