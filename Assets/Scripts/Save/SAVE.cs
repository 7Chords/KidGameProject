using System.IO;
using UnityEngine;
using System.Security.Cryptography;
using System;

namespace KidGame.Core
{
    public static class SAVE
    {
        static string GetPath(string fileName)
        {
            return Path.Combine(Application.persistentDataPath, fileName);
        }

        #region 加密相关
        private static readonly string EncryptionKey = "KidGame765"; // 密钥
        private static readonly byte[] Salt = new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 };

        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return plainText;

            using (Aes aesAlg = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, Salt);
                aesAlg.Key = pdb.GetBytes(32);
                aesAlg.IV = pdb.GetBytes(16);

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return cipherText;

            try
            {
                using (Aes aesAlg = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, Salt);
                    aesAlg.Key = pdb.GetBytes(32);
                    aesAlg.IV = pdb.GetBytes(16);

                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                return srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch
            {
                Debug.LogWarning("解密失败，可能数据未加密或密钥不匹配");
                return cipherText; // 返回原始数据，可能是未加密的旧存档
            }
        }
        #endregion

        #region  PlayerPrefs
        public static void PlayerPrefsSave(string key, object data, bool encrypt = true)
        {
            string json = JsonUtility.ToJson(data);
            string dataToSave = encrypt ? Encrypt(json) : json;
            PlayerPrefs.SetString(key, dataToSave);
            PlayerPrefs.Save();
        }

        public static T PlayerPrefsLoad<T>(string key, bool encrypted = true) where T : new()
        {
            string data = PlayerPrefs.GetString(key, null);
            if (string.IsNullOrEmpty(data)) return new T();

            try
            {
                string json = encrypted ? Decrypt(data) : data;
                return JsonUtility.FromJson<T>(json);
            }
            catch (Exception ex)
            {
                Debug.LogError($"PlayerPrefsLoad failed: {ex.Message}");
                return new T();
            }
        }
        #endregion

        #region JSON
        public static void JsonSave(string fileName, object data, bool encrypt = true)
        {
            string json = JsonUtility.ToJson(data);
            string dataToSave = encrypt ? Encrypt(json) : json;

            File.WriteAllText(GetPath(fileName), dataToSave);
            Debug.Log($"已保存{GetPath(fileName)}");
        }

        public static T JsonLoad<T>(string fileName, bool encrypted = true) where T : new()
        {
            string path = GetPath(fileName);
            if (File.Exists(path))
            {
                try
                {
                    string fileData = File.ReadAllText(path);
                    string json = encrypted ? Decrypt(fileData) : fileData;
                    var data = JsonUtility.FromJson<T>(json);
                    Debug.Log($"读取{path}");
                    return data;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"JsonLoad failed: {ex.Message}");
                    return new T();
                }
            }
            return new T();
        }

        public static void JsonDelete(string fileName)
        {
            File.Delete(GetPath(fileName));
        }
        
        public static void ResetGlobalData()
        {
            RecordData.Instance.unlockedItems.Clear();
            RecordData.Instance.Save();
        }
        #endregion

        #region 清空
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Tools/Delete/Records List")]
        public static void DeleteRecord()
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("已清空存档列表");
        }

        [UnityEditor.MenuItem("Tools/Delete/Player Data")]
        public static void DeletePlayerData()
        {
            ClearDirectory(Application.persistentDataPath);
            Debug.Log("已清空玩家数据");
        }

        static void ClearDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                foreach (FileInfo file in new DirectoryInfo(path).GetFiles())
                {
                    File.Delete(file.FullName);
                }
            }
        }

        [UnityEditor.MenuItem("Tools/Delete/Global Data")]
        public static void DeleteGlobalData()
        {
            ResetGlobalData();
            Debug.Log("已清空全局数据");
        }

        [UnityEditor.MenuItem("Tools/Delete/All")]
        public static void DeleteAll()
        {
            DeletePlayerData();
            DeleteRecord();
            DeleteGlobalData();
        }

        [UnityEditor.MenuItem("Tools/Encryption/Test Encryption")]
        public static void TestEncryption()
        {
            string testData = "Hello, World!";
            string encrypted = Encrypt(testData);
            string decrypted = Decrypt(encrypted);
            
            Debug.Log($"Original: {testData}");
            Debug.Log($"Encrypted: {encrypted}");
            Debug.Log($"Decrypted: {decrypted}");
            Debug.Log($"Test Result: {testData == decrypted}");
        }
#endif
        #endregion
    }
}
