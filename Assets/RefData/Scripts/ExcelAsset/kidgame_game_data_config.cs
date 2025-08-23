using KidGame.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class kidgame_game_data_config : ScriptableObject
{
	public List<TrapData> TrapDataList; // Replace 'EntityType' to an actual type that is serializable.
	public List<MaterialData> MaterialDataList; // Replace 'EntityType' to an actual type that is serializable.
	public List<WeaponData> WeaponDataList; // Replace 'EntityType' to an actual type that is serializable.
	public List<FoodData> FoodDataList; // Replace 'EntityType' to an actual type that is serializable.
	public List<FurnitureInfoData> FurnitureInfoDataList; // Replace 'EntityType' to an actual type that is serializable.
	public List<RoomInfoData> RoomInfoDataList; // Replace 'EntityType' to an actual type that is serializable.
	//public List<EntityType> RecipeDataList; // Replace 'EntityType' to an actual type that is serializable.
	//public List<EntityType> BuffDataList; // Replace 'EntityType' to an actual type that is serializable.
	//public List<EntityType> EnemyDataList; // Replace 'EntityType' to an actual type that is serializable.
	//public List<EntityType> EnvironmentDataList; // Replace 'EntityType' to an actual type that is serializable.

	Dictionary<Type, IList> dataListT2LDic;
	private void InitDataDic()
	{
		if (dataListT2LDic != null) return;

		dataListT2LDic = new Dictionary<Type, IList>();
		// 好像只能手动添加类型
		Add2Dic(typeof(TrapData), TrapDataList);
        Add2Dic(typeof(MaterialData), MaterialDataList);
        Add2Dic(typeof(WeaponData), WeaponDataList);
        Add2Dic(typeof(FoodData), FoodDataList);
        Add2Dic(typeof(FurnitureInfoData), FurnitureInfoDataList);
        Add2Dic(typeof(RoomInfoData), RoomInfoDataList);
    }

	public List<T> GetDataList<T>() where T : BagItemInfoBase
	{
		InitDataDic();

        if (dataListT2LDic.TryGetValue(typeof(T), out var list))
		{
			return list as List<T>;
		}
		else
		{
			Debug.LogWarning($"无类型{typeof(T).Name}，请确定类型无误");
			return null;
		}

    }
	private void Add2Dic(Type type, IList list)
	{
        dataListT2LDic.Add(type, list);
    }
	
}
