using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int initialSize = 5;

    private Stack<GameObject> pooledInstances;
    private List<GameObject> aliveInstances;

    public GameObject Prefab => prefab;

    private void Awake(){
        pooledInstances = new Stack<GameObject>();

        for(int i = 0; i < initialSize; i++){
            var instance = Instantiate(prefab);
            instance.transform.SetParent(this.transform);

            instance.transform.localPosition = Vector3.zero;
            instance.transform.localEulerAngles = Vector3.zero;
            instance.transform.localScale = Vector3.one;

            instance.SetActive(false);

            pooledInstances.Push(instance);
        }

        aliveInstances = new List<GameObject>();
    }

    public GameObject Spawn(Vector3 position, Quaternion rotation, Vector3 scale, Transform parent = null, bool useLocalPosition = false, bool useLocalRotation = false){
        if (pooledInstances.Count == 0){ // all objects in pool have been used
            var newInstance = Instantiate(prefab);

            newInstance.transform.SetParent(parent);
            InternalSetProperties(newInstance,position,rotation,scale,parent,useLocalPosition,useLocalRotation);

            aliveInstances.Add(newInstance);
            return newInstance;
        }

        // stack has object in pool
        var instance = pooledInstances.Pop();

        instance.transform.SetParent(parent);
        InternalSetProperties(instance,position,rotation,scale,parent,useLocalPosition,useLocalRotation);

        instance.SetActive(true);
        aliveInstances.Add(instance);
        return instance;
    }

    /// <summary>
    /// Deactivate gameobject, remove gameobject from aliveInstance list then add back to the pool 
    /// </summary>
    /// <param name="go"></param>
    public void Kill(GameObject go){
        int idx = aliveInstances.FindIndex(o => go == o);
        // no gameobject is found in the alive list, destroy object normally
        if (idx == -1){
            Destroy(go);
            return;
        }

        go.SetActive(false);

        go.transform.SetParent(this.transform);
        go.transform.localPosition = Vector3.zero;
        go.transform.localEulerAngles = Vector3.zero;
        go.transform.localScale = Vector3.one;

        aliveInstances.RemoveAt(idx);
        pooledInstances.Push(go);
    }

    public bool IsResponsibleForObject(GameObject go){
        int idx = aliveInstances.FindIndex ( o => go == o);

        if (idx == -1){
            return false;
        }

        return true;
    }

    private void InternalSetProperties(GameObject go,Vector3 position, Quaternion rotation, Vector3 scale, Transform parent = null, bool useLocalPosition = false, bool useLocalRotation = false){
        go.transform.SetParent(parent);
        if(useLocalRotation){
            go.transform.localRotation = rotation;
        }
        else{
            go.transform.rotation = rotation;
        }
        if (useLocalPosition){
            go.transform.localPosition = position;
        }
        else{
            go.transform.position = position;
        }
        go.transform.localScale = scale;
    }
}