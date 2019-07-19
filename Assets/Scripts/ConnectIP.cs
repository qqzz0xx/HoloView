using System.Collections;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.WSA;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConnectIP : MonoBehaviour
{
    public InputField inputField;

    [SerializeField]
    private string IP;

    private bool connected;


    //private void Start()
    //{
    //    Connect();
    //}
    public void Connect()
    {
        if (HolographicRemoting.ConnectionState != HolographicStreamerConnectionState.Connected)
        {
            if (inputField != null && !string.IsNullOrEmpty(inputField.text))
            {
                IP = inputField.text;
            }

            HolographicRemoting.Connect(IP);
        }
    }

    void Update()
    {
        if (!connected && HolographicRemoting.ConnectionState == HolographicStreamerConnectionState.Connected)
        {
            connected = true;

            StartCoroutine(LoadDevice("WindowsMR"));
        }
    }

    IEnumerator LoadDevice(string newDevice)
    {
        XRSettings.LoadDeviceByName(newDevice);
        yield return null;
        XRSettings.enabled = true;

        yield return SceneManager.LoadSceneAsync("MRScene");
    }
}
