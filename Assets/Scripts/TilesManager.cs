using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesManager : MonoBehaviour
{
    [SerializeField] private SelectionUI selectionUI;
    [SerializeField] private List<TileProduct> selectedTiles = new List<TileProduct>();
    private void Start() {
        TileProduct.OnMoveCompleted += PushToList;
    }

    private void OnDestroy() {
        TileProduct.OnMoveCompleted -= PushToList;
    }

    // will remove if identical item found in list
    // helpful to undo command
    private void PushToList(TileProduct tileProduct){
        if (selectedTiles.Contains(tileProduct)){
            selectedTiles.Remove(tileProduct);
        }
        else selectedTiles.Add(tileProduct);
    }
}
