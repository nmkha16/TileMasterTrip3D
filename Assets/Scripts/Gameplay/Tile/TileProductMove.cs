using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileProductMove : MonoBehaviour
{
    private TileProduct tileProduct;
    public static event Action<TileProduct> OnTileSelected;
    public static event Action OnMoveCompleted;
    private bool isMoved;
    private Rigidbody rgbd;
    // private Collider tileCollider;
    private Vector3 baseScale;

    private void Awake() {
        rgbd = GetComponent<Rigidbody>();
        // tileCollider = GetComponent<Collider>();
    }

    private void Start(){
        tileProduct = GetComponent<TileProduct>();
        baseScale = this.transform.localScale;
    }

    private void OnEnable() {
        isMoved = false;
    }

    public void Move(Vector3 position){
        isMoved = !isMoved;
        TogglePhysics(!isMoved);
        // this.gameObject.SetActive(true);
        if (isMoved){
            OnTileSelected?.Invoke(tileProduct);
        }
        StartCoroutine(MoveToTileUIRoutine(position));
    }

    private IEnumerator MoveToTileUIRoutine(Vector3 destination){
        float elapsed = 0f;
        Vector3 scaleTo = isMoved ? Vector3.zero : baseScale;
        while(elapsed < 0.5f){
            var lerpFactor = elapsed / 0.5f;
            transform.position = Vector3.Lerp(transform.position,destination,lerpFactor);
            this.transform.localScale = Vector3.Lerp(this.transform.localScale,scaleTo,lerpFactor);
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        this.transform.localScale = scaleTo;
        // transform.position = destination;
        // this.gameObject.SetActive(!isMoved);


        OnMoveCompleted?.Invoke();
        yield break;
    }

    private void TogglePhysics(bool toggle){
        rgbd.useGravity = toggle;
        // tileCollider.enabled = toggle;
        rgbd.isKinematic = !toggle;
    }
}
