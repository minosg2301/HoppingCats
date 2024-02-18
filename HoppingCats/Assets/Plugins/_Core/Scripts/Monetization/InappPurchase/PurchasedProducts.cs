using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace moonNest
{
    public static class PurchasedProducts
    {
        static List<string> productIds;
        static string key = "vgames-products";

        static void InitProducts()
        {
            if (productIds == null)
            {
                if (PlayerPrefs.HasKey(key))
                {
                    var cached = PlayerPrefs.GetString(key, "");
                    try
                    {
                        productIds = (JsonHelper.Deserialize(cached) as List<object>)
                            .Select(p => p as string)
                            .ToList();
                    }
                    catch (Exception)
                    {
                        productIds = new List<string>();
                    }
                }
                else
                {
                    productIds = new List<string>();
                }
            }
        }

        static void Save()
        {
            PlayerPrefs.SetString(key, JsonHelper.Serialize(productIds));
            PlayerPrefs.Save();
        }

        internal static void AddProduct(string productId)
        {
            InitProducts();
            productIds.Add(productId);
            Save();
        }

        internal static bool ExistProduct(string productId)
        {
            InitProducts();
            return productIds.Contains(productId);

        }
    }
}