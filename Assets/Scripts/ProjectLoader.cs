using Microsoft.MixedReality.Toolkit.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Parabox.Stl;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
namespace nn
{

    public class ProjectLoader : ScriptableObject
    {
        class MeshCS
        {
            public string name;
            public string uuid;
            public GameObject mesh;
            public Bounds bounds;

        } 

        public static readonly string Hololens_TEST_PATH = "naviholo";
        public static readonly string PROJECT_JSON_NAME = "project.json";
        public static readonly string MEASURE_JSON_NAME = "measure.json";
        public static T CopyComponent<T>(T original, GameObject destination) where T : Component
        {
            System.Type type = original.GetType();
            var dst = destination.GetComponent(type) as T;
            if (!dst) dst = destination.AddComponent(type) as T;
            var fields = type.GetFields();
            foreach (var field in fields)
            {
                if (field.IsStatic) continue;
                field.SetValue(dst, field.GetValue(original));
            }
            var props = type.GetProperties();
            foreach (var prop in props)
            {
                if (!prop.CanWrite || !prop.CanWrite || prop.Name == "name") continue;
                prop.SetValue(dst, prop.GetValue(original, null), null);
            }
            return dst as T;
        }
        static BoundingBox AddBoundingBox(int idx, GameObject go)
        {
            var boundingbox = go.AddComponent<BoundingBox>();
            boundingbox.BoxGrabbedMaterial = MainApp.Inst.BoxGrabMaterial;
            boundingbox.BoxMaterial = MainApp.Inst.BoxMaterial;
            boundingbox.BoundingBoxActivation = BoundingBox.BoundingBoxActivationType.ActivateManually;
            boundingbox.ScaleHandleSize = 0.015f;
            boundingbox.RotationHandleDiameter = 0.015f;
            boundingbox.WireframeEdgeRadius = 0.0015f;
            boundingbox.HideElementsInInspector = false;

            go.AddComponent<ManipulationHandler>();

            MainApp.Inst.AddRadial(idx, go);

            return boundingbox;

        }

        public static void LoadMeasure()
        {
            var root = MainApp.Inst.MeshRoot.transform.parent;

            var path = Application.persistentDataPath + "/" + Hololens_TEST_PATH;

            var fileName = path + "/" + MEASURE_JSON_NAME;

            var txt = File.ReadAllText(fileName);

            var lineLists = JsonConvert.DeserializeObject<List<MeasureJS>>(txt);

            var prefab = Resources.Load("MeasureLine") as GameObject;

            lineLists.ForEach(p =>
            {

                var p1 = Helper.ArrayToVector3(p.@base.p1);
                var p2 = Helper.ArrayToVector3(p.@base.p2);
                p1.x = -p1.x;
                p2.x = -p2.x;

                var go = GameObject.Instantiate(prefab);

                go.name = p.txt;
                go.transform.parent = root;
                go.transform.localScale = Vector3.one;

                var line = go.GetComponent<MeasureLine>();

                line.SetPoint(p1, p2);

            });

        }
        public static void Load()
        {
            GameObject root = MainApp.Inst.MeshRoot;
            Material sharedmaterial = MainApp.Inst.material;

            var path = Application.persistentDataPath + "/" + Hololens_TEST_PATH;

            var fileName = path + "/" + PROJECT_JSON_NAME;

            var txt = File.ReadAllText(fileName);

            var ProjectjsonObj = JsonConvert.DeserializeObject<List<ProjectJS>>(txt)[0];

            fileName = path + "/" + "mask.json";
            txt = File.ReadAllText(fileName);
            JObject jObject = JObject.Parse(txt);
            IList<JToken> results = jObject["CTMaskList"].Children().ToList();

            Dictionary<string, string> meshNameMap = new Dictionary<string, string>();
            foreach (JToken result in results)
            {
                var id = result["ID"].ToString();
                var name = result["Name"].ToString();

                meshNameMap[id] = name;
            }


            var mainBounds = new Bounds();

            MainApp.Inst.InitRadialNum(ProjectjsonObj.nodes.Count + 1);

            int nodesIdx = 1;

            ProjectjsonObj.nodes.ForEach(p =>
            {
             
                var stl = path + "/" + p.uuid + ".stl";

                Debug.Log(stl);

                var meshes = Importer.Import(stl, CoordinateSpace.Right, UpAxis.Z, true, UnityEngine.Rendering.IndexFormat.UInt32);
                string name;
                if (p.name == "MeshActor")
                {
                    name = meshNameMap[p.uuid];
                }
                else
                {
                    name = p.name;
                }

                Color color = Helper.ArrayToColor(p.color);
                color.a = p.opacity;

                Matrix4x4 matrix = Helper.ArrayToMatrix(p.matrix);
                Matrix4x4 rightToLeftMat = Matrix4x4.zero;
                rightToLeftMat.m02 = -1;
                rightToLeftMat.m10 = 1;
                rightToLeftMat.m21 = 1;
                rightToLeftMat.m33 = 1;

                matrix.m03 = -matrix.m03;
                //Matrix4x4 t = Matrix4x4.identity;
                //t.m00 = -1;
                //t.m11 = 1;
                //t.m22 = -1;
                matrix = matrix * rightToLeftMat;

                if (meshes.Length < 1)
                    return;

                if (meshes.Length < 2)
                {
                    var parent = new GameObject();
                    parent.name = name;
                    parent.transform.parent = root.transform;
                    parent.transform.localScale = Vector3.one;
                    parent.transform.localPosition = Vector3.zero;
                    parent.transform.localRotation = Quaternion.identity;


                    var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    Object.DestroyImmediate(go.GetComponent<BoxCollider>());
                    go.transform.parent = parent.transform;
                    go.transform.localScale = Vector3.one;
                    go.transform.FromMatrix(matrix);

                    //go.layer = LayerMask.NameToLayer("Transparent");

                    go.name = name;
                    meshes[0].name = "Mesh-" + name;
          
                    go.GetComponent<MeshFilter>().sharedMesh = meshes[0];

                

                    var renderer = go.GetComponent<Renderer>();
                    renderer.material = sharedmaterial;
                    var material = renderer.material;
                    material.color = color;
                    //material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    //material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    //material.SetInt("_ZWrite", 0);
                    //material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent + nodesIdx;

                    var box = go.AddComponent<BoxCollider>();
                    box.size = meshes[0].bounds.size + Vector3.one * 100;
                    box.center = meshes[0].bounds.center;

                    var boundingBox = AddBoundingBox(nodesIdx, parent);
                    boundingBox.BoundsOverride = box;

                    mainBounds.Encapsulate(meshes[0].bounds);

                }
                else
                {
                    var parent = new GameObject();
                    parent.name = name;
                    parent.transform.parent = root.transform;
                    parent.transform.localScale = Vector3.one;
                    parent.transform.FromMatrix(matrix);

                    var bds = new Bounds();

                    for (int i = 0, c = meshes.Length; i < c; i++)
                    {
                        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        Object.DestroyImmediate(go.GetComponent<BoxCollider>());
                        go.transform.SetParent(parent.transform, false);
                        go.transform.localPosition = Vector3.zero;

                        go.name = name + "(" + i + ")";

                        var mesh = meshes[i];
                        mesh.name = "Mesh-" + name + "(" + i + ")";
                        go.GetComponent<MeshFilter>().sharedMesh = mesh;
                    
                        //GameObject.Instantiate(go);
                        // ctx.AddObjectToAsset(go.name, go);
                        var renderer = go.GetComponent<Renderer>();
                        renderer.material = sharedmaterial;
                        var material = renderer.material;

                        material.color = color;
                        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        material.SetInt("_ZWrite", 0);
   
                        bds.Encapsulate(new Bounds(mesh.bounds.center, mesh.bounds.size));

                    }


                    var box = parent.AddComponent<BoxCollider>();
                    box.size = bds.size  + Vector3.one * 100;
                    box.center = bds.center;

                    var boundingBox = AddBoundingBox(nodesIdx, parent);
                    boundingBox.BoundsOverride = box;

                    mainBounds.Encapsulate(bds);

                }


                nodesIdx++;
            });

            var mainbox = root.gameObject.AddComponent<BoxCollider>();
            mainbox.size = mainBounds.size + Vector3.one * 100;
            mainbox.center = mainBounds.center;
            var mainBounding = AddBoundingBox(0, root);

            mainBounding.BoundsOverride = mainbox;

        }
    }
}