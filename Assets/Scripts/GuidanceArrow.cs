using UnityEngine;
using UnityEngine.SceneManagement;

public class GuidanceArrow : MonoBehaviour
{
    [SerializeField] DuckController duck;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        transform.LookAt(CheckpointHandler.GetNearestCheckpoint(duck.NextCheckpointIndex, duck.transform.position));
    }
}
