using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Header("Map Data")]
    [SerializeField] private MapDataScriptableObject data;
    [Header("Factories")]
    [SerializeField] private ObjectFactory[] factories;
    private ObjectFactory factory;
    [Header("Level")]
    public Level currentLevel;
    private Camera mainCamera;


    public GameState state {
        get{
            return state;
        }
        private set{
            state = value;
            ActivateState(state);
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
        
        SpawnFlower(Level.Level_1);
    }

    private void SelectLevel(Level level){

    }

    private void NextLevel(){

    }

    private void Retry(){

    }

    public void EndGame(bool isWon){
        ActivateState(isWon ? GameState.Win : GameState.Lose);
    }

    private void ActivateState(GameState state){
        this.state = state;
        switch (state)
        {
            case GameState.Menu:
                break;
            case GameState.Win:
                break;
            case GameState.Lose:
                break;
        }
    }

    private void SpawnFlower(Level level){
        factory = factories[0]; // get factory, well technically we only have one factory
        var currentData = data.mapData.maps[(int)level].tilePool;

        for(int i = 0; i < currentData.Count; ++i){
            // roll odd to decide whether to spawn
            float tileOdd = currentData[i].chance;
            float k = Random.Range(0f,1f);
            if (tileOdd <= k) continue;

            int setsOfThreeCount = Random.Range(2,5);
            while(setsOfThreeCount-- >= 0){
                // spawn 3 tiles of the same type
                for (int j = 0; j < 3; j++){
                    Vector3 pos = mainCamera.ScreenToWorldPoint(new Vector3(Random.Range(0,Screen.width), Random.Range(Screen.height*0.4f,Screen.height), 0));
                    pos.y = Random.Range(1f,1.75f);
                    var tileProduct = factory.GetProduct(pos);
                    tileProduct.gameObjectProduct.GetComponent<TileProduct>().Initialize(currentData[i].name,currentData[i].sprite);
                }
            }
        }
    }
    
}
