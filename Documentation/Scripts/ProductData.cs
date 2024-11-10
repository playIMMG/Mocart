using UnityEngine;

[CreateAssetMenu(fileName = "ProductData", menuName = "Product/ProductData", order = 1)]
public class ProductData : ScriptableObject
{
    public Product[] products = new Product[10]; 
}
