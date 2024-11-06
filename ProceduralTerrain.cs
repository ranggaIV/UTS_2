using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralTerrain : MonoBehaviour
{
    public int width = 256;
    public int depth = 256;
    public float scale = 20f;
    public float height = 20f;

    private Mesh mesh;
    private Vector3[] vertices;

    void Start()
    {
        GenerateTerrain();
    }

    void Update()
    {
        if (Input.GetMouseButton(0)) // Klik kiri untuk mengubah tinggi terrain
        {
            ModifyTerrain();
        }
    }

    void GenerateTerrain()
    {
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }

        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.material = new Material(Shader.Find("Standard"));
        }

        mesh = new Mesh();
        vertices = new Vector3[(width + 1) * (depth + 1)];

        for (int i = 0, z = 0; z <= depth; z++)
        {
            for (int x = 0; x <= width; x++)
            {
                float y = Mathf.PerlinNoise(x * scale * 0.1f, z * scale * 0.1f) * height;
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }
        
        mesh.vertices = vertices;

        int[] triangles = new int[width * depth * 6];
        for (int z = 0, vert = 0, tris = 0; z < depth; z++, vert++)
        {
            for (int x = 0; x < width; x++, vert++, tris += 6)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + width + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + width + 1;
                triangles[tris + 5] = vert + width + 2;
            }
        }

        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }

    void ModifyTerrain()
    {
        // Melakukan raycasting untuk mendapatkan titik di terrain
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Dapatkan posisi hit pada mesh
            Vector3 hitPoint = hit.point;

            // Konversi posisi hit ke koordinat vertex
            int x = Mathf.RoundToInt(hitPoint.x);
            int z = Mathf.RoundToInt(hitPoint.z);

            if (x >= 0 && x <= width && z >= 0 && z <= depth)
            {
                // Cari index vertex yang sesuai
                int index = z * (width + 1) + x;

                // Ubah tinggi vertex di area yang di-klik
                vertices[index].y += 10f; // Tambah tinggi terrain

                // Terapkan perubahan ke mesh
                mesh.vertices = vertices;
                mesh.RecalculateNormals();
            }
        }
    }
}
