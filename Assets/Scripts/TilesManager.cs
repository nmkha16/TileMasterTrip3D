using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesManager : MonoBehaviour
{
    [SerializeField] private SelectionUI selectionUI;
    private List<TileProduct> selectedTiles = new List<TileProduct>();
    private void Start() {
        TileProduct.OnMoveCompleted += AddToList;
    }

    private void OnDestroy() {
        TileProduct.OnMoveCompleted -= AddToList;
    }

    private void AddToList(TileProduct tileProduct){
        selectedTiles.Add(tileProduct);
    }
}
