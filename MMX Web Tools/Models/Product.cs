using System;
using System.Collections.Generic;

namespace MMX_Web_Tools.Models
{
    [Serializable]
    public class Product
    {
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
        public string Name { get; set; }
        public string Sku { get; set; }
        public string Code { get; set; }
        public decimal Cost1 { get; set; }
        public decimal Cost2 { get; set; }
        public decimal Retail { get; set; }
        public decimal Sale { get; set; }
        public int Stock { get; set; }
        public List<Variant> Variants { get; set; } = new List<Variant>();

        public int ProductId => SubCategoryId; // Convenience for grid
        public string DisplayName => Name;
        public decimal WholesalePrice => Cost1; // Alias for grid column naming
    }

    [Serializable]
    public class Variant
    {
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
        public int VariantId { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }
        public decimal Price { get; set; }
    }
}
