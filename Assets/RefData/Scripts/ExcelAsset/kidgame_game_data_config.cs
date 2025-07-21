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
}
