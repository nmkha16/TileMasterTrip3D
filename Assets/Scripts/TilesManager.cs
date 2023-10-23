using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

// nmkha: messy code, need a better way to do this :(
public class TilesManager : MonoBehaviour
{
    public Action OnTilesDestroyed;
    public static TilesManager Instance;
    public event Action<List<TileProduct>> OnListUpdated;
    [SerializeField] private SelectionUI selectionUI;
    private List<TileProduct> tilesList = new List<TileProduct>();
    private Dictionary<TileName,int> tilesCountDict = new Dictionary<TileName, int>();
    private TileName currentMatchingTile;
    public bool isRemoving = false;

    [SerializeField] private GameObject explosionEffect;

    private Vector3 explosionPoint;

    // hardcoded size
    private readonly int maxTilesSize = 7;

    private IClickable currentClickable;


    private void Awake(){
        if (Instance == null) Instance = this;
        else Destroy(this.gameObject);
    }

    private void Start() {
        TileProduct.OnTileClicked += SelectTile;
        GameManager.Instance.inputReader.OnSelectPerformed += TriggerClick;
        GameManager.Instance.OnGameStarted += ClearTiles;
        TileProductMove.OnTileSelected += PushToList;
        TileProductMove.OnMoveCompleted += UpdateUI;
        selectionUI.OnUIUpdated += DestroyMatchingThreeTiles;
        selectionUI.OnUIUpdated += CheckLoseGame;
    }

    private void OnDestroy() {
        TileProduct.OnTileClicked -= SelectTile;
        GameManager.Instance.inputReader.OnSelectPerformed -= TriggerClick;
        GameManager.Instance.OnGameStarted -= ClearTiles;
        TileProductMove.OnTileSelected -= PushToList;
        TileProductMove.OnMoveCompleted -= UpdateUI;
        selectionUI.OnUIUpdated -= DestroyMatchingThreeTiles;
        selectionUI.OnUIUpdated -= CheckLoseGame;
    }

    private void SelectTile(IClickable clickable){
        this.currentClickable = clickable;
    }

    private void TriggerClick(){
        currentClickable?.Click();
    }

    private void PushToList(TileProduct tileProduct){
        var tileName = tileProduct.tileName;
        int idx = tilesList.GetIndexOfTile(tileName);
        if (idx == -1){
            tilesList.Add(tileProduct);
        }
        else tilesList.Insert(idx, tileProduct);


        // should block player input when max size is reached, wait until further 3 matched tiles remove
        if (tilesList.Count >= maxTilesSize){
            GameManager.Instance.inputReader.enabled = false;
        }

        // increment count
        IncrementTileCount(tileName);
        // check for matching three
        var isThreeMatched = IsMatchingThree(tileName);
        if (isThreeMatched){
            currentMatchingTile = tileName;
            GameManager.Instance.inputReader.enabled = false;
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

    private Task DestroyTiles(TileName tileName) {
        int idx = tilesList.GetIndexOfTile(tileName);
        if (idx == -1){
            Debug.LogWarning("Warning: attempt to remove tiles that isnt existed in list");
            return Task.CompletedTask;
        }
        explosionPoint = GetExplosionPoint(idx);
        for(int i = idx; i < idx+3; ++i){
            var tileProduct = tilesList[idx];
            Destroy(tileProduct.gameObject);
            tilesList.Remove(tileProduct);
        }
        tilesCountDict[tileName] = 0;
        OnTilesDestroyed?.Invoke();
        currentMatchingTile = TileName.None;
        return Task.CompletedTask;
    }

    private void UpdateUI(){
        OnListUpdated?.Invoke(tilesList);
    }

    private async void DestroyMatchingThreeTiles(){
        if (isRemoving || currentMatchingTile == TileName.None) return;
        isRemoving = true;
        {
            await DestroyTiles(currentMatchingTile);
            await Task.Delay(400);
            SoundManager.Instance.PlayOneShotSound(SoundId.s_explode_tiles);
            GameManager.Instance.inputReader.enabled = true;
            SetExplosion();
            UpdateUI();
        }
        isRemoving = false;
    }

    public void SetExplosion(){
        // set explosion go to explosion point
        explosionEffect.transform.position = explosionPoint;

        // enable explosion
        explosionEffect.SetActive(true);
        CloseExplosion(500);
    }

    public async void CloseExplosion(int milliseconds){
        await Task.Delay(milliseconds);
        explosionEffect.SetActive(false);
    }

    public Vector3 GetTileUISlotPosition(TileName tileName){
        int idx = tilesList.GetIndexOfTile(tileName);
        if (idx == -1){
            idx = tilesList.Count;
        }
        return selectionUI.GetHolderUILocation(idx);
    }

    public Vector3 GetExplosionPoint(int idx){
        // +1 move to middle
        return selectionUI.GetHolderUILocation(idx+1);
    }

    private void ClearTiles(){
        tilesList.Clear();
        tilesCountDict.Clear();
        currentMatchingTile = TileName.None;
        currentClickable = null;
    }

    
    private void CheckLoseGame(){
        // if there is no matching three and no more slot to hold
        if(tilesList.Count >= maxTilesSize && currentMatchingTile == TileName.None){
            // call end game
            // update UI so we can we get the final slot on UI
            //UpdateUI();
            GameManager.Instance.EndGame(false);
        }
    }

}
