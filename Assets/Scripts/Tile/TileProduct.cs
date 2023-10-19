using System;
using System.Collections;
using System.Collections.Generic;
using Factory;
using UnityEngine;

public class TileProduct : MonoBehaviour, IProduct, IHoverable, IClickable
{
    public static Action<TileProduct> OnMoveCompleted;
    private Camera mainCamera;
    [SerializeField] private TileName tileName;
    [SerializeField] private Sprite sprite;
    [SerializeField] private Outline outline;
    private Rigidbody rgbd;
    private Collider tileCollider;
    public string productName { get => this.tileName.ToString(); }
    public GameObject gameObjectProduct => this.gameObject;

    // TODO: fix this
    public int maxTilesHolderCount = 7; // hardcoded

    private bool isAllowedToMoveOut;
    private bool isMoved = false;
    private void Awake(){
        mainCamera = Camera.main;
        rgbd = GetComponent<Rigidbody>();
        tileCollider = GetComponent<Collider>();
    }

    private void Start() {
        Application.targetFrameRate = 60;
    }

    public void Initialize(TileName name, Sprite sprite){
        this.tileName = name;
        this.sprite = sprite;
        
        this.transform.rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(-90f,90f),UnityEngine.Random.Range(-90f,90f),0f));
    }

    private void FixedUpdate(){
        if (isAllowedToMoveOut) return;
        Vector3 pos = mainCamera.WorldToViewportPoint (transform.position);
		pos.x = Mathf.Clamp(pos.x,0.2f,0.8f);
		pos.y = Mathf.Clamp(pos.y,0.1f,0.9f);
		transform.position = Camera.main.ViewportToWorldPoint(pos);
    }

    public void Hover(){
        outline.enabled = true;
    }

    public void CancelHover(){
        outline.enabled = false;
    }

    public void Click(){
        CancelHover();
        // calculate ui screenpoint to world position
        var pos = GameManager.Instance.tileHolderUI.position;
        pos = mainCamera.ScreenToWorldPoint(pos);
        // invoke move command
        ICommand command = new MoveCommand(this,this.transform.position,pos);
        CommandInvoker.ExecuteCommand(command);
    }

    public void Move(Vector3 position){
        isMoved = !isMoved;
        TogglePhysics(!isMoved);
        this.gameObject.SetActive(true);
        StartCoroutine(MoveToTileUI(position));
    }

    // TODO: make it move to different tile holder UI
    private IEnumerator MoveToTileUI(Vector3 destination){
        float elapsed = 0f;
        while(elapsed < 1.25f){
            var lerpFactor = elapsed / 1.25f;
            lerpFactor = lerpFactor * lerpFactor * (3f - 2f * lerpFactor);
            transform.position = Vector3.Lerp(transform.position,destination,lerpFactor);
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        this.gameObject.SetActive(!isMoved);
        OnMoveCompleted?.Invoke(this);
        yield break;
    }

    private void TogglePhysics(bool toggle){
        rgbd.useGravity = toggle;
        tileCollider.enabled = toggle;
        isAllowedToMoveOut = !toggle;
    }
}

