using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using KidGame.Core;

namespace KidGame.Editor
{
    public static class ParticleDataGenerator
    {
        [MenuItem("Tools/Generator/Generate Particle Data")]
        public static void GenerateParticleDataFromResources()
        {
            ParticleDatas particleDatas = FindOrCreateParticleDatasAsset();
            particleDatas.particleDataList.Clear();

            string resourcesPath = Application.dataPath + "/Resources/Particles";
            
            // 确保目录存在
            if (!Directory.Exists(resourcesPath))
            {
                Debug.LogWarning($"Particles directory not found: {resourcesPath}");
                return;
            }

            string[] particleFiles = Directory.GetFiles(resourcesPath, "*.prefab", SearchOption.AllDirectories);

            foreach (string filePath in particleFiles)
            {
                string relativePath = filePath.Replace(Application.dataPath + "/Resources/", "");
                relativePath = relativePath.Replace(".prefab", "");

                ParticleData newData = new ParticleData
                {
                    effectName = Path.GetFileNameWithoutExtension(filePath),
                    effectPath = relativePath
                };

                particleDatas.particleDataList.Add(newData);
            }

            EditorUtility.SetDirty(particleDatas);
            AssetDatabase.SaveAssets();

            Debug.Log($"Successfully generated {particleDatas.particleDataList.Count} particle effect datas!");
        }

        private static ParticleDatas FindOrCreateParticleDatasAsset()
        {
            string[] guids = AssetDatabase.FindAssets("t:ParticleDatas");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<ParticleDatas>(path);
            }

            ParticleDatas newAsset = ScriptableObject.CreateInstance<ParticleDatas>();
            newAsset.particleDataList = new List<ParticleData>();
            
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }

            string assetPath = "Assets/Resources/ParticleDataList.asset";
            AssetDatabase.CreateAsset(newAsset, assetPath);
            AssetDatabase.SaveAssets();

            return newAsset;
        }
    }
}