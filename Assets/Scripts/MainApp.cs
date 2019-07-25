using UnityEngine;
using System.Collections;
using nn;
using UnityEngine.XR.WSA;
using UnityEngine.XR;
using Microsoft.MixedReality.Toolkit.UI;

namespace nn
{
    public class MainApp : MonoBehaviour
    {

        public static MainApp Inst = null;

        public GameObject MeshRoot;
        public UIController uiController;
        public Material material;
        public Material BoxMaterial;
        public Material BoxGrabMaterial;
        public InteractableToggleCollectionExt ToggleCollection;

        private void Awake()
        {
            Inst = this;
        }

        public void LoadProject()
        {
            foreach (Transform obj in transform)
            {
                Destroy(obj.gameObject);
            }

            ProjectLoader.Load();

            var it = ToggleCollection.ToggleList[0];
            var label = it.transform.Find("ButtonContent").Find("Label");
            it.gameObject.name = "ALL";
            label.GetComponent<TextMesh>().text = "ALL";


            //var bdbox = MeshRoot.GetComponent<BoundingBox>();
            //if (bdbox)
            //{
            //    bdbox.Active = true;
            //}
            StartCoroutine(InitEnableboundingBox());
        }

        public void InitRadialNum(int num)
        {
            ToggleCollection.ToggleList = new Interactable[num];

            var dy = ToggleCollection.transform.GetChild(1).localPosition.y - ToggleCollection.transform.GetChild(0).localPosition.y;

            for (int i = ToggleCollection.transform.childCount; i < num; i++)
            {
                var go = Instantiate(ToggleCollection.transform.GetChild(i - 1).gameObject, ToggleCollection.transform);
                go.transform.localPosition += new Vector3(0, dy, 0);
            }
            for (int i = 0; i < num; i++)
            {
                var it = ToggleCollection.transform.GetChild(i).GetComponent<Interactable>();
                ToggleCollection.SetToggleItemAt(i, it);
                int itemIndex = i;
                it.OnClick.AddListener(() => OnRadialClicked(itemIndex));
            }
        }
        public void AddRadial(int i, GameObject go)
        {
            var it = ToggleCollection.ToggleList[i];
            var label = it.transform.Find("ButtonContent").Find("Label");
            it.gameObject.name = go.name;

            label.GetComponent<TextMesh>().text = go.name;
        }

        public void OnRadialClicked(int idx)
        {
            Debug.Log(idx);


            EnableboundingBox(MeshRoot, ToggleCollection.CurrentIndex == 0);


            for (int i = 1; i < ToggleCollection.ToggleList.Length; i++)
            {
                var it = ToggleCollection.ToggleList[i];
                var name = it.gameObject.name;
                var pickedGameObject = MeshRoot.transform.Find(name).gameObject;

                EnableboundingBox(pickedGameObject, ToggleCollection.CurrentIndex == i);

                if (i == ToggleCollection.CurrentIndex)
                {
                    uiController.PickedGameObject = pickedGameObject;
                }
            }
        }


        public IEnumerator InitEnableboundingBox()
        {

            yield return null;


            //EnableboundingBox(MeshRoot, false);

            foreach (Transform trans in MeshRoot.transform)
            {
                if (trans.gameObject.name != "rigRoot")
                {
                    EnableboundingBox(trans.gameObject, false);
                }
            }


        }
        public void EnableboundingBox(GameObject obj, bool v)
        {
       
            var bdbox = obj.GetComponent<BoundingBox>();
            if (bdbox)
            {
                bdbox.Active = v;
            }

            var manipulationHandler = obj.GetComponent<ManipulationHandler>();
            if (manipulationHandler)
            {
                manipulationHandler.enabled = v;
            }

            var boxcolliders = obj.GetComponents<BoxCollider>();
            foreach (var b in boxcolliders)
            {
                b.enabled = v;
            }

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