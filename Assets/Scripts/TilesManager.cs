using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TilesManager : MonoBehaviour
{
    public event Action OnTilesDestroyed;
    public static TilesManager Instance;
    public event Action<List<TileProduct>> OnListUpdated;
    [SerializeField] private SelectionUI selectionUI;
    private List<TileProduct> tilesList = new List<TileProduct>();
    private Dictionary<TileName,int> tilesCountDict = new Dictionary<TileName, int>();
    private TileName ThreeMatchedTileName;
    // hardcoded size
    private readonly int maxTilesSize = 7;

    [SerializeField] private bool hasPending = false;

    private void Awake(){
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);
    }

    private void OnEnable() {
        tilesList.Clear();
        tilesCountDict.Clear();
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

        if (tilesList.Count >= maxTilesSize){
            GameManager.Instance.inputReader.enabled = false;
        }

        // increment count
        IncrementTileCount(tileName);
        // check for matching three
        var isThreeMatched = IsMatchingThree(tileName);
        if (!hasPending){
            hasPending = isThreeMatched;
        }
        if (isThreeMatched){
            ThreeMatchedTileName = tileName;
        }
        // if there is no matching three and no more slot to hold
        else if(!isThreeMatched && tilesList.Count >= maxTilesSize && !hasPending){
            // call end game
            GameManager.Instance.EndGame(false);
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
        hasPending = false;
        if (tilesList.Count < maxTilesSize){
            GameManager.Instance.inputReader.enabled = true;
        }
        OnTilesDestroyed?.Invoke();
    }

    private void UpdateUI(){
        OnListUpdated?.Invoke(tilesList);
    }

    private async void DestroyMatchingThreeTiles(){
        if (!IsMatchingThree(ThreeMatchedTileName)) return;
        DestroyTiles(ThreeMatchedTileName);
        tilesCountDict[ThreeMatchedTileName] = 0; // reset counter
        ThreeMatchedTileName = TileName.None;
        await Task.Delay(500);
        UpdateUI();
    }

    public Vector3 GetTileUISlotPosition(TileName tileName){
        int idx = tilesList.GetIndexOfTile(tileName);
        if (idx == -1){
            idx = tilesList.Count;
        }
        return selectionUI.GetHolderUILocation(idx);
    }
}
