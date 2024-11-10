
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;
using Unity.VisualScripting;


//pruduct paramenters defined for the ProductData scripltable object
[System.Serializable]
public class Product
{
    public int productId;
    public string name;
    public string description;
    public float price;
    public Mesh mesh;
}

[System.Serializable]
public class ProductResponse
{
    public List<Product> products;
}


//This script fetches the data from the server and my product prefab 1-3 times and overide the ProductData scriptable object  parameter values depending on the data fetched




public class ProductFetcher : MonoBehaviour
{
    public string apiUrl = "https://homework.mocart.io/api/products";
    public GameObject productPrefab;
    public Transform[] holders; 
    public ProductUIManager uiManager;

    public static event UnityAction<Product> OnProductSelected;

    public ProductData productData;  

    private void Start()
    {
        StartCoroutine(FetchProducts());
    }

    IEnumerator FetchProducts()
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ProductResponse response = JsonUtility.FromJson<ProductResponse>(request.downloadHandler.text);
            UpdateProductData(response.products); 
            DisplayProducts(response.products);
        }
        
    }


    void UpdateProductData(List<Product> products)
    {
        foreach (Product updatedProduct in products)
        {
            bool found = false;

           
            int productIdFromName = ExtractProductIdFromName(updatedProduct.name);

            for (int i = 0; i < productData.products.Length; i++)
            {
                if (productData.products[i].productId == productIdFromName)
                {
                    productData.products[i].name = updatedProduct.name;
                    productData.products[i].description = updatedProduct.description;
                    productData.products[i].price = updatedProduct.price;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                for (int i = 0; i < productData.products.Length; i++)
                {
                    if (productData.products[i].productId == 0) 
                    {
                        productData.products[i].name = updatedProduct.name;
                        productData.products[i].description = updatedProduct.description;
                        productData.products[i].price = updatedProduct.price;
                        productData.products[i].productId = productIdFromName; 
                        break;
                    }
                }
            }
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(productData);
        UnityEditor.AssetDatabase.SaveAssets();
#endif
    }

   
    int ExtractProductIdFromName(string productName)
    {
        
        string numberString = new string(productName.Where(char.IsDigit).ToArray());
        return int.TryParse(numberString, out int productId) ? productId : 0;
    }




    void DisplayProducts(List<Product> products)
    {
       
        foreach (Transform child in holders)
            if (child.childCount > 0)
                Destroy(child.GetChild(0).gameObject);

       
        int[] positions;
        switch (products.Count)
        {
            case 1:
                positions = new int[] { 2 }; 
                break;
            case 2:
                positions = new int[] { 1, 3 }; 
                break;
            case 3:
                positions = new int[] { 0, 2, 4 }; 
                break;
            default:
                positions = new int[] { 0, 1, 2, 3, 4 }; 
                break;
        }

       
        for (int i = 0; i < products.Count && i < positions.Length; i++)
        {
            int holderIndex = positions[i];
            GameObject productModel = Instantiate(productPrefab, holders[holderIndex]);
            productModel.name = products[i].name;

            
            ProductButton productButton = productModel.AddComponent<ProductButton>();

           
            Product currentProduct = products[i];
            productButton.SetProduct(currentProduct);

           
            productButton.OnClick += () => OnProductSelected?.Invoke(currentProduct);
        }
    }
}