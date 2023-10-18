using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MapData
{
    public List<MapSettings> maps;
}

[Serializable]
public class MapSettings{
    public Level level;
    public int playTime;
    public List<Sprite> spritePool;
}