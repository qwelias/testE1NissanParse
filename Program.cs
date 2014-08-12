using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

using HtmlAgilityPack;

namespace e1NissanParse
{
    static class Parse
    {
        private static WebClient client;
        private static HtmlDocument doc;
        private static StreamWriter writer;
        private static int pages = 0;
        static void Main(string[] args)
        {
            client = new WebClient();
            try
            {
                writer = new StreamWriter("out.txt", false, Encoding.UTF8);
                try
                {
                    doc = new HtmlDocument();
                    client.Encoding = Encoding.UTF8;
                    writer.AutoFlush = true;
                    doc.OptionDefaultStreamEncoding = Encoding.UTF8;
                    readPage("http://auto.e1.ru/car/nissan/?region=213");
                    for (int j = 2; j <= pages; j++)
                    {
                        Console.WriteLine(j);
                        readPage("http://auto.e1.ru/car/nissan/?region=213&pure=0&page=" + j);
                    }
                }
                finally
                {
                    client.Dispose();
                    writer.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.ReadLine();
        }

        private static void readPage(string link)
        {
            doc.LoadHtml(client.DownloadString(link));
            if (pages == 0)
            {
                HtmlNodeCollection nums = doc.DocumentNode.SelectNodes("//ul[@class='au-pagination__list']//li");
                pages = Convert.ToInt32(nums[(nums.Count / 2) - 2].InnerText);
                if (pages == 0) throw new IOException("no pages detected");
                Console.WriteLine("total " + pages);
                Console.WriteLine(1);
            }
            HtmlNodeCollection items = doc.DocumentNode.SelectNodes("//div[@class='_offers_list']//table//tbody//tr");
            string price; // /td[1]/nobr[1]/a[1]
            string year; // /td[2]
            string model; // /td[4]
            string date; // td[5]
            string city; // /td[12]
            string p = "            ";
            for (int i = 3; i < items.Count - 2; i++)
            {
                HtmlNode item = items[i];
                price = item.SelectSingleNode(item.XPath + "//td[1]//nobr[1]//a[1]").InnerText;
                price = price.Trim();
                price = price.Replace("&nbsp;", " ");
                year = item.SelectSingleNode(item.XPath + "//td[2]").InnerText;
                year = year.Trim();
                model = item.SelectSingleNode(item.XPath + "//td[4]").InnerText;
                model = model.Trim();
                model = model.Replace("&#039;", "`");
                date = item.SelectSingleNode(item.XPath + "//td[5]").InnerText;
                date = date.Trim();
                city = item.SelectSingleNode(item.XPath + "//td[12]").InnerText;
                city = city.Trim();
                writer.WriteLine(price + p + year + p + model + p + date + p + city);
            }
        }
    }
}
