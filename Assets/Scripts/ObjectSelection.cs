using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSelection : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private LayerMask objectLayermask;
    private Camera mainCamera;
    [SerializeField] private bool isSelecting = false;

    private IClickable currentClickable;
    private IHoverable currentHoverable;

    private RaycastHit[] hits = new RaycastHit[1]; // only need to detect one object at a time

    private void Awake(){
        mainCamera = Camera.main;
    }

    private void Start(){
        inputReader.OnHoverPerformed += EnableSelecting;
        inputReader.OnSelectPerformed += DisableSelecting;
    }

    private void Update(){
        if (isSelecting){
            PerformRaycast();
        }
    }

    private void OnDestroy() {
        inputReader.OnHoverPerformed -= EnableSelecting;
        inputReader.OnSelectPerformed -= DisableSelecting;
    }

    private void PerformRaycast(){
        var ray = mainCamera.ScreenPointToRay(inputReader.touchPosition);
        int hitCount = Physics.RaycastNonAlloc(ray,hits,10f,objectLayermask);
        if (hitCount > 0){
            var newHoverable = hits[0].transform.GetComponent<IHoverable>();
            if (currentHoverable != newHoverable){
                currentHoverable?.CancelHover();
                currentHoverable = newHoverable;
                currentHoverable?.Hover();
            }
            currentClickable = hits[0].transform.GetComponent<IClickable>();
        }
        else{
            currentHoverable?.CancelHover();
            currentHoverable = null;
            currentClickable = null;
        }
    }

    private void EnableSelecting() => isSelecting = true;
    private void DisableSelecting() {
        isSelecting = false;
        currentHoverable = null;
        PerformClick();
    }
    private void PerformClick(){
        currentClickable?.Click();
        currentClickable = null;
    }
}
