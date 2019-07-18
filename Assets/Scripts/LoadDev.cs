using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA;

public class LoadDev : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        OnConnectIp();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnConnectIp()
    {
        HolographicRemoting.Connect("192.168.0.110");
    }
}
