using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColoritWPF
{
    class ClientByNameFilter
    {
        public string SearchText
        { get; set; }
        public ClientByNameFilter(string text)
        {
            SearchText = text;
        }
        public bool FilterItem(object item)
        {
            Client client = item as Client;
            if (client != null)
                return (client.Name.Contains(SearchText));
            return false;
        }
    }
}
