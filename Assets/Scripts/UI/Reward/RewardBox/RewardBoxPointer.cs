using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RewardBoxPointer : MonoBehaviour, IPointerClickHandler
{
    public event Action OnClicked;

    private void Awake() {
        this.enabled = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClicked?.Invoke();
    }
}
