using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DataManagerUI : MonoBehaviour
{
    [SerializeField] private TMP_Text starText;
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text playCanvasHeader;
    [SerializeField] private TMP_Text mainMenuLevelNumberText;

    public void UpdateStatUI(UserData userData){
        starText.text = userData.star.ToString();
        goldText.text = userData.gold.ToString();
        mainMenuLevelNumberText.text = (userData.level + 1).ToString();
        playCanvasHeader.text = "Level " + (userData.level + 1).ToString();
    }
}
