using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class CameraActionHandler : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera uiCamera;
    [SerializeField] private Camera particleCamera;
    [SerializeField] private float maxOrthosize = 3f;
    public float zoomDuration = 5f;
    private float baseOrthoSize;

    private Coroutine zoomCameraRoutine = null;

    private void Awake(){
        baseOrthoSize = mainCamera.orthographicSize;
    }

    // false = zoom out
    // true = zoom in
    public void ZoomCamera(bool isZoomOut){
        if (zoomCameraRoutine != null){
            StopCoroutine(zoomCameraRoutine);
            zoomCameraRoutine = null;
        }
        zoomCameraRoutine = StartCoroutine(ZoomCameraRoutine(isZoomOut ? maxOrthosize : baseOrthoSize));
    }

    private IEnumerator ZoomCameraRoutine(float to){
        float elapsed =  0f;

        while(elapsed < zoomDuration){
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize,to,elapsed/zoomDuration);
            uiCamera.orthographicSize = Mathf.Lerp(particleCamera.orthographicSize,to,elapsed/zoomDuration);
            particleCamera.orthographicSize = Mathf.Lerp(particleCamera.orthographicSize,to,elapsed/zoomDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.orthographicSize = to;
        uiCamera.orthographicSize = to;
        particleCamera.orthographicSize = to;
        zoomCameraRoutine = null;
    }
}
