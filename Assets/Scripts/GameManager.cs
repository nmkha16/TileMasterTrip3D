using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private ObjectFactory[] factories;
    private ObjectFactory factory;
    public Level currentLevel;

    private Camera mainCamera;
    private RandomPosition randomPosition;

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
        randomPosition = new RandomPosition();
    }

    private void Start(){
        factory = factories[0];

        for(int i = 0; i < 50; ++i){
        //    Vector2 randomViewPointPosition = randomPosition.OnScreen();
        //    Vector3 randomWorldPointPosition = mainCamera.ScreenToWorldPoint(randomViewPointPosition);
            Vector3 pos = mainCamera.ScreenToWorldPoint(new Vector3(Random.Range(0,Screen.width), Random.Range(0,Screen.height), 0));
            pos.y = UnityEngine.Random.Range(0.5f,2f);
            var tileProduct = factory.GetProduct(pos);
            tileProduct.gameObjectProduct.GetComponent<TileProduct>().Initialize(TileName.Tile_01,null);
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
