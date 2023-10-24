using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    private Dictionary<GameObject,Pool> currentPools;

    private void Awake(){
        currentPools = new Dictionary<GameObject, Pool>();

        Pool[] childrenPools = GetComponentsInChildren<Pool>();

        for(int i = 0; i < childrenPools.Length; ++i){
            currentPools.Add(childrenPools[i].Prefab,childrenPools[i]);
        }
    }

    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Vector3 scale, Transform parent = null, bool useLocalPosition = false, bool useLocalRotation = false){
        // locate the pool that provides the prefab
        if (!currentPools.ContainsKey(prefab)){
            return SpawnNonPooledObject(prefab,position,rotation,scale,parent,useLocalPosition,useLocalRotation);
        }

        return currentPools[prefab].Spawn(position,rotation,scale,parent,useLocalPosition,useLocalRotation);
    }

    private GameObject SpawnNonPooledObject(GameObject prefab, Vector3 position, Quaternion rotation, Vector3 scale, Transform parent = null, bool useLocalPosition = false, bool useLocalRotation = false){
        Debug.LogWarning("You are spawning a non-pooled prefab \"" + prefab.name + "\"."
                        + " The object will be instantiated normally."
                        + " Check your scene setup.");

        var go = Instantiate(prefab);

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

        return go;
    }

    public void Kill(GameObject go, bool surpassWarning = false){
        if (go == null){
            Debug.LogWarning("<color=red>Warning: Despawning a null object.</color>");
            return;
        }

        if(!go.activeInHierarchy) // somehow the obj is disabled or killed
        {
            return;
        }
        
        foreach(KeyValuePair<GameObject,Pool> pool in currentPools){
            if (pool.Value.IsResponsibleForObject(go)){
                pool.Value.Kill(go);
                return;
            }
        }

        if(!surpassWarning){
            Debug.LogWarning("You are killing a non-pooled prefab: " + go.name 
                            + " The object will be destroyed normally."
                            + " Please check your scene setup.");
            Destroy(go);
        }
    }
}