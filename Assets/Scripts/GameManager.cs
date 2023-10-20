using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event Action<GameState> OnUpdateUI;
    public static GameManager Instance;

    [Header("Map Data")]
    [SerializeField] private MapDataScriptableObject data;
    [Header("Factories")]
    [SerializeField] private ObjectFactory[] factories;
    private ObjectFactory factory;
    [Header("Level")]
    public Level currentLevel;
    private Camera mainCamera;

    [Header("Ground Material")]
    [SerializeField] private Renderer groundRenderer;
    private Material groundMaterial;

    private GameState state;
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
    }

    private void Start(){
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        
        groundMaterial = groundRenderer.material;
    }

    public void StartGame(){
        State = GameState.Play;
        SpawnFlower(Level.Level_1);
    }

    public void NextLevel(){

    }

    public void Retry(){

    }

    public void EndGame(bool isWon){
        State = isWon ? GameState.Win : GameState.Lose;
    }

    private void ActivateState(){
        switch (State)
        {
            case GameState.Menu:
                groundMaterial.SetInt("_ShouldGradientNoiseColor",1);
                break;
            case GameState.Play:
                groundMaterial.SetInt("_ShouldGradientNoiseColor",0);
                break;
            case GameState.Win:
                break;
            case GameState.Lose:
                break;
        }
        OnUpdateUI?.Invoke(State);
    }

    private void SpawnFlower(Level level){
        factory = factories[0]; // get factory, well technically we only have one factory
        var currentData = data.mapData.maps[(int)level].tilePool;

        for(int i = 0; i < currentData.Count; ++i){
            // roll odd to decide whether to spawn
            float tileOdd = currentData[i].chance;
            float k = UnityEngine.Random.Range(0f,1f);
            if (tileOdd <= k) continue;

            int setsOfThreeCount = UnityEngine.Random.Range(2,5);
            while(setsOfThreeCount-- >= 0){
                // spawn 3 tiles of the same type
                for (int j = 0; j < 3; j++){
                    Vector3 pos = mainCamera.ScreenToWorldPoint(new Vector3(UnityEngine.Random.Range(0,Screen.width), UnityEngine.Random.Range(Screen.height*0.4f,Screen.height), 0));
                    pos.y = UnityEngine.Random.Range(1f,1.75f);
                    var tileProduct = factory.GetProduct(pos);
                    tileProduct.gameObjectProduct.GetComponent<TileProduct>().Initialize(currentData[i].name,currentData[i].sprite);
                }
            }
        }
    }
    
}
