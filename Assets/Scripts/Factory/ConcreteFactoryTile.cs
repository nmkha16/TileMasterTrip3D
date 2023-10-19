using UnityEngine;
using Factory;

public class ConcreteFactoryTile : ObjectFactory
{
    [SerializeField] private TileProduct tilePrefab;

    public override IProduct GetProduct(Vector3 position)
    {
        // TODO: replace with object pool
        GameObject instance = Instantiate(tilePrefab.gameObject, position,Quaternion.identity);
        TileProduct newProduct = instance.GetComponent<TileProduct>();

        return newProduct;
    }
}
