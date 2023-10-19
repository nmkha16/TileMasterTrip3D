using System.Collections;
using System.Collections.Generic;
using Factory;
using UnityEngine;

public class TileProduct : MonoBehaviour, IProduct, IHoverable, IClickable
{
    private Camera mainCamera;
    [SerializeField] private TileName tileName;
    [SerializeField] private Sprite sprite;
    public string productName { get => this.tileName.ToString(); }

    public GameObject gameObjectProduct => this.gameObject;

    private void Awake(){
        mainCamera = Camera.main;
    }

    public void Initialize(TileName name, Sprite sprite){
        this.tileName = name;
        this.sprite = sprite;
    }

    private void Update(){
        Vector3 pos = mainCamera.WorldToViewportPoint (transform.position);
		pos.x = Mathf.Clamp(pos.x,0.2f,0.8f);
		pos.y = Mathf.Clamp(pos.y,0.2f,0.8f);
		transform.position = Camera.main.ViewportToWorldPoint(pos);
    }

    public void Hover(){
        // TODO: draw something on screen to let the user know it is being selected
    }

    public void Click(){
        // TODO: implement click logic
        // requires command pattern to undo
    }
}

