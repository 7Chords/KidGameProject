using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExcelAsset]
public class 小鬼当家配置表 : ScriptableObject
{
	public List<TrapEnitiy> 陷阱; // Replace 'EntityType' to an actual type that is serializable.
	public List<ResourceEnitiy> 材料; // Replace 'EntityType' to an actual type that is serializable.
	public List<DebuffEnitiy> 负面效果; // Replace 'EntityType' to an actual type that is serializable.
	public List<FurnitureEntity> 家具; // Replace 'EntityType' to an actual type that is serializable.
	public List<RoomEntity> 房间; // Replace 'EntityType' to an actual type that is serializable.
	public List<Recipe> 配方; // Replace 'EntityType' to an actual type that is serializable.
	public List<Room_F_Relation> 房间_家具; // Replace 'EntityType' to an actual type that is serializable.
	public List<Room_R_Relation> 房间_材料; // Replace 'EntityType' to an actual type that is serializable.
	public List<tool> 手持道具; // Replace 'EntityType' to an actual type that is serializable.
	public List<EnemyEntity> 敌人; // Replace 'EntityType' to an actual type that is serializable.
	//public List<EntityType> 家具状态; // Replace 'EntityType' to an actual type that is serializable.
	//public List<EntityType> 敌人被动技能; // Replace 'EntityType' to an actual type that is serializable.
	//public List<EntityType> 敌人主动技能; // Replace 'EntityType' to an actual type that is serializable.
	//public List<EntityType> 敌人_被动; // Replace 'EntityType' to an actual type that is serializable.
	//public List<EntityType> 敌人_主动; // Replace 'EntityType' to an actual type that is serializable.
	//public List<EntityType> 音效需求文档; // Replace 'EntityType' to an actual type that is serializable.
}
