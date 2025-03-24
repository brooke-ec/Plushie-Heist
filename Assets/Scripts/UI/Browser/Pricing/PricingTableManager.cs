using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PricingTableManager : MonoBehaviour
{
    public ProductRowUI rowPrefab;
    public Transform contentTransform;
    /// <summary> Logical storage of the product data </summary>
    public List<ProductData> products;
    /// <summary> UI storage of data </summary>
    public List<ProductRowUI> productRows;

    private void Start()
    {
        PopulateTable();
    }


    private void PopulateTable()
    {
        //Clear previous rows
        for (int i = 0; i < contentTransform.childCount; i++)
        {
            Destroy(contentTransform.GetChild(i).gameObject);
        }

        foreach (ProductData product in products)
        {
            ProductRowUI row = Instantiate(rowPrefab, contentTransform);
            row.Set(product);
            productRows.Add(row);
        }
    }

    #region Area for sorting with buttons
    public void SortByProductName()
    {
        products.Sort((p, p2)=>p.productName.CompareTo(p2.productName));
        PopulateTable();
    }
    #endregion
}
