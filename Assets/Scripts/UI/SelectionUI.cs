using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionUI : MonoBehaviour
{
    public event Action OnUIUpdated;
    [SerializeField] private List<Image> tilesUI;
    [SerializeField] private TilesManager tilesManager;

    private void Start(){
        tilesManager.OnListUpdated += UpdateUI;
        GameManager.Instance.OnGameEnded += ClearListUI;
    }

    private void OnDestroy() {
        tilesManager.OnListUpdated -= UpdateUI;
        GameManager.Instance.OnGameEnded -= ClearListUI;
    }

    public void UpdateUI(List<TileProduct> list){
        if (list.Count == 0){
            foreach(var tileUI in tilesUI){
                tileUI.sprite = null;
                tileUI.gameObject.SetActive(false);
            }
        }

        else{
            for(int i = 0; i < tilesUI.Count; ++i){
                if (i < list.Count){
                    tilesUI[i].sprite = list[i]?.spriteRenderer.sprite;
                    tilesUI[i].gameObject.SetActive(true);
                }
                else{
                    tilesUI[i].sprite = null;
                    tilesUI[i].gameObject.SetActive(false);
                }
            }
        }
        OnUIUpdated?.Invoke();
    }

    private void ClearListUI(){
        foreach(var tileUI in tilesUI){
            tileUI.sprite = null;
            tileUI.gameObject.SetActive(false);
        }
    }

    // ScreenPoint Position
    public Vector3 GetHolderUILocation(int idx){
        return tilesUI[idx].rectTransform.position;
    }
}
