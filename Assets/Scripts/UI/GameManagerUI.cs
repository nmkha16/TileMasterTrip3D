using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerUI : MonoBehaviour
{
    [SerializeField] private GameObject inGameCanvas;
    [SerializeField] private GameObject menuCanvas;

    private void Awake(){

    }

    private void Start(){
        GameManager.Instance.OnUpdateUI += ActivateState;
        inGameCanvas.SetActive(false);
        menuCanvas.SetActive(true);
    }

    private void OnDestroy() {
        GameManager.Instance.OnUpdateUI -= ActivateState;
    }

    private void ActivateState(GameState state){
        switch (state)
        {
            case GameState.Menu:
                inGameCanvas.SetActive(false);
                menuCanvas.SetActive(true);
                break;
            case GameState.Play:
                inGameCanvas.SetActive(true);
                menuCanvas.SetActive(false);
                break;
            case GameState.Win:
                break;
            case GameState.Lose:
                break;
        }
    }
}
