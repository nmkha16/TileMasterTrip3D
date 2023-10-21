
using UnityEngine;

// nmkha: this old method is not very accurate, raycast usually ignore front object most of the time
public class ObjectSelection : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private LayerMask objectLayermask;
    private Camera mainCamera;
    [SerializeField] private bool isSelecting = false;

    private IClickable currentClickable;
    private IHoverable currentHoverable;

    private RaycastHit[] hits;

    private void Awake(){
        mainCamera = Camera.main;
    }

    private void Start(){
        //inputReader.OnHoverPerformed += EnableSelecting;
        inputReader.OnSelectPerformed += DisableSelecting;
    }

    private void FixedUpdate(){
        if (isSelecting){
            PerformRaycast();
        }
    }

    private void OnDestroy() {
        //inputReader.OnHoverPerformed -= EnableSelecting;
        inputReader.OnSelectPerformed -= DisableSelecting;
    }

    private void PerformRaycast(){
        var ray = mainCamera.ScreenPointToRay(inputReader.touchPosition);
        //int hitCount = Physics.RaycastNonAlloc(ray,hits,5f,objectLayermask);
        hits = Physics.RaycastAll(ray,5f,objectLayermask);
        int hitCount = hits.Length;
        if (hitCount > 0){
            var hit = GetClosestHit(hits,ray.origin);
            var newHoverable = hit.transform.GetComponent<IHoverable>();
            if (currentHoverable != newHoverable){
                currentHoverable?.CancelHover();
                currentHoverable = newHoverable;
                currentHoverable?.Hover();
            }
            currentClickable = hit.transform.GetComponent<IClickable>();
        }
        else{
            currentHoverable?.CancelHover();
            currentHoverable = null;
            currentClickable = null;
        }
    }

    private RaycastHit GetClosestHit(RaycastHit[] raycastHits, Vector3 startPos){
        var hit = raycastHits[0];
        float distance = (startPos - hit.transform.position).sqrMagnitude;

        for(int i = 1; i < raycastHits.Length; ++i){
            var distanceSoFar = (startPos - hit.transform.position).sqrMagnitude;
            hit = raycastHits[i];
        }

        return hit;

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
