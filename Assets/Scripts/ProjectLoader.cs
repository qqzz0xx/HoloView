using Newtonsoft.Json;
using Parabox.Stl;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace nn
{
    public class ProjectLoader : ScriptableObject
    {
        public static readonly string Hololens_TEST_PATH = "Hololens_TEST";
        public static readonly string PROJECT_JSON_NAME = "project.json";


        public static void Load()
        {

            var path = Application.persistentDataPath + "/" + Hololens_TEST_PATH;

            var fileName = path + "/" + PROJECT_JSON_NAME;

            var txt = File.ReadAllText(fileName);

            var jsonObj = JsonConvert.DeserializeObject<List<ProjectJS>>(txt)[0];

            jsonObj.nodes.ForEach(p =>
            {

                var stl = path + "/" + p.uuid + ".stl";

                Debug.Log(stl);

                var meshes = Importer.Import(stl);

                string name = p.uuid;

                Matrix4x4 matrix = Helper.ArrayToMatrix(p.matrix);
                matrix = MainApp.Inst.transform.localToWorldMatrix * matrix * Matrix4x4.Scale(Vector3.one * 0.001f);

                if (meshes.Length < 1)
                    return;

                if (meshes.Length < 2)
                {
                    var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    Object.DestroyImmediate(go.GetComponent<BoxCollider>());
                    go.name = name;
                    meshes[0].name = "Mesh-" + name;
                    go.GetComponent<MeshFilter>().sharedMesh = meshes[0];
                    go.transform.parent = MainApp.Inst.transform;
                    go.transform.FromMatrix(matrix);
                }
                else
                {
                    var parent = new GameObject();
                    parent.name = name;

                    for (int i = 0, c = meshes.Length; i < c; i++)
                    {
                        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        Object.DestroyImmediate(go.GetComponent<BoxCollider>());
                        go.transform.SetParent(parent.transform, false);
                        go.name = name + "(" + i + ")";

                        var mesh = meshes[i];
                        mesh.name = "Mesh-" + name + "(" + i + ")";
                        go.GetComponent<MeshFilter>().sharedMesh = mesh;
                        go.transform.parent = MainApp.Inst.transform;
                        go.transform.FromMatrix(matrix);
                        //GameObject.Instantiate(go);
                        // ctx.AddObjectToAsset(go.name, go);
                    }
                }
            });
        }
    }
}