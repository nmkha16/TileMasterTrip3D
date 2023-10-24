using System;
using Factory;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileProduct : MonoBehaviour, IProduct, IHoverable, IClickable, IDisposable, IPointerEnterHandler, IPointerExitHandler
{
    public static event Action<IClickable>  OnTileClicked;
    [SerializeField] private Outline outline;
    TileProductMove tileProductMove;
    private Camera mainCamera;
    public TileName tileName {get; private set;}
    public SpriteRenderer spriteRenderer {get; private set;}

    public string productName { get => this.tileName.ToString(); }
    public GameObject gameObjectProduct => this.gameObject;
    private Rigidbody rgbd;
    private Vector3 defaultScale;
    public float DragValue = 20;
    private bool isPointerOnSelf;
    private IClickable clickable;

    public bool isDestroyed;

    private void Awake(){
        mainCamera = Camera.main;
        defaultScale = this.transform.localScale;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rgbd = GetComponent<Rigidbody>();

        clickable = GetComponent<IClickable>();
    }

    private void Start(){
        tileProductMove = GetComponent<TileProductMove>();
        
        GameManager.Instance.OnReturnedToMenu += Dispose;
        GameManager.Instance.OnGameStarted += Dispose;
        // GameManager.Instance.OnGameEnded += Dispose;
        GameManager.Instance.OnNuked += Dispose;
        GameManager.Instance.OnGamePaused += DisableMotion;
        GameManager.Instance.OnGameUnpaused += EnableMotion;
    }

    private void OnEnable() {
        Invoke(nameof(IncreaseDrag),6f);
    }

    private void OnDisable() {
        rgbd.drag = 1f;
        rgbd.angularDrag = 0.01f;
    }

    private void OnDestroy(){
        GameManager.Instance.OnReturnedToMenu -= Dispose;
        GameManager.Instance.OnGameStarted -= Dispose;
        // GameManager.Instance.OnGameEnded -= Dispose;
        GameManager.Instance.OnNuked -= Dispose;
        GameManager.Instance.OnGamePaused -= DisableMotion;
        GameManager.Instance.OnGameUnpaused -= EnableMotion;
    }

    public void Initialize(TileName name, Sprite sprite){
        this.tileName = name;
        this.spriteRenderer.sprite = sprite;        
        isDestroyed = false;
    }

    private void EnableMotion(){
        SetMotion(true);
    }

    private void DisableMotion(){
        SetMotion(false);
    }

    private void SetMotion(bool toggle){
        rgbd.isKinematic = !toggle;
        rgbd.useGravity = toggle;
        rgbd.freezeRotation = !toggle;
        this.enabled = toggle;
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
        if (!isPointerOnSelf) return;
        if (TilesManager.Instance.isAvailableToMove){
            SoundManager.Instance.PlayOneShotSound(SoundId.s_select_tile);
            CancelHover();
            var pos = TilesManager.Instance.GetTileUISlotPosition(this.tileName);
            // invoke move command
            ICommand command = new MoveCommand(this,this.transform.position,pos);
            CommandInvoker.ExecuteCommand(command);
        }
    }

    public void DoMove(Vector3 destination){
        tileProductMove.Move(destination);
    }

    private void IncreaseDrag(){
        // this.rgbd.drag = DragValue;
        this.rgbd.angularDrag = DragValue;
    }

    public void Dispose()
    {
        //Destroy(this.gameObject);
        isDestroyed = true;
        SetMotion(true);
        GameManager.Instance.poolManager.Kill(gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundManager.Instance.PlayOneShotSound(SoundId.s_hover_tile);
        Hover();
        isPointerOnSelf = true;
        OnTileClicked?.Invoke(clickable);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CancelHover();
        isPointerOnSelf = false;
    }

    private bool IsUpdsideDown(){
        return Vector3.Dot(transform.up,Vector3.down) > 0;
    }
}

