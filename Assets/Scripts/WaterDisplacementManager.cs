using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class WaterDisplacementManager : MonoBehaviour
{
    private const int NUM_WAVES = 8; // Match this with the shader
    private Material material;

    private static float[] amplitudes;
    private static Vector2[] directions;
    private static float[] frequencies;
    private static float[] speeds;
    private static readonly int AmplitudesID = Shader.PropertyToID("_Amplitudes");
    private static readonly int DirectionsID = Shader.PropertyToID("_Directions");
    private static readonly int FrequenciesID = Shader.PropertyToID("_Frequencies");
    private static readonly int SpeedsID = Shader.PropertyToID("_Speeds");
    private static readonly int CPUTimeID = Shader.PropertyToID("_CPUTime");


    //Generate random number between min and max, but excluding the range midClampLow to midClampHigh
    float generateRandomNum(float min, float max, float midClampLow, float midClampHigh)
    {
        float randomValue;
        do
        {
            randomValue = Random.Range(min, max);
        } while (randomValue >= midClampLow && randomValue <= midClampHigh);

        return randomValue;
    }

    private void OnValidate()
    {
        GeneratePlane();
    }

    void Start()
    {
        // Get the material instance
        if (!material)
        {
            material = GetComponent<MeshRenderer>().material;
        }

        InitialiseWaveParameters();
        UpdateShaderParameters();
        GeneratePlane(); // apparently unity isn't saving the mesh to the asset so i guess we're doing this too
    }

    [SerializeField] private int pointsAlongX;
    [SerializeField] private int pointsAlongZ;

    [ContextMenu("Generate Plane")]
    public void GeneratePlane()
    {
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        mesh.name = $"Subdivided plane {pointsAlongX}x{pointsAlongZ}";
        List<Vector3> vertices = new List<Vector3>();
        List<int> tris = new List<int>();

        //Vertex data
        float subdivisionIncrementX = 1.0f / (pointsAlongX - 1); //Distance between points along x axis
        float subdivisionIncrementZ = 1.0f / (pointsAlongZ - 1); //Distance between points along z axis

        Vector2 topLeft = new Vector2(-0.5f, 0.5f);

        for (int i = 0; i < pointsAlongZ; ++i)
        {
            float z = topLeft.y - (i * subdivisionIncrementZ);
            for (int j = 0; j < pointsAlongX; ++j)
            {
                float x = topLeft.x + (j * subdivisionIncrementX);
                vertices.Add(new Vector3(x, 0.0f, z));
            }
        }

        //Indices data
        for (int z = 0; z < pointsAlongZ - 1; ++z)
        {
            for (int x = 0; x < pointsAlongX - 1; ++x)
            {
                //First triangle
                tris.Add((z * pointsAlongX) + x + 1);
                tris.Add((z * pointsAlongX) + x + pointsAlongX);
                tris.Add((z * pointsAlongX) + x);

                //Second triangle
                tris.Add((z * pointsAlongX) + x + pointsAlongX + 1);
                tris.Add((z * pointsAlongX) + x + pointsAlongX);
                tris.Add((z * pointsAlongX) + x + 1);
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.Optimize();
    }

    void InitialiseWaveParameters()
    {
        amplitudes = new float[NUM_WAVES];
        directions = new Vector2[NUM_WAVES];
        frequencies = new float[NUM_WAVES];
        speeds = new float[NUM_WAVES];

        float scale = Mathf.Max(transform.lossyScale.x, transform.lossyScale.z);

        Vector2 startAmplitudeRange = new Vector2(0.0004f, 0.0008f) * scale;
        Vector2 startFrequencyRange = new Vector2(25f, 50f) / scale;
        Vector2 startSpeedRange = new Vector2(-1.2f, 1.2f);
        int wavesPerIteration = 1;
        float amplitudeFalloff = 0.83f;
        float frequencyFalloff = 1.18f;
        float speedFalloff = 1.075f;
        Vector2 amplitudeRange = startAmplitudeRange;
        Vector2 frequencyRange = startFrequencyRange;
        Vector2 speedRange = startSpeedRange;
        for (int i = 0; i < NUM_WAVES; ++i)
        {
            amplitudes[i] = generateRandomNum(amplitudeRange.x, amplitudeRange.y, 0, 0);
            frequencies[i] = generateRandomNum(frequencyRange.x, frequencyRange.y, 0, 0);
            directions[i] = new Vector2(generateRandomNum(-1, 1, 0, 0), generateRandomNum(-1, 1, 0, 0)).normalized;
            speeds[i] = generateRandomNum(speedRange.x, speedRange.y, 0, 0);

            if ((i % wavesPerIteration == 0) && (i > 0))
            {
                amplitudeRange *= amplitudeFalloff;
                frequencyRange *= frequencyFalloff;
                speedRange *= speedFalloff;
            }
        }
    }

    public float GetHeightAtPosition(Vector2 xz)
    {
        //Calculate wave height using sum of sines
        float height = 0.0f;
        for (int i = 0; i < NUM_WAVES; i++)
        {
            Vector2 dir = directions[i];
            float freq = frequencies[i];
            float speed = speeds[i];
            float amp = amplitudes[i];
            float time = Time.time;

            float phase = Vector2.Dot(dir, xz) * freq + time * speed;
            height += amp * Mathf.Sin(phase);
        }

        return height;
    }

    void UpdateShaderParameters()
    {
        material.SetFloatArray(AmplitudesID, amplitudes);
        material.SetVectorArray(DirectionsID, directions.Select(d => new Vector4(d.x, d.y, 0, 0)).ToArray());
        material.SetFloatArray(FrequenciesID, frequencies);
        material.SetFloatArray(SpeedsID, speeds);
        material.SetFloat(CPUTimeID, Time.time);
    }

    void Update()
    {
        UpdateShaderParameters();
    }
}