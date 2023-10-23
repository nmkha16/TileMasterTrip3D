using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Factory;

public abstract class ObjectFactory : MonoBehaviour
{
    public abstract IProduct GetProduct(Vector3 position);
    public abstract GameObject GetProductAsGameObject(Vector3 position);
    public string GetLog(IProduct product){
        string log = "Factory: created product " + product.productName;
        return log;
    }
}

