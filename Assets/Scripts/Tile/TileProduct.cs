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
    public SpriteRenderer spriteRenderer {get; private set;}

    public string productName { get => this.tileName.ToString(); }
    public GameObject gameObjectProduct => this.gameObject;
    private Rigidbody rgbd;
    private Vector3 defaultScale;
    private void Awake(){
        mainCamera = Camera.main;
        defaultScale = this.transform.localScale;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rgbd = GetComponent<Rigidbody>();
    }

    private void Start(){
        tileProductMove = GetComponent<TileProductMove>();
        Invoke(nameof(IncreaseDrag),2f);
    }

    public void Initialize(TileName name, Sprite sprite){
        this.tileName = name;
        this.spriteRenderer.sprite = sprite;        
        this.transform.rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(-90f,90f),UnityEngine.Random.Range(-90f,90f),0f));
    }

    // constraint Tile within camera view
    private void FixedUpdate(){
        Vector3 pos = mainCamera.WorldToViewportPoint (transform.position);
		pos.x = Mathf.Clamp(pos.x,0.1f,0.9f);
		pos.y = Mathf.Clamp(pos.y,0.2f,0.9f);
		transform.position = Camera.main.ViewportToWorldPoint(pos);
    }

    public void Hover(){
        outline.enabled = true;
        this.gameObject.transform.localScale = defaultScale * 1.1f;
    }

    public void CancelHover(){
        outline.enabled = false;
        this.gameObject.transform.localScale = defaultScale;
    }

    public void Click(){
        CancelHover();
        // calculate ui screenpoint to world position
        var pos = TilesManager.Instance.GetTileUISlotPosition(this.tileName);
        pos = mainCamera.ScreenToWorldPoint(pos);

        // invoke move command
        ICommand command = new MoveCommand(this,this.transform.position,pos);
        CommandInvoker.ExecuteCommand(command);
        OnTileSelected?.Invoke(this);
    }

    public void DoMove(Vector3 destination){
        tileProductMove.Move(destination);
    }

    private void IncreaseDrag(){
        this.rgbd.drag = 50f;
    }
}

