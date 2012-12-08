using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColoritWPF
{
    class ProductByNameFilter
    {
        public string SearchText
        { get; set; }
        public ProductByNameFilter(string text)
        {
            SearchText = text;
        }
        public bool FilterItem(object item)
        {
            Product product = item as Product;
            if (product != null)
                return (product.Name.Contains(SearchText));
            return false;
        }
    }
}
