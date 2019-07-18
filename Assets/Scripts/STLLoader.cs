using Parabox.Stl;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace nn
{
    public class STLLoader : ScriptableObject
    {
        static readonly string Hololens_TEST_PATH = "Hololens_TEST";
        static readonly string PROJECT_JSON_NAME = "project.json";


        public static void Load()
        {
            var path = Application.persistentDataPath + "/" + Hololens_TEST_PATH;

            var stl = path + "/" + "start_part15.stl";


            var meshes = Importer.Import(stl);

            string name = "STL_obj";

            if (meshes.Length < 1)
                return;

            if (meshes.Length < 2)
            {
                var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Object.DestroyImmediate(go.GetComponent<BoxCollider>());
                go.name = name;
                meshes[0].name = "Mesh-" + name;
                go.GetComponent<MeshFilter>().sharedMesh = meshes[0];

                //GameObject.Instantiate(go);
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
                    //GameObject.Instantiate(go);
                    // ctx.AddObjectToAsset(go.name, go);
                }



            }

        }

    }
}