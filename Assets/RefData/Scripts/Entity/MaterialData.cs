using KidGame;
using KidGame.Core;
using System;

[Serializable]
public class MaterialData: BagItemInfoBase
{
    public string id;
    public string materialName;
    public string materialDesc;
    public string materialIconPath;
    public int rare;
    public string pickSoundName;
    public string pickParticleName;
    public string workSoundName;
    public string workParticleName;
    public UseItemType UseItemType => UseItemType.Material;
    public string Id => id;
}

