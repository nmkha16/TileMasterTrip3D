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

    private bool isCooldown;

    private void Awake(){
        mainCamera = Camera.main;
        defaultScale = this.transform.localScale;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rgbd = GetComponent<Rigidbody>();

        clickable = GetComponent<IClickable>();
    }

    private void Start(){
        tileProductMove = GetComponent<TileProductMove>();
        Invoke(nameof(IncreaseDrag),3f);
        GameManager.Instance.OnGameStarted += Dispose;
        GameManager.Instance.OnReturnedToMenu += Dispose;
    }

    private void OnDestroy(){
        GameManager.Instance.OnReturnedToMenu -= Dispose;
        GameManager.Instance.OnGameStarted -= Dispose;
    }

    public void Initialize(TileName name, Sprite sprite){
        this.tileName = name;
        this.spriteRenderer.sprite = sprite;        
        this.transform.rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(-90f,90f),UnityEngine.Random.Range(-90f,90f),0f));
    }

    private void Update(){
        if (isCooldown) return;
        if (IsUpdsideDown()){
            ToppleUp();
        }
    }

    // constraint Tile within camera view
    private void FixedUpdate(){
        Vector3 pos = mainCamera.WorldToViewportPoint (transform.position);
		pos.x = Mathf.Clamp(pos.x,0.1f,0.9f);
		pos.y = Mathf.Clamp(pos.y,0.3f,0.9f);
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
        if (!isPointerOnSelf) return;
        SoundManager.Instance.PlayOneShotSound(SoundId.s_select_tile);
        CancelHover();
        // calculate ui screenpoint to world position
        var pos = TilesManager.Instance.GetTileUISlotPosition(this.tileName);
        pos = mainCamera.ScreenToWorldPoint(pos);

        // invoke move command
        ICommand command = new MoveCommand(this,this.transform.position,pos);
        CommandInvoker.ExecuteCommand(command);
    }

    public void DoMove(Vector3 destination){
        tileProductMove.Move(destination);
    }

    private void IncreaseDrag(){
        this.rgbd.drag = DragValue;
        //this.rgbd.angularDrag = DragValue;
    }

    public void Dispose()
    {
        Destroy(this.gameObject);
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

    private void ToppleUp(){
        isCooldown = true;
        rgbd.drag= 0f;
        rgbd.AddForceAtPosition(Vector3.up * 4f,transform.position * 0.75f,ForceMode.Impulse);
        rgbd.AddTorque(Vector3.up*2f,ForceMode.Impulse);
        Invoke(nameof(IncreaseDrag),3f);
        Invoke(nameof(ResetCooldown),3f);
    }

    private void ResetCooldown(){
        isCooldown = false;
    }
}

