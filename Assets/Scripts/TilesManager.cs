using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class TilesManager : MonoBehaviour
{
    public static TilesManager Instance;
    public event Action<List<TileProduct>> OnListUpdated;
    [SerializeField] private List<TileProduct> tilesList = new List<TileProduct>();
    private Dictionary<TileName,int> tilesCountDict = new Dictionary<TileName, int>();
    private TileName matchingThreeTileName;

    private void Awake(){
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);
    }

    private void Start() {
        TileProduct.OnTileSelected += PushToList;
        TileProductMove.OnMoveCompleted += UpdateUI;
        SelectionUI.OnUIUpdated += DestroyMatchingThreeTiles;
    }

    private void OnDestroy() {
        TileProduct.OnTileSelected -= PushToList;
        TileProductMove.OnMoveCompleted -= UpdateUI;
        SelectionUI.OnUIUpdated -= DestroyMatchingThreeTiles;
    }

    private void PushToList(TileProduct tileProduct){
        var tileName = tileProduct.tileName;
        int idx = tilesList.GetIndexOfTile(tileName);
        if (idx == -1){
            tilesList.Add(tileProduct);
        }
        else tilesList.Insert(idx, tileProduct);
        // increment count
        IncrementTileCount(tileName);
        if (IsMatchingThree(tileName)){
            matchingThreeTileName = tileName;
        }
    }

    public void RemoveFromList(TileProduct tileProduct){
        tilesList.Remove(tileProduct);
        DecrementTileCount(tileProduct.tileName);
        UpdateUI();
    }

    private void IncrementTileCount(TileName tileName){
        if (tilesCountDict.ContainsKey(tileName)){
            tilesCountDict[tileName]++;
        }
        else{
            tilesCountDict[tileName] = 1;
        }
    }

    private void DecrementTileCount(TileName tileName){
        if (tilesCountDict.ContainsKey(tileName)){
            tilesCountDict[tileName]--;
        }
    }

    private bool IsMatchingThree(TileName tileName){
        if (tilesCountDict.ContainsKey(tileName)){
            return tilesCountDict[tileName] == 3;
        }
        return false;
    }

    private void DestroyTiles(TileName tileName){
        int idx = tilesList.GetIndexOfTile(tileName);
        for(int i = idx; i < idx+3; ++i){
            var tileProduct = tilesList[idx];
            Destroy(tileProduct.gameObject);
            tilesList.Remove(tileProduct);
        }
    }

    private void UpdateUI(){
        OnListUpdated?.Invoke(tilesList);
    }

    private async void DestroyMatchingThreeTiles(){
        if (!IsMatchingThree(matchingThreeTileName)) return;
        DestroyTiles(matchingThreeTileName);
        tilesCountDict[matchingThreeTileName] = 0; // reset counter
        matchingThreeTileName = TileName.None;
        await Task.Delay(500);
        UpdateUI();
    }
}
