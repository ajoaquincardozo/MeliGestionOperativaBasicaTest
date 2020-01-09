using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MeliGestionOperativaBasicaTest.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MeliGestionOperativaBasicaTest
{
    public class Program
    {
        public static string SITE_ID = "MLA";
        public static string SELLER_ID = "179571326";
        public static string urlGetInfoSeller = $"https://api.mercadolibre.com/sites/{SITE_ID}/search?seller_id=";
        public static string urlCategoryName = "https://api.mercadolibre.com/categories/";
        public static HttpClient client = new HttpClient();

        public static async Task<List<SellerItem>> GetListOfItemsFromSeller(long sellerId)
        {
            try
            {
                Uri urlGetInfoSellerComplete = new Uri(urlGetInfoSeller + sellerId);
                var response = await client.GetAsync(urlGetInfoSellerComplete);

                string jsonString = await response.Content.ReadAsStringAsync();
                JObject json = JObject.Parse(jsonString);
                return json["results"].Select(itemStr => JsonConvert.DeserializeObject<SellerItem>(itemStr.ToString())).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public static string ToRowLogItem(SellerItem item) 
            => $"item: {item.Id}, title: {item.Title}, category_id: {item.CategoryId}, category_name: {item.CategoryName}";

        public static void LogItemsFrom(long sellerId, List<SellerItem> items)
        {
            var nameLogFile = $"./LogItems_{sellerId}.txt";
            if (File.Exists(nameLogFile))
                File.Delete(nameLogFile);

            using (var fileStream = File.Create(nameLogFile))
            {
                var allData = String.Join(Environment.NewLine, items.Select(ToRowLogItem));
                byte[] info = new UTF8Encoding(true).GetBytes(allData);
                fileStream.Write(info, 0, info.Length);
            }
        }

        public static async Task GetNameOfCategory(SellerItem sellerItem)
        {
            var uriCategoryNameComplete = new Uri(urlCategoryName + sellerItem.CategoryId);
            var response = await client.GetAsync(uriCategoryNameComplete);

            string jsonString = await response.Content.ReadAsStringAsync();
            JObject json = JObject.Parse(jsonString);
            sellerItem.CategoryName = json["name"].ToString();
        }

        public static async Task LogItemsFromSeller(params long[] sellersId)
        {
            List<Task> tasks = new List<Task>();
            foreach (var sellerId in sellersId)
            {
                var sellerItems = await GetListOfItemsFromSeller(sellerId);

                sellerItems.ForEach(item => tasks.Add(GetNameOfCategory(item)));
                Task task = Task.WhenAll(tasks.ToArray());
                task.Wait();

                LogItemsFrom(sellerId, sellerItems);
            }
        }

        static void Main(string[] args)
        {
            LogItemsFromSeller(long.Parse(SELLER_ID)).Wait();
        }
    }
}
