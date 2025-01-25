using UnityEngine;


public class MultipleCamerasOnStartup : MonoBehaviour
{
    void Start()
    {
        int numCams = GameStateManager.numPlayers;
        if (numCams == 3) { numCams = 4; }
        GameObject[] camObjects = new GameObject[numCams];
        for (int i=0; i<numCams; ++i)
        {
            camObjects[i] = new GameObject($"Player {i + 1} Camera");
            switch (numCams)
            {
            case 1:
            {
                camObjects[i].AddComponent<Camera>().rect = new Rect(0,0,1,1);
                break;
            }
            case 2:
            {
                camObjects[i].AddComponent<Camera>().rect = new Rect(i * 0.5f, 0, 0.5f, 1);
                break;
            }
            case 4:
            {
                if (i == 0 || i == 1)
                {
                    camObjects[i].AddComponent<Camera>().rect = new Rect(i * 0.5f, 0.5f, 0.5f, 0.5f);
                    break;
                }
                else
                {
                    camObjects[i].AddComponent<Camera>().rect = new Rect((i-2) * 0.5f, 0.0f, 0.5f, 0.5f);
                    break;
                }
            }
            }
        }   
    }
}
