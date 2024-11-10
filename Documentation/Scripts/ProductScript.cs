using UnityEngine;
using UnityEngine.Events;

//the script invokes the events when a product is clicked
// also determins product mesh by product#  tween animate it
// and do tween animate selected products 



public class ProductScript : MonoBehaviour
{
    public ProductData productData; 
    public MeshFilter meshFilter;   

    public static event UnityAction<ProductData> OnProductClicked;
    private static ProductScript currentSpinningProduct; 

    private bool isSpinning = false; 
    private Vector3 targetScale = new Vector3(20f, 20f, 20f); 
    private Vector3 spinningScale = new Vector3(25f, 25f, 25f);
    private float scaleSpeed = 2f; 
    public float elevationAmount = 1f; 
    public float elevationSpeed = 2f; 
    private Vector3 originalPosition; 

    private void Start()
    {
      
        string productName = gameObject.name; 
      

        if (productName.StartsWith("Product "))
        {
            string productIdString = productName.Substring("Product ".Length);
            if (int.TryParse(productIdString, out int productId))
            {
               

                Product product = FindProductById(productId);

                if (product != null)
                {
                   
                    if (meshFilter != null)
                    {
                       
                        meshFilter.mesh = product.mesh;
                    }
                  
                }
               
            }
           
        }
       

       
        originalPosition = transform.position;
        transform.localScale = targetScale;
    }

    private Product FindProductById(int productId)
    {
     
        foreach (Product product in productData.products)
        {
            if (product.productId == productId)
            {
                return product;
            }
        }
        return null;
    }

    private void OnMouseDown()
    {
        HandleClick();
    }

    private void HandleClick()
    {
       
        if (currentSpinningProduct != null && currentSpinningProduct != this)
        {
            currentSpinningProduct.StopSpinning();
        }

        
        if (!isSpinning)
        {
            StartSpinning();
        }

       
        OnProductClicked?.Invoke(productData);
    }

    private void StartSpinning()
    {
        currentSpinningProduct = this; 
        isSpinning = true;
    }

    private void StopSpinning()
    {
        isSpinning = false;
        targetScale = new Vector3(20f, 20f, 20f); 
    }

    private void Update()
    {
        if (isSpinning)
        {
           
            transform.Rotate(0, 0, 100 * Time.deltaTime); 

           
            transform.localScale = Vector3.Lerp(transform.localScale, spinningScale, scaleSpeed * Time.deltaTime);

            
            transform.position = Vector3.Lerp(transform.position, new Vector3(originalPosition.x, originalPosition.y + elevationAmount, originalPosition.z), elevationSpeed * Time.deltaTime);
        }
        else
        {
            
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, scaleSpeed * Time.deltaTime);

            transform.position = Vector3.Lerp(transform.position, originalPosition, elevationSpeed * Time.deltaTime);
        }
    }
}
