using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileProductMove : MonoBehaviour
{
    public static event Action OnMoveCompleted;
    private bool isMoved;
    private Rigidbody rgbd;
    private Collider tileCollider;

    private void Awake() {
        rgbd = GetComponent<Rigidbody>();
        tileCollider = GetComponent<Collider>();
    }

    public void Move(Vector3 position){
        isMoved = !isMoved;
        TogglePhysics(!isMoved);
        this.gameObject.SetActive(true);
        StartCoroutine(MoveToTileUIRoutine(position));
    }

    private IEnumerator MoveToTileUIRoutine(Vector3 destination){
        float elapsed = 0f;
        while(elapsed < 0.5f){
            var lerpFactor = elapsed / 0.5f;
            transform.position = Vector3.Lerp(transform.position,destination,lerpFactor);
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        OnMoveCompleted?.Invoke();
        yield break;
    }

    private void TogglePhysics(bool toggle){
        rgbd.useGravity = toggle;
        tileCollider.enabled = toggle;
    }
}
