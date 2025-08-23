using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace KidGame.Core
{
    public class SoLoader : SingletonNoMono<SoLoader>
    {
        
        public Dictionary<string, ScriptableObject> soDic;

        //装Entity的 id -> Entity 也就是一行数据的字典
        private Dictionary<string, object> entityDic;

        public void InitialSoResource()
        {

            soDic = new Dictionary<string, ScriptableObject>();
            entityDic = new Dictionary<string, object>();

            ScriptableObject[] allSobj = Resources.LoadAll<ScriptableObject>("ScriptObject");

            Debug.Log($"共找到 {allSobj.Length} 个 ScriptableObject");
            foreach (var so in allSobj)
            {
                if (so != null)
                {
                    if (!soDic.ContainsKey(so.name)) soDic.Add(so.name, so);
                    else Debug.LogWarning("重复加载！！！！");
                }
                else Debug.LogWarning("So为空异常！！！");
            }
        }

        public kidgame_game_data_config GetGameDataConfig()
        {
            return soDic[SoConst.KID_GAME_DATA_CONFIG] as kidgame_game_data_config;
        }

        // 封装一层 不然 点引用太长了
        public List<T> GetDataList<T>() where T : BagItemInfoBase
        {
            kidgame_game_data_config temp = soDic[SoConst.KID_GAME_DATA_CONFIG] as kidgame_game_data_config;
            if (temp != null) return temp.GetDataList<T>();
            else
            {
                Debug.LogWarning("kidgame_game_data_config 为空！ 请确定配表是否正常导入");
                return null;
            }
        }

        /// <summary>
        /// 通过id  配表名 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dataName"></param>
        /// <param name="soName"></param>
        /// <returns></returns>
        private object GetDataById(string id, string dataName, string soName)
        {
            // 直接从缓存中查找
            string compositeKey = $"{dataName}_{id}";
            if (entityDic.TryGetValue(compositeKey, out object cachedData))
            {
                return cachedData;
            }

            // 缓存中没有，从SO中查找

            if (!soDic.TryGetValue(soName, out ScriptableObject so))
            {
                Debug.LogError("�Ҳ���SO: " + soName);
                return null;
            }

            // 在SO中查找实体列表
            object foundEntity = FindEntityInSo(so, id);
            if (foundEntity != null)
            {
                // 添加到缓存
                entityDic[compositeKey] = foundEntity;
                return foundEntity;
            }

            Debug.LogWarning($"在SO '{soName}' 中未找到ID为 {id} 的实体");
            return null;
        }

        /*private object FindEntityInSo(ScriptableObject so, string id)
        {
            var fields = so.GetType().GetFields();

            foreach (var field in fields)
            {
                field.GetValue("id");
                // 检查字段是否为List<>类型
                if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var entityList = field.GetValue(so);
                    *//*直接使用 entityList.Count 不香吗？为什么要绕这么一大圈？
        原因：               entityList 的类型在编译时是 object（因为通过反射获取的字段值默认是 object 类型），
                    编译器不知道它是一个列表，因此无法直接访问 Count 属性。*//*
                    //GetType->List    GetProperty->Count   GetValue  -> num
                    var count = (int)entityList.GetType().GetProperty("Count").GetValue(entityList);
                    for (int i = 0; i < count; i++)
                    {
                        *//*第一个参数 entityList：表示要获取属性值的对象（即列表实例）。
                        第二个参数 new object[] { i }：索引器的参数（这里传入索引值 i，表示要获取第 i 个元素）。*//*
                        var entity = entityList.GetType().GetProperty("Item").GetValue(entityList, new object[] { i });

                        // 检查实体是否有"id"属性
                        var idProperty = entity.GetType().GetProperty("id");

                        if (idProperty != null)
                        {
                            string entityId = idProperty.GetValue(entity).ToString();
                            //Debug.Log(entityId);
                            // 找到匹配ID的实体
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
            //field是字段    value是拿值
            //GetProperty是拿属性    public string Id { get; set; } // id 是属性
            var fields = so.GetType().GetFields();

            foreach (var field in fields)
            {
                // 检查字段是否为 List<> 类型

                if (field.FieldType.IsGenericType &&
                    field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    //从so这个fields获取一个field

                    var entityList = field.GetValue(so);
                    if (entityList == null) continue;

                    // 更高效的方式 直接转换为 IList 避免反射 Count

                    var list = entityList as System.Collections.IList;
                    if (list == null) continue;

                    for (int i = 0; i < list.Count; i++)
                    {
                        var entity = list[i];  // 直接通过索引访问 
                        if (entity == null) continue;

                        // 检查实体是否有 "id" 字段

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

        #region 配表快捷获取方法 通过id

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