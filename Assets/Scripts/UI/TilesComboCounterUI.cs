using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TilesComboCounterUI : MonoBehaviour
{
    [SerializeField] private TMP_Text comboMultiplierText;
    [SerializeField] private Slider durationSlider;

    public void UpdateMultiplierUI(int multiplier){
        comboMultiplierText.text = "x" + multiplier.ToString();
    }

    public void UpdateDurationUI(float percentage){
        durationSlider.value = percentage;
    }
}
