using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeliGestionOperativaBasicaTest.Models
{
    public class Seller
    {
        [JsonProperty("site_id")]
        public string SiteId { get; set; }

        [JsonProperty("seller")]
        public string SellerInfo { get; set; }

        [JsonProperty("results")]
        public string Items { get; set; }
    }
}
