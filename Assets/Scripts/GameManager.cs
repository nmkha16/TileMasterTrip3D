using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event Action OnGameStarted;
    public event Action OnGameEnded;
    public event Action OnReturnedToMenu;
    public event Action<GameState> OnUpdateUI;
    public static GameManager Instance;
    public InputReader inputReader;
    [Header("Data Manager")]
    private DataManager dataManager;
    [Header("Map Data")]
    [SerializeField] private MapDataScriptableObject data;
    public int playOnCost = 1000;
    [Header("Factories")]
    [SerializeField] private ObjectFactory[] factories;
    private ObjectFactory factory;
    [Header("Level")]
    public int currentLevel;
    private Camera mainCamera;

    [Header("Ground Material")]
    [SerializeField] private Renderer groundRenderer;
    private Material groundMaterial;

    [Header("Timer")]
    [SerializeField] private Timer timer;
    [SerializeField] private TilesComboCounter tilesComboCounter;

    [SerializeField] private GameState state;

    [Header("Tiles Manager")]
    private TilesManager tilesManager;
    [SerializeField] private int totalTilesCount;
    public int TotalTilesCount{
        get{
            return totalTilesCount;
        }
        set{
            totalTilesCount = value;
            if (totalTilesCount == 0){
                // call end game with win if is playing
                if (State == GameState.Play){
                    EndGame(true);
                }
            }
        }
    }

    public GameState State{
        get{
            return state;
        }
        private set{
            state = value;
            ActivateState();
        }
    }
    

    private void Awake(){
        mainCamera = Camera.main;

        if (Instance == null){
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else{
            Destroy(this.gameObject);
        }

        inputReader = GetComponent<InputReader>();
        tilesManager = GetComponentInChildren<TilesManager>();
        dataManager = GetComponentInChildren<DataManager>();
    }

    private void Start(){
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        
        groundMaterial = groundRenderer.material;

        tilesManager.OnTilesDestroyed +=ReduceTilesCount;
        timer.OnCountdownFinished += Timeout;
        
        State = GameState.Menu;
    }

    private void OnDestroy() {
        tilesManager.OnTilesDestroyed -=ReduceTilesCount;
        timer.OnCountdownFinished -= Timeout;
    }

    public void StartGame(){
        OnGameStarted?.Invoke();
        TotalTilesCount = 0;
        tilesManager.enabled = true;
        State = GameState.Play;

        // get user current level progress
        var currentLevel = dataManager.GetLevel();

        // spawn flower
        SpawnFlower(currentLevel);
        // set timer
        var duration = data.mapData.maps[currentLevel].playTime;
        timer.StopCountdown();
        timer.StartCountdown(duration);

    }

    public void ReturnToMenu(){
        OnGameEnded?.Invoke();
        OnReturnedToMenu?.Invoke();
        State = GameState.Menu;
    }

    public void Pause(){
        State = GameState.Pause;
    }

    public void UnPause(){
        State = GameState.Play;
    }

    public void Continue(){
        // no need to check again, we validate playon button should be enabled or not based on update event
        //if (dataManager.GetGold() >= playOnCost){
        dataManager.TransactGold(-playOnCost);
        //}
        // undo all previous move before continue
        CommandInvoker.UndoAllCommands();
        State = GameState.Play;
    }

    public void Retry(){
        OnGameEnded?.Invoke();
        StartGame();
    }

    public void EndGame(bool isWon){
        State = isWon ? GameState.Win : GameState.Lose;
        inputReader.enabled = false;
        tilesManager.enabled = false;
        OnGameEnded?.Invoke();
    }

    private void Timeout(){
        EndGame(false);
    }

    private void ActivateState(){
        switch (State)
        {
            case GameState.Menu:
                timer.StopCountdown();
                inputReader.enabled = false;
                groundMaterial.SetInt("_ShouldGradientNoiseColor",1);
                break;
            case GameState.Play:
                timer.ContinueCountdown();
                inputReader.enabled = true;
                tilesComboCounter.enabled = true;
                groundMaterial.SetInt("_ShouldGradientNoiseColor",0);
                break;
            case GameState.Pause:
                timer.PauseCountdown();
                inputReader.enabled = false;
                tilesComboCounter.enabled = false;
                break;
            case GameState.Win:
                SoundManager.Instance.PlayOneShotSound(SoundId.s_win);
                timer.PauseCountdown();
                inputReader.enabled = false;
                dataManager.IncrementLevelProgress();
                dataManager.AddIngameStarsToUserData();
                //ReturnToMenu();
                break;
            case GameState.Lose:
                SoundManager.Instance.PlayOneShotSound(SoundId.s_lose);
                timer.PauseCountdown();
                inputReader.enabled = false;
                //ReturnToMenu();
                break;
        }
        OnUpdateUI?.Invoke(State);
    }

    private void SpawnFlower(int level){
        
        factory = factories[0]; // get factory, well technically we only have one factory
        var currentData = data.mapData.maps[(int)level].tilePool;

        for(int i = 0; i < currentData.Count; ++i){
            // roll odd to decide whether to spawn
            float tileOdd = currentData[i].chance;
            float k = UnityEngine.Random.Range(0f,1f);
            if (tileOdd <= k) continue;

            int setsOfThreeCount = UnityEngine.Random.Range(2,4);
            while(setsOfThreeCount-- >= 0){
                // spawn 3 tiles of the same type
                for (int j = 0; j < 3; j++){
                    Vector3 pos = mainCamera.ScreenToWorldPoint(new Vector3(UnityEngine.Random.Range(0,Screen.width), UnityEngine.Random.Range(Screen.height*0.3f,Screen.height*0.9f), 0));
                    pos.y = UnityEngine.Random.Range(1f,1.75f);
                    var tileProduct = factory.GetProduct(pos);
                    tileProduct.gameObjectProduct.GetComponent<TileProduct>().Initialize(currentData[i].name,currentData[i].sprite);
                    TotalTilesCount ++;
                }
            }
        }
    }
    
    private void ReduceTilesCount(){
        TotalTilesCount -= 3;
    }
}
