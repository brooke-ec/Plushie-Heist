using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PricingTableManager : MonoBehaviour
{
    public GameObject rowPrefab;
    public Transform contentTransform;
    public List<ProductData> products;

    private void Start()
    {
        PopulateTable();
    }

    private void PopulateTable()
    {
        //Clear previous rows
        for (int i = 0; i < contentTransform.childCount; i++)
        {
            Destroy(contentTransform.GetChild(i));
        }

        foreach (ProductData product in products)
        {
            GameObject row = Instantiate(rowPrefab, contentTransform);
        }
    }
}
