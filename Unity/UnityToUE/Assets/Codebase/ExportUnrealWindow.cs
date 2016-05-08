using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nucleus
{
    public class ExportUnrealWindow : EditorWindow
    {
        public ExportUnrealWindow()
        {
            this.titleContent = new GUIContent("Unreal Export");

        }

        [MenuItem("Tools/Export Unreal Project...")]
        private static void Init()
        {
            //ExportUnrealWindow window = EditorWindow.GetWindow<ExportUnrealWindow>();
            //window.Show();

            // for now dont make the window just test

            // export static meshes
            Scene scene = SceneManager.GetActiveScene();
            GameObject[] roots = scene.GetRootGameObjects();

            string sceneFile = Path.Combine(Application.dataPath, scene.name + ".json");

            SceneInfo sceneInfo = new SceneInfo();
            sceneInfo.name = scene.name;

            for (int i = 0; i < roots.Length; i++)
            {
                GameObject root = roots[i];
                ObjectInfo obj = MakeObject(root);
                sceneInfo.rootObjects.Add(obj);

                RecursiveSearch(root, obj, sceneInfo);
            }

            string json = JsonConvert.SerializeObject(sceneInfo);
            if (File.Exists(sceneFile))
            {
                File.Delete(sceneFile);
            }
            File.WriteAllText(sceneFile, json);
        }

        private static ObjectInfo MakeObject(GameObject root)
        {
            ObjectInfo thisObj = new ObjectInfo();
            thisObj.name = root.name;
            thisObj.active = root.activeSelf;
            thisObj.position = new float3(root.transform.position);
            thisObj.scale = new float3(root.transform.localScale);
            thisObj.rotation = new float4(root.transform.rotation);

            return thisObj;
        }

        private static AssetInfo AddAsset(UnityEngine.Object obj, SceneInfo scene)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            if (scene.usedAssets.Any(c => c.fullPath == path))
            {
                return scene.usedAssets.First(c => c.fullPath == path);
            }

            // get import options
            AssetImporter importer = AssetImporter.GetAtPath(path);
            AssetInfo asset;//= new AssetInfo();

            if (importer is TextureImporter)
            {
                TextureAssetInfo ass = new TextureAssetInfo();
                asset = ass;

                TextureImporter tex = (TextureImporter)importer;
                ass.type = tex.textureType;
            }
            else if (importer is ModelImporter)
            {
                ModelAssetInfo ass = new ModelAssetInfo();
                asset = ass;

                ModelImporter model = (ModelImporter)importer;
                ass.fileScale = model.fileScale;
            }
            else
            {
                asset = new AssetInfo();
            }
            asset.fullPath = path;
            scene.usedAssets.Add(asset);

            return asset;
        }

        private static void RecursiveSearch(GameObject root, ObjectInfo obj, SceneInfo sceneInfo)
        {
            ObjectInfo thisObj = MakeObject(root);
            obj.children.Add(thisObj);

            Component[] comps = root.GetComponents<Component>();

            for (int i = 0; i < comps.Length; i++)
            {
                Component comp = comps[i];
                if (comp == null)
                {
                    continue;
                }

                if (comp is MeshRenderer)
                {
                    MeshRenderer meshRen = (MeshRenderer)comp;
                    MeshFilter filter = comps.FirstOrDefault(c => c is MeshFilter) as MeshFilter;
                    if (filter == null)
                    {
                        continue;
                    }
                    Mesh mesh = filter.sharedMesh;
                    if (mesh == null)
                    {
                        continue;
                    }

                    string path = AssetDatabase.GetAssetPath(mesh);
                    AssetInfo asset = AddAsset(mesh, sceneInfo);
                    if (asset == null)
                    {
                        continue;
                    }

                    MeshRendererComponent meshRenderer = new MeshRendererComponent();
                    meshRenderer.mesh = sceneInfo.usedAssets.IndexOf(asset);
                    meshRenderer.enabled = true;
                    thisObj.components.Add(meshRenderer);

                    Material[] mats = meshRen.sharedMaterials;
                    for (int j = 0; j < mats.Length; j++)
                    {
                        Material mat = mats[j];
                        MaterialInfo matInfo = new MaterialInfo();
                        meshRenderer.materials.Add(matInfo);

                        if (mat == null)
                        {
                            // make empty material just so the indexing works
                            // properly
                            continue;
                        }
                        matInfo.name = mat.name;

                        if (mat.mainTexture != null)
                        {
                            AssetInfo mainTex = AddAsset(mat.mainTexture, sceneInfo);
                            MaterialPropertyInfo tex = new MaterialPropertyInfo();
                            tex.value = sceneInfo.usedAssets.IndexOf(mainTex);
                            tex.name = "DiffuseTexture";
                            tex.type = ShaderUtil.ShaderPropertyType.TexEnv;
                            matInfo.properties.Add(tex);
                        }

                        if (mat.HasProperty("_Color"))
                        {
                            MaterialPropertyInfo color = new MaterialPropertyInfo();
                            color.value = new float4(mat.color);
                            color.name = "DiffuseColor";
                            color.type = ShaderUtil.ShaderPropertyType.Color;
                            matInfo.properties.Add(color);
                        }
                    }
                }
            }

            foreach (Transform trans in root.transform)
            {
                RecursiveSearch(trans.gameObject, thisObj, sceneInfo);
            }
        }

        private void OnGUI()
        {

        }
    }
}
