using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MapData
{
    public List<MapSetting> maps;
}

[Serializable]
public class MapSetting{
    public Level level;
    public int playTime;
    public List<Tile> tilePool;
}

[Serializable]
public class Tile{
    public Sprite sprite;
    [Range(0,1f)] public float chance;
}