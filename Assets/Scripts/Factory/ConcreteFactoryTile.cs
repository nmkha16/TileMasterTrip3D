using UnityEngine;
using Factory;

public class ConcreteFactoryTile : ObjectFactory
{
    [SerializeField] private TileProduct tilePrefab;
    [SerializeField] private Transform parent;
    public override IProduct GetProduct(Vector3 position)
    {
        // TODO: replace with object pool
        Quaternion rot = Quaternion.Euler(new Vector3(0f,Random.Range(0,360f),0f));
        GameObject instance = Instantiate(tilePrefab.gameObject, position, rot);
        instance.transform.parent = parent;
        TileProduct newProduct = instance.GetComponent<TileProduct>();

        return newProduct;
    }
}
