using UnityEngine;

namespace Factory{
    public interface IProduct
    {
        public GameObject gameObjectProduct{get;}
        public string productName{get;}
    }
}
