using System;
using System.Collections;
using System.Collections.Generic;
using Factory;
using UnityEngine;

public class TileProduct : MonoBehaviour, IProduct, IHoverable, IClickable
{
    public static Action<TileProduct> OnTileSelected;
    [SerializeField] private Outline outline;
    TileProductMove tileProductMove;
    private Camera mainCamera;
    public TileName tileName {get; private set;}
    public Sprite sprite {get; private set;}

    public string productName { get => this.tileName.ToString(); }
    public GameObject gameObjectProduct => this.gameObject;

    // TODO: fix this hardcode tiles holder
    public int maxTilesHolderCount = 7; // hardcoded

    private Vector3 defaultScale;
    private void Awake(){
        mainCamera = Camera.main;
        defaultScale = this.transform.localScale;
    }

    private void Start(){
        tileProductMove = GetComponent<TileProductMove>();
    }

    public void Initialize(TileName name, Sprite sprite){
        this.tileName = name;
        this.sprite = sprite;
        
        this.transform.rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(-90f,90f),UnityEngine.Random.Range(-90f,90f),0f));
    }

    // constraint Tile within camera view
    private void FixedUpdate(){
        Vector3 pos = mainCamera.WorldToViewportPoint (transform.position);
		pos.x = Mathf.Clamp(pos.x,0.2f,0.8f);
		pos.y = Mathf.Clamp(pos.y,0.1f,0.9f);
		transform.position = Camera.main.ViewportToWorldPoint(pos);
    }

    public void Hover(){
        outline.enabled = true;
        this.gameObject.transform.localScale = defaultScale * 1.20f;
    }

    public void CancelHover(){
        outline.enabled = false;
        this.gameObject.transform.localScale = defaultScale;
    }

    public void Click(){
        CancelHover();
        // calculate ui screenpoint to world position
        var pos = GameManager.Instance.tileHolderUI.position;
        pos = mainCamera.ScreenToWorldPoint(pos);
        // invoke move command
        ICommand command = new MoveCommand(this,this.transform.position,pos);
        CommandInvoker.ExecuteCommand(command);
        OnTileSelected?.Invoke(this);
    }

    public void DoMove(Vector3 destination){
        tileProductMove.Move(destination);
    }
}

