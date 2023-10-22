using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class DataManagerUI : MonoBehaviour
{
    [Header("Main Menu")]
    [SerializeField] private TMP_Text starText;
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text playCanvasHeader;
    [SerializeField] private TMP_Text mainMenuLevelNumberText;

    [Header("Ingame")]
    [SerializeField] private TMP_Text ingameStarsText;
    [Header("Ingame Win")]
    [SerializeField] private TMP_Text ingameWinStarsText;

    public void UpdateStatUI(UserData userData){
        starText.text = userData.star.ToString();
        goldText.text = userData.gold.ToString();
        mainMenuLevelNumberText.text = (userData.level + 1).ToString();
        playCanvasHeader.text = "Level " + (userData.level + 1).ToString();
    }

    public void UpdateIngameStar(int stars){
        ingameStarsText.text = stars.ToString();
        ingameWinStarsText.text = "+" + stars.ToString();
    }
}
