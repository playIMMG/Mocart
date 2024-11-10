using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;


//This scrip is responsible for all the methods used by te UI buttons and functionality.
//and lisening to events invoked by products clicked to change UI

public class ProductUIManager : MonoBehaviour
{
    
    public TextMeshProUGUI productNameText;
    public TextMeshProUGUI productDescriptionText;
    public TextMeshProUGUI productPriceText;

    
    [SerializeField] private TMP_InputField productNameInputField;
    [SerializeField] private TMP_InputField descriptionInputField;
    [SerializeField] private TMP_InputField priceInputField;

   
    [SerializeField] private Button productNameCancelButton;
    [SerializeField] private Button productNameApplyButton;

    
    [SerializeField] private Button descriptionCancelButton;
    [SerializeField] private Button descriptionApplyButton;

   
    [SerializeField] private Button priceCancelButton;
    [SerializeField] private Button priceApplyButton;

   
    [SerializeField] private ProductData productData;

    public GameObject confirmationPrefab; 
    public Transform parentTransform;      


    private Product currentProduct;  

    private void OnEnable()
    {
        ProductFetcher.OnProductSelected += UpdateUI;
    }

    private void OnDisable()
    {
        ProductFetcher.OnProductSelected -= UpdateUI;
    }

    private void UpdateUI(Product product)
    {
        int productIdFromName = ExtractProductIdFromName(product.name);
        currentProduct = GetProductFromData(productIdFromName);

        if (currentProduct != null)
        {

           
            if (productNameText != null)
                productNameText.text = currentProduct.name;
            if (productDescriptionText != null)
                productDescriptionText.text = currentProduct.description;
            if (productPriceText != null)
                productPriceText.text = "$" + currentProduct.price.ToString("F2");

           
            if (productNameInputField.gameObject.activeSelf || descriptionInputField.gameObject.activeSelf || priceInputField.gameObject.activeSelf)
            {
               
                ShowConfirmationMessage(false);
            }

           
            ToggleEditingFields(false);

            
            productNameText.gameObject.SetActive(true);
            productDescriptionText.gameObject.SetActive(true);
            productPriceText.gameObject.SetActive(true);
        }
      
    }

   
    int ExtractProductIdFromName(string productName)
    {
        string numberString = new string(productName.Where(char.IsDigit).ToArray());
        return int.TryParse(numberString, out int productId) ? productId : 0;
    }


    
    Product GetProductFromData(int productId)
    {
       
        foreach (Product p in productData.products)
        {
            if (p.productId == productId)
            {
                return p;
            }
        }
        return null;
    }


    public void OnPressProductNameText()
    {
        ToggleInputFields(productNameText, productNameInputField, productNameCancelButton, productNameApplyButton, currentProduct.name);
    }

    public void OnPressDescriptionText()
    {
        ToggleInputFields(productDescriptionText, descriptionInputField, descriptionCancelButton, descriptionApplyButton, currentProduct.description);
    }

    public void OnPressPriceText()
    {
        ToggleInputFields(productPriceText, priceInputField, priceCancelButton, priceApplyButton, currentProduct.price.ToString("F2"));
    }

    private void ToggleInputFields(TextMeshProUGUI textField, TMP_InputField inputField, Button cancelButton, Button applyButton, string initialValue)
    {
        textField.gameObject.SetActive(false);
        inputField.gameObject.SetActive(true);
        inputField.text = initialValue;

        cancelButton.gameObject.SetActive(true);
        applyButton.gameObject.SetActive(true);
    }

    private void ToggleEditingFields(bool isActive)
    {
        productNameInputField.gameObject.SetActive(isActive);
        productNameCancelButton.gameObject.SetActive(isActive);
        productNameApplyButton.gameObject.SetActive(isActive);

        descriptionInputField.gameObject.SetActive(isActive);
        descriptionCancelButton.gameObject.SetActive(isActive);
        descriptionApplyButton.gameObject.SetActive(isActive);

        priceInputField.gameObject.SetActive(isActive);
        priceCancelButton.gameObject.SetActive(isActive);
        priceApplyButton.gameObject.SetActive(isActive);
    }

    public void OnPressProductNameApply()
    {
        currentProduct.name = productNameInputField.text;
        UpdateProductInData(currentProduct);
        productNameText.text = currentProduct.name;
        ToggleEditingFields(false);
        productNameText.gameObject.SetActive(true);

        ShowConfirmationMessage(true);
    }

   
    public void OnPressDescriptionApply()
    {
        currentProduct.description = descriptionInputField.text;
        UpdateProductInData(currentProduct);
        productDescriptionText.text = currentProduct.description;
        ToggleEditingFields(false);
        productDescriptionText.gameObject.SetActive(true);

       
        ShowConfirmationMessage(true);
    }

 
    public void OnPressPriceApply()
    {
        float newPrice;
        if (float.TryParse(priceInputField.text, out newPrice))
        {
            currentProduct.price = newPrice;
            UpdateProductInData(currentProduct);
        }
        productPriceText.text = "$" + currentProduct.price.ToString("F2");
        ToggleEditingFields(false);
        productPriceText.gameObject.SetActive(true);

        ShowConfirmationMessage(true);

    }

    public void OnPressProductNameCancel()
    {
        productNameInputField.gameObject.SetActive(false);
        productNameText.gameObject.SetActive(true);
        productNameCancelButton.gameObject.SetActive(false);
        productNameApplyButton.gameObject.SetActive(false);

        ShowConfirmationMessage(false);
    }

    public void OnPressDescriptionCancel()
    {
        descriptionInputField.gameObject.SetActive(false);
        productDescriptionText.gameObject.SetActive(true);
        descriptionCancelButton.gameObject.SetActive(false);
        descriptionApplyButton.gameObject.SetActive(false);

        ShowConfirmationMessage(false);
    }

    public void OnPressPriceCancel()
    {
        priceInputField.gameObject.SetActive(false);
        productPriceText.gameObject.SetActive(true);
        priceCancelButton.gameObject.SetActive(false);
        priceApplyButton.gameObject.SetActive(false);

        ShowConfirmationMessage(false);
    }

    private void UpdateProductInData(Product updatedProduct)
    {
        for (int i = 0; i < productData.products.Length; i++)
        {
            if (productData.products[i].name == updatedProduct.name)
            {
                productData.products[i] = updatedProduct;
                return;
            }
        }
    }

    private void ShowConfirmationMessage(bool isApplied)
    {
        GameObject confirmationMessage = Instantiate(confirmationPrefab, parentTransform);

        TMP_Text textComponent = confirmationMessage.GetComponentInChildren<TMP_Text>();

        Image imageComponent = confirmationMessage.GetComponent<Image>();

        if (isApplied)
        {
            textComponent.text = "Changes Saved";  
                                                   
            imageComponent.color = new Color32(123, 219, 136, 255);
        }
        else
        {
            textComponent.text = "Changes Not Saved";  
                                                      
            imageComponent.color = new Color32(234, 147, 163, 255);
        }
    }




}
