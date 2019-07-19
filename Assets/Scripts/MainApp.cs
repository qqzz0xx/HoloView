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

        public void LoadProject()
        {
            foreach(Transform obj in transform)
            {
                Destroy(obj.gameObject);
            }

            ProjectLoader.Load();
        }

        private void Start()
        {
            LoadProject();
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