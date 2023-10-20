using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Transform tileHolderUI;
    [SerializeField] private ObjectFactory[] factories;
    private ObjectFactory factory;
    public Level currentLevel;
    private Camera mainCamera;

    [SerializeField] private MapDataScriptableObject data;

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
        // TODO: spawn tiles according to data
        factory = factories[0];
        var currentData = data.mapData.maps[0].tilePool;
        for(int i = 0; i < 42; ++i){
            Vector3 pos = mainCamera.ScreenToWorldPoint(new Vector3(Random.Range(0,Screen.width), Random.Range(Screen.height*0.55f,Screen.height), 0));
            pos.y = 1.75f;
            // Vector3 pos = Random.insideUnitSphere*1.25f;
            // pos.y = 1.5f;
            var tileProduct = factory.GetProduct(pos);
            tileProduct.gameObjectProduct.GetComponent<TileProduct>().Initialize(TileName.Tile_01,currentData[0].sprite);
        }
    }

    private void SelectLevel(Level level){

    }

    private void NextLevel(){

    }

    private void Retry(){

    }
    
    private void ActivateState(GameState state){
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
}
