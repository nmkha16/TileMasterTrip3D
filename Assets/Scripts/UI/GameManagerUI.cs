using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerUI : MonoBehaviour
{
    [SerializeField] private GameObject menuCanvas;
    [SerializeField] private GameObject[] inGameCanvas;
    [SerializeField] private GameObject pauseInGameCanvas;
    // win canvas
    [SerializeField] private GameObject winCanvas;
    // lose canvas
    [SerializeField] private GameObject loseCanvas;
    [SerializeField] private Button playOnButton;
    // playcanvas
    [SerializeField] private GameObject playCanvas;

    [Header("Timer")]
    [SerializeField] private Timer timer;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private Slider timerProgressBar;

    private void Start(){
        GameManager.Instance.OnUpdateUI += ActivateState;
        timer.OnTimeElapsed += UpdateTimeUI;
        timer.OnCountdownReset += ResetCountdown;

        ToggleInGameCanvas(false);
        menuCanvas.SetActive(true);
    }

    private void OnDestroy() {
        GameManager.Instance.OnUpdateUI -= ActivateState;
        timer.OnTimeElapsed -= UpdateTimeUI;
        timer.OnCountdownReset -= ResetCountdown;
    }

    private void ActivateState(GameState state){
        switch (state)
        {
            case GameState.Menu:
                ToggleInGameCanvas(false);
                menuCanvas.SetActive(true);
                break;
            case GameState.Play:
                ToggleInGameCanvas(true);
                menuCanvas.SetActive(false);
                break;
            case GameState.Pause:
                pauseInGameCanvas.SetActive(true);
                break;
            case GameState.Win:
                winCanvas.SetActive(true);
                break;
            case GameState.Lose:
                loseCanvas.SetActive(true);
                break;
        }
    }

    private void ToggleInGameCanvas(bool toggle){
        foreach(var canvas in inGameCanvas){
            canvas.SetActive(toggle);
        }
    }

    #region Lose Canvas
    public void ValidatePlayOnButton(UserData userData){
        
        //playOnButton.interactable = userData.gold >= GameManager.Instance.playOnCost;
    }
    #endregion

    #region  Timer
    private void SetText(string content)
    {
        timerText.text = content;
    }

    private void SetTimeProgressBar(float percentage){
        timerProgressBar.value = percentage;
    }

    private void SetTextColor(Color color)
    {
        timerText.color = color;
    }

    private void ResetCountdown(){
        SetText("0:00");
    }

    private void UpdateTimeUI(float timer, float percentage){
        string minutes = ((int)timer / 60).ToString("00"); 
        string seconds = (timer % 60).ToString("00");

        SetText(string.Format("{0}:{1}", minutes, seconds));
        SetTextColor(percentage < .1f ? Color.red : Color.white);

        SetTimeProgressBar(percentage);
    }
    #endregion
}
