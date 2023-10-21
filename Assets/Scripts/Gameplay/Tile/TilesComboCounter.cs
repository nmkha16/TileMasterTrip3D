using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TilesComboCounter : MonoBehaviour
{
    public Action<int> OnComboUpdated;
    public Action<float> OnTimePercentageElapsed;
    [SerializeField] private TilesComboCounterUI tilesComboCounterUI;
    [SerializeField] private float comboDuration = 5f;
    private int multiplier = 1;

    [Header("Data Manager")]
    [SerializeField] private DataManager dataManager;
    private TilesManager tilesManager;

    private float elapsed = 0f;
    private void Awake() {
        tilesManager = GetComponent<TilesManager>();
    }

    private void Start(){
        tilesManager.OnTilesDestroyed += ComboUpgrade;
        OnComboUpdated += tilesComboCounterUI.UpdateMultiplierUI;
        OnTimePercentageElapsed += tilesComboCounterUI.UpdateDurationUI;
    }

    private void OnDestroy() {
        tilesManager.OnTilesDestroyed -= ComboUpgrade;
        OnComboUpdated -= tilesComboCounterUI.UpdateMultiplierUI;
        OnTimePercentageElapsed -= tilesComboCounterUI.UpdateDurationUI;
    }

    private void Update(){
        if (multiplier <= 1 )return;
        elapsed -= Time.deltaTime;
        OnTimePercentageElapsed?.Invoke(elapsed/comboDuration);
        if (elapsed < 0){
            multiplier = 1;
            OnComboUpdated?.Invoke(multiplier);
        }
    }

    private void AddStarsToInGameStars(){
        dataManager.AddIngameStars(3 * multiplier);
    }

    private void ComboUpgrade(){
        AddStarsToInGameStars();
        elapsed = comboDuration;
        multiplier++;
        OnComboUpdated?.Invoke(multiplier);
    }
}
