using UnityEngine;
using System.Collections;
using nn;
using UnityEngine.XR.WSA;
using UnityEngine.XR;

namespace nn
{
    public class MainApp : MonoBehaviour
    {

        public static MainApp Inst = null;

        private void Awake()
        {
            Inst = this;
        }

        private void Start()
        {
            ProjectLoader.Load();
        }

        void Update()
        {
        
        }

        private void OnDestroy()
        {
            if (HolographicRemoting.ConnectionState == HolographicStreamerConnectionState.Connected)
            {
                HolographicRemoting.Disconnect();
            }
        }
    }
}