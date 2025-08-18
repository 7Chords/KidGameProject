using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace KidGame.Core
{
    public class SoLoader : SingletonPersistent<SoLoader>
    {

        public Dictionary<string, ScriptableObject> soDic;

        //װEntity�� id -> Entity��ƥ��
        private Dictionary<string, object> entityDic;

        protected override void Awake()
        {
            base.Awake();
            
            soDic = new Dictionary<string, ScriptableObject>();
            entityDic = new Dictionary<string, object>();
            InitialSoResource();
        }

        private void InitialSoResource()
        {
            ScriptableObject[] allSobj = Resources.LoadAll<ScriptableObject>("ScriptObject");

            Debug.Log($"�ҵ� {allSobj.Length} �� ScriptableObject");
            // ���ӵ��ֵ䣨��Ϊ�ļ�����������չ����
            foreach (var so in allSobj)
            {
                if (so != null)
                {
                    if (!soDic.ContainsKey(so.name)) soDic.Add(so.name, so);
                    else Debug.LogWarning("�ظ����أ��� ����Ƿ�������������µ�����SO����");
                }
                else Debug.LogWarning("soΪ�գ���");
            }
        }

        public kidgame_game_data_config GetGameDataConfig()
        {
            return soDic[SoConst.KID_GAME_DATA_CONFIG] as kidgame_game_data_config;
        }

        /// <summary>
        /// ����ֵΪ��Ҫ�ҵ�ĳ�������string���͵�id
        /// �Լ����Data Type������
        /// �� װ�����Entity�� So����
        /// </summary>
        private object GetDataById(string id, string dataName, string soName)
        {
            // ֱ�Ӵӻ����в���
            string compositeKey = $"{dataName}_{id}";
            if (entityDic.TryGetValue(compositeKey, out object cachedData))
            {
                return cachedData;
            }

            // ������û�У���SO�в���
            if (!soDic.TryGetValue(soName, out ScriptableObject so))
            {
                Debug.LogError("�Ҳ���SO: " + soName);
                return null;
            }

            // ��SO�в���ʵ���б�
            object foundEntity = FindEntityInSo(so, id);
            if (foundEntity != null)
            {
                // ���ӵ�����
                entityDic[compositeKey] = foundEntity;
                return foundEntity;
            }

            Debug.LogWarning($"��SO '{soName}' ��δ�ҵ�IDΪ {id} ��ʵ��");
            return null;
        }

        // ��SO�в����ض�ID��ʵ��
        /*private object FindEntityInSo(ScriptableObject so, string id)
        {
            var fields = so.GetType().GetFields();

            foreach (var field in fields)
            {
                field.GetValue("id");
                // ����ֶ��Ƿ�ΪList<>����
                if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var entityList = field.GetValue(so);
                    */ /*ֱ��ʹ�� entityList.Count ������ΪʲôҪ����ôһ��Ȧ��
ԭ��               entityList �������ڱ���ʱ�� object����Ϊͨ�������ȡ���ֶ�ֵĬ���� object ���ͣ���
                    ��������֪������һ���б�������޷�ֱ�ӷ��� Count ���ԡ�*/ /*
                    //GetType->List    GetProperty->Count   GetValue  -> num
                    var count = (int)entityList.GetType().GetProperty("Count").GetValue(entityList);
                    for (int i = 0; i < count; i++)
                    {
                        */ /*��һ������ entityList����ʾҪ��ȡ����ֵ�Ķ��󣨼��б�ʵ������
                        �ڶ������� new object[] { i }���������Ĳ��������ﴫ������ֵ i����ʾҪ��ȡ�� i ��Ԫ�أ���*/ /*
                        var entity = entityList.GetType().GetProperty("Item").GetValue(entityList, new object[] { i });

                        // ���ʵ���Ƿ���"id"����
                        var idProperty = entity.GetType().GetProperty("id");

                        if (idProperty != null)
                        {
                            string entityId = idProperty.GetValue(entity).ToString();
                            //Debug.Log(entityId);
                            // �ҵ�ƥ��ID��ʵ��
                            if (entityId == id)
                            {
                                return entity;
                            }
                        }
                    }
                }
            }

            return null;
        }
*/
        private object FindEntityInSo(ScriptableObject so, string id)
        {
            //field���ֶ�    value����ֵ
            //GetProperty��������    public string Id { get; set; } // id ������
            var fields = so.GetType().GetFields();

            foreach (var field in fields)
            {
                // ����ֶ��Ƿ�Ϊ List<> ����
                if (field.FieldType.IsGenericType &&
                    field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    //��so���fields��ȡһ��field
                    var entityList = field.GetValue(so);
                    if (entityList == null) continue;

                    // ����Ч�ķ�ʽ ֱ��ת��Ϊ IList ���ⷴ�� Count
                    var list = entityList as System.Collections.IList;
                    if (list == null) continue;

                    for (int i = 0; i < list.Count; i++)
                    {
                        var entity = list[i]; // ֱ��ͨ���������� 
                        if (entity == null) continue;

                        // ���ʵ���Ƿ��� "id" �ֶ�
                        var idField = entity.GetType().GetField("id");
                        if (idField != null)
                        {
                            string entityId = idField.GetValue(entity)?.ToString();
                            if (entityId == id)
                            {
                                return entity;
                            }
                        }
                    }
                }
            }

            return null;
        }

        #region ���ÿһ��Data �����Ƶĵ��ǲ�ͬ�Ĵ���

        //So kidgame_game_data_config

        public TrapData GetTrapDataById(string id)
        {
            return GetDataById(id, "TrapData", SoConst.KID_GAME_DATA_CONFIG) as TrapData;
        }

        public MaterialData GetMaterialDataDataById(string id)
        {
            return GetDataById(id, "MaterialData", SoConst.KID_GAME_DATA_CONFIG) as MaterialData;
        }

        public RoomInfoData GetRoomInfoDataById(string id)
        {
            return GetDataById(id, "RoomInfoData", SoConst.KID_GAME_DATA_CONFIG) as RoomInfoData;
        }

        public FurnitureInfoData GetFurnitureInfoDataById(string id)
        {
            return GetDataById(id, "FurnitureInfoData", SoConst.KID_GAME_DATA_CONFIG) as FurnitureInfoData;
        }

        public BuffData GetBuffDataDataById(string id)
        {
            return GetDataById(id, "BuffData", SoConst.KID_GAME_DATA_CONFIG) as BuffData;
        }

        public WeaponData GetWeaponDataById(string id)
        {
            return GetDataById(id, "WeaponData", SoConst.KID_GAME_DATA_CONFIG) as WeaponData;
        }

        public FoodData GetFoodDataById(string id)
        {
            return GetDataById(id, "FoodData", SoConst.KID_GAME_DATA_CONFIG) as FoodData;
        }

        #endregion
    }
}