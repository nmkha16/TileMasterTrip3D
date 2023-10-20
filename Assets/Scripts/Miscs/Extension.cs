using System.Collections.Generic;

public static class ExtensionMethods
{
    public static void Swap<T>(this List<T> list, int index1, int index2)
    {
        T temp = list[index1];
        list[index1] = list[index2];
        list[index2] = temp;
    }

    public static void RemoveAllTiles(this List<TileProduct> list, TileName tileName){
        for (int i = 0; i < list.Count; ++i){
            if (list[i].tileName == tileName){
                list.RemoveAt(i--);
            }
        }
    }

    public static int GetIndexOfTile(this List<TileProduct> list, TileName tileName){
        int i = 0;
        foreach(var tile in list){
            if (tile.tileName == tileName) return i;
            i++;
        }
        return -1;
    }
}