using UnityEngine;
using TMPro;

public class ConnectionBannerUI : MonoBehaviour
{
    public ESP32JsonReader reader;

    [Header("UI")]
    public GameObject bannerRoot;
    public TMP_Text bannerText;

    void Update()
    {
        bool connected = reader != null && reader.IsConnected;

        if (bannerRoot) bannerRoot.SetActive(!connected);
        if (!connected && bannerText) bannerText.text = "CONTROLLER DISCONNECTED";
    }
}
