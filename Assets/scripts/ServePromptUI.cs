using UnityEngine;

public class ServePromptUI : MonoBehaviour
{
    public PlateController plate;
    public GameObject servePromptRoot; // your SERVE! UI object

    void Update()
    {
        if (!servePromptRoot || plate == null) return;
        servePromptRoot.SetActive(plate.IsComplete);
    }
}
