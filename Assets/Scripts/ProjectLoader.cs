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
        public static readonly string Hololens_TEST_PATH = "Hololens";
        public static readonly string PROJECT_JSON_NAME = "project.json";
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
        static void AddBoundingBox(int idx, GameObject go)
        {
            var boundingbox = go.AddComponent<BoundingBox>();
            boundingbox.BoxGrabbedMaterial = MainApp.Inst.BoxGrabMaterial;
            boundingbox.BoxMaterial = MainApp.Inst.BoxMaterial;
            boundingbox.BoundingBoxActivation = BoundingBox.BoundingBoxActivationType.ActivateManually;
            boundingbox.ScaleHandleSize = 0.03f;
            boundingbox.RotationHandleDiameter = 0.03f;
            boundingbox.WireframeEdgeRadius = 0.003f;
            boundingbox.HideElementsInInspector = false;

            go.AddComponent<ManipulationHandler>();

            MainApp.Inst.AddRadial(idx, go);

        }
        public static void Load()
        {
            GameObject root = MainApp.Inst.gameObject;
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

            MainApp.Inst.InitRadialNum(ProjectjsonObj.nodes.Count);

            int nodesIdx = 0;

            ProjectjsonObj.nodes.ForEach(p =>
            {
             
                var stl = path + "/" + p.uuid + ".stl";

                Debug.Log(stl);

                var meshes = Importer.Import(stl);

                string name = meshNameMap[p.uuid];

                Color color = Helper.ArrayToColor(p.color);
                color.a = p.opacity;

                Matrix4x4 matrix = Helper.ArrayToMatrix(p.matrix);
                matrix = root.transform.localToWorldMatrix * matrix;

                if (meshes.Length < 1)
                    return;

                if (meshes.Length < 2)
                {
                    var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    Object.DestroyImmediate(go.GetComponent<BoxCollider>());
                    go.transform.parent = root.transform;
                    go.transform.localScale = Vector3.one;

                    go.name = name;
                    meshes[0].name = "Mesh-" + name;
                    go.GetComponent<MeshFilter>().sharedMesh = meshes[0];
                    go.transform.FromMatrix(matrix);
                    var renderer = go.GetComponent<Renderer>();
                    renderer.material = sharedmaterial;
                    var material = renderer.material;
                    material.color = color;
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);

                    var box = go.AddComponent<BoxCollider>();
                    box.size = meshes[0].bounds.size;
                    box.center = meshes[0].bounds.center;

                    AddBoundingBox(nodesIdx, go);

                    mainBounds.Encapsulate(box.bounds);

                }
                else
                {
                    var parent = new GameObject();
                    parent.name = name;
                    parent.transform.parent = MainApp.Inst.transform;
                    parent.transform.localScale = Vector3.one;
                    parent.transform.localPosition = Vector3.zero;

                    var bds = new Bounds();

                    for (int i = 0, c = meshes.Length; i < c; i++)
                    {
                        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        Object.DestroyImmediate(go.GetComponent<BoxCollider>());
                        go.transform.SetParent(parent.transform, false);
                       

                        go.name = name + "(" + i + ")";

                        var mesh = meshes[i];
                        mesh.name = "Mesh-" + name + "(" + i + ")";
                        go.GetComponent<MeshFilter>().sharedMesh = mesh;
                        go.transform.FromMatrix(matrix);
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
                    box.size = bds.size;
                    box.center = bds.center;

                    AddBoundingBox(nodesIdx, parent);

                    mainBounds.Encapsulate(bds);

                }


                nodesIdx++;
            });
            //var mainbox = MainApp.Inst.gameObject.AddComponent<BoxCollider>();
            //mainbox.size = mainBounds.size;
            //mainbox.center = mainBounds.center;
            //AddBoundingBox(MainApp.Inst.gameObject);

        }
    }
}