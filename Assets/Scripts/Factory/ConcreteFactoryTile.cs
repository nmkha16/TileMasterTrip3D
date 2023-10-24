using UnityEngine;
using Factory;

public class ConcreteFactoryTile : ObjectFactory
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Transform parent;
    public override IProduct GetProduct(Vector3 position)
    {
        Quaternion rot = Quaternion.Euler(new Vector3(0,Random.Range(0,360f),0f));
        GameObject instance = Instantiate(tilePrefab, position, rot);
        instance.transform.parent = parent;
        IProduct newProduct = instance.GetComponent<IProduct>();
        return newProduct;
    }

    public override IProduct GetProduct(PoolManager poolManager, Vector3 position)
    {
        Quaternion rot = Quaternion.Euler(new Vector3(0,Random.Range(0,360f),0f));
        GameObject instance = poolManager.Spawn(tilePrefab,position,rot,tilePrefab.transform.localScale,parent);
        IProduct newProduct = instance.GetComponent<IProduct>();
        return newProduct;
    }

    public override GameObject GetProductAsGameObject(Vector3 position){
        Quaternion rot = Quaternion.Euler(new Vector3(0,Random.Range(0,360f),0f));
        GameObject instance = Instantiate(tilePrefab, position, rot);
        instance.transform.parent = parent;
        return instance;
    }

    public override GameObject GetProductAsGameObject(PoolManager poolManager, Vector3 position)
    {
        Quaternion rot = Quaternion.Euler(new Vector3(0,Random.Range(0,360f),0f));
        GameObject instance = poolManager.Spawn(tilePrefab,position,rot,tilePrefab.transform.localScale,parent);
        instance.transform.parent = parent;
        return instance;
    }
}
