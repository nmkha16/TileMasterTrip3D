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
    public TileName name;
    public Sprite sprite;
    [Range(0,1f)] public float chance;

    public Tile(TileName name = TileName.Tile_01, Sprite sprite = null, float chance = 0){
        this.name = name;
        this.sprite = sprite;
        this.chance = chance;
    }
}