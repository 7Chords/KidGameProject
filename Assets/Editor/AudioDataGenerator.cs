using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using KidGame.Core;

namespace KidGame.Editor
{
    public static class AudioDataGenerator
    {
        [MenuItem("Tools/Audio/Generate Audio Data")]
        public static void GenerateAudioDataFromResources()
        {
            AudioDatas audioDatas = FindOrCreateAudioDatasAsset();
            audioDatas.audioDataList.Clear();

            string resourcesPath = Application.dataPath + "/Resources/Audios";
            string[] audioFiles = Directory.GetFiles(resourcesPath, "*.*", SearchOption.AllDirectories);

            HashSet<string> supportedExtensions = new HashSet<string> { ".wav", ".mp3", ".ogg" };

            foreach (string filePath in audioFiles)
            {
                string extension = Path.GetExtension(filePath).ToLower();
                if (!supportedExtensions.Contains(extension))
                    continue;

                string relativePath = filePath.Replace(Application.dataPath + "/Resources/", "");
                relativePath = relativePath.Replace(extension, "");

                AudioData newData = new AudioData
                {
                    audioName = Path.GetFileNameWithoutExtension(filePath),
                    audioPath = relativePath
                };

                audioDatas.audioDataList.Add(newData);
            }

            EditorUtility.SetDirty(audioDatas);
            AssetDatabase.SaveAssets();

            Debug.Log($"Successfuly generate {audioDatas.audioDataList.Count} audio datas!");
        }

        private static AudioDatas FindOrCreateAudioDatasAsset()
        {
            string[] guids = AssetDatabase.FindAssets("t:AudioDatas");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<AudioDatas>(path);
            }

            AudioDatas newAsset = ScriptableObject.CreateInstance<AudioDatas>();
            newAsset.audioDataList = new List<AudioData>();

            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }

            string assetPath = "Assets/Resources/AudioDataList.asset";
            AssetDatabase.CreateAsset(newAsset, assetPath);
            AssetDatabase.SaveAssets();

            return newAsset;
        }
    }
}
