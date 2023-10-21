using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineScale : MonoBehaviour
{
    [SerializeField] private float amplitude;
    [SerializeField] private float speed = 1;
    private void Update(){
        var sineValue = Mathf.Sin(Time.time*speed) * amplitude + 1;
        Vector3 scale = new Vector3(sineValue,sineValue,sineValue);
        transform.localScale = scale;
    }
}
