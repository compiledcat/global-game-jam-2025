using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class RemoveCheckpoints : MonoBehaviour
{
    [SerializeField] DuckController duckController;
    new Camera camera;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            camera = GetComponent<Camera>();
            RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
            RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
        }
    }

    void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        if (camera != this.camera) return;

        foreach (var cp in CheckpointHandler.checkpoints[duckController.NextCheckpointIndex])
        {
            cp.meshRenderer1.enabled = true;
            cp.meshRenderer2.enabled = true;
        }
    }

    void OnEndCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        if (camera != this.camera) return;

        foreach (var cp in CheckpointHandler.checkpoints[duckController.NextCheckpointIndex])
        {
            cp.meshRenderer1.enabled = false;
            cp.meshRenderer2.enabled = false;
        }
    }
}
