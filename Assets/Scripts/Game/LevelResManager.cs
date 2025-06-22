using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace KidGame.Core
{
    /// <summary>
    /// 游戏资源管理器 管理材料生成回收等
    /// </summary>
    public class LevelResManager : Singleton<LevelResManager>
    {
        private LevelResData _resData;
        public void Init()
        {

        }
        public void Discard()
        {

        }

        public void InitLevelRes(LevelResData resData)
        {
            _resData = resData;
            List<MaterialItem> tmpMaterialItemList = new List<MaterialItem>();

            //给有材料的家具初始化
            foreach (var mapFurniture in MapManager.Instance.mapFurnitureList)
            {
                tmpMaterialItemList.Clear();
                //该家具存在和材料的映射，说明该家具里有材料
                Furniture2MaterialMapping mapping = _resData.f2MMappingList.Find(x => x.furnitureData.Equals(mapFurniture.mapFurnitureData.furnitureData));
                if (mapping != null)
                {
                    foreach (var item in mapping.materialDataList)
                    {
                        tmpMaterialItemList.Add(new MaterialItem(item.materialData, Random.Range(item.randomAmount_min, item.randomAmount_max+1)));
                    }
                    mapFurniture.Init(true, tmpMaterialItemList);
                }

            }


            //生成房间里可以直接捡的材料

            foreach (var mapping in _resData.r2MMappingList)
            {
                List<MapTile> tmpTileList = MapManager.Instance.mapTileDic[mapping.roomType];
                List<MapFurniture> tmpFurnitureList = MapManager.Instance.mapFurnitureDic[mapping.roomType];
                foreach (var mapTile in tmpTileList)
                {
                    //这个瓦片上没有墙和家具
                    if(!tmpFurnitureList.Find(x=>x.mapFurnitureData.mapPosList.Contains(mapTile.mapTileData.mapPos))
                        && MapManager.Instance.mapWallList.Find(x=>x.mapWallData.mapPosList.Contains(mapTile.mapTileData.mapPos)))
                    {

                    }
                }
            }

            #region Delete
            ////生成房间里可以直接捡的材料
            //int xMin = 999, xMax = -1, yMin = 999, yMax = -1;
            //int canSpawnTileAmount = 0;

            //foreach (var mapping in _resData.r2MMappingList)
            //{
            //    List<MapTile> tmpTileList = MapManager.Instance.mapTileDic[mapping.roomType];
            //    xMin = 999;
            //    xMax = -1;
            //    yMin = 999;
            //    yMax = -1;
            //    //获得当前房间的边界范围 在这些范围上的瓦片不能生成材料
            //    foreach (var mapTile in tmpTileList)
            //    {
            //        if (mapTile.mapTileData.mapPos.x < xMin)
            //            xMin = mapTile.mapTileData.mapPos.x;
            //        if (mapTile.mapTileData.mapPos.x > xMax)
            //            xMax = mapTile.mapTileData.mapPos.x;
            //        if (mapTile.mapTileData.mapPos.y < yMin)
            //            yMin = mapTile.mapTileData.mapPos.y;
            //        if (mapTile.mapTileData.mapPos.y > yMax)
            //            yMax = mapTile.mapTileData.mapPos.y;
            //    }
            //    canSpawnTileAmount = 0;
            //    foreach (var mapTile in tmpTileList)
            //    {
            //        if(mapTile.mapTileData.mapPos.x > xMin && mapTile.mapTileData.mapPos.x < xMax 
            //            && mapTile.mapTileData.mapPos.y > yMin && mapTile.mapTileData.mapPos.y < yMax)
            //        {
            //            canSpawnTileAmount++;
            //        }
            //    }

            //    int total = 0;
            //    foreach(var resCfg in mapping.materialDataList)
            //    {

            //    }
            //}
            #endregion

        }
    }
}
