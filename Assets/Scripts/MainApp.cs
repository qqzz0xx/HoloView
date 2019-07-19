﻿using UnityEngine;
using System.Collections;
using nn;
using UnityEngine.XR.WSA;
using UnityEngine.XR;

public class MainApp : MonoBehaviour
{
    private void Awake()
    {
        //STLLoader.Load();
    }
    private string IP = "192.168.0.110";

    private bool connected;

    private void Start()
    {
        Connect();
    }
    public void Connect()
    {
        if (HolographicRemoting.ConnectionState != HolographicStreamerConnectionState.Connected)
        {
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
    }
}
