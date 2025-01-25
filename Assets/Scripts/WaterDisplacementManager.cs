using UnityEngine;
using System.Linq;
using Unity.Mathematics;

public class WaterDisplacementManager : MonoBehaviour
{
    private const int NUM_WAVES = 8; // Match this with the shader
    private Material material;

    private static float[] amplitudes;
    private static Vector2[] directions;
    private static float[] frequencies;
    private static float[] speeds;


    //Generate random number between min and max, but excluding the range midClampLow to midClampHigh
    float generateRandomNum(float min, float max, float midClampLow, float midClampHigh)
    {
        float randomValue;
        do
        {
            randomValue = UnityEngine.Random.Range(min, max);
        } while (randomValue >= midClampLow && randomValue <= midClampHigh);
        return randomValue;
    }


    void OnEnable()
    {
        // Get the material instance
        material = GetComponent<MeshRenderer>().sharedMaterial;
        InitialiseWaveParameters();
        UpdateShaderParameters();
    }

    void InitialiseWaveParameters()
    {
        amplitudes = new float[NUM_WAVES];
        directions = new Vector2[NUM_WAVES];
        frequencies = new float[NUM_WAVES];
        speeds = new float[NUM_WAVES];

        float scale = Mathf.Max(transform.lossyScale.x, transform.lossyScale.z);

        Vector2 startAmplitudeRange = new Vector2(0.4f, 0.8f) * scale;
        Vector2 startFrequencyRange = new Vector2(2.5f, 5.0f) / scale;
        Vector2 startSpeedRange = new Vector2(-1.2f, 1.2f);
        int wavesPerIteration = 1;
        float amplitudeFalloff = 0.83f;
        float frequencyFalloff = 1.18f;
        float speedFalloff = 1.075f;
        Vector2 amplitudeRange = startAmplitudeRange;
        Vector2 frequencyRange = startFrequencyRange;
        Vector2 speedRange = startSpeedRange;
        for (int i = 0; i < NUM_WAVES; ++i) {
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
        material.SetFloatArray("_Amplitudes", amplitudes);
        material.SetVectorArray("_Directions", directions.Select(d => new Vector4(d.x, d.y, 0, 0)).ToArray());
        material.SetFloatArray("_Frequencies", frequencies);
        material.SetFloatArray("_Speeds", speeds);
        material.SetFloat("_CPUTime", Time.time);
    }

    void Update()
    {
        material.SetFloat("_CPUTime", Time.time);
    }
}