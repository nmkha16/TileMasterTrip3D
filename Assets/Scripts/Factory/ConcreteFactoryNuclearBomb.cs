using System.Collections;
using System.Collections.Generic;
using Factory;
using UnityEngine;

public class ConcreteFactoryNuclearBomb : ObjectFactory
{
    public GameObject rocketPrefab;
    private  Vector3 startPosition = new Vector3(0f,5f,-5f);
    // unused
    public override IProduct GetProduct(Vector3 position)
    {
        return null;
    }

    public override GameObject GetProductAsGameObject(Vector3 position)
    {
        GameObject rocket = Instantiate(rocketPrefab,startPosition,Quaternion.identity);
        return rocket;
    }
}
