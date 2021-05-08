using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Verosofta
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            using (var client = new WebClient())
            {
                string url = "https://hosting.dexmen.com/b8cGjmHF9.html";
                string html = client.DownloadString(url);

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);
                var jsScript = doc.DocumentNode.SelectNodes("//body/script")?.Where(x => x.Attributes.Contains("type")).ToList()[1].InnerText;
                var scriptLines = jsScript.Split("\n");

                var json = scriptLines.FirstOrDefault(x => x.Contains("var data=")).Substring(9);

                List<SalaryObject> salaryObjects = new List<SalaryObject>();
                salaryObjects = JsonConvert.DeserializeObject<List<SalaryObject>>(json);
                salaryObjects.RemoveAt(0);

                Console.WriteLine("Verotaulukkojuttu\n");

                foreach (var salary in salaryObjects)
                {
                    salary.calculateNetIncomePerMonth();
                    var monthly = salary.GrossSalaryPerMonth;
                    var tax = salary.TaxPercent;
                    var netMonthly = salary.NetIncomePerMonth;
                    var yearly = salary.GrossSalaryPerYear;
                    var netYearly = salary.NetIncomePerYear;

                    Console.WriteLine($"kuukausipalkka (brutto): {monthly}\u20AC/kk, verojen ({tax}) jälkeen {netMonthly}\u20AC/kk. Vuositulot (brutto): {yearly}\u20AC, verojen jälkeen {netYearly}\u20AC ");
                }
                Console.WriteLine("Press enter to exit");
                var input = Console.ReadLine();

            }
        }
    }
}

public class SalaryObject
{
    [JsonProperty("0")]
    public string GrossSalaryPerYear { get; set; }

    [JsonProperty("1")]
    public string GrossSalaryPerMonth { get; set; }

    [JsonProperty("2")]
    public string TaxPercent { get; set; }

    [JsonProperty("3")]
    public string MarginTaxPercent { get; set; }

    [JsonProperty("4")]
    public string NetIncomePerYear { get; set; }

    public string NetIncomePerMonth { get; set; }

    public void calculateNetIncomePerMonth()
    {
        try
        {
            var nipyInt = int.Parse(NetIncomePerYear);
            NetIncomePerMonth = (nipyInt / 12) + "";
        }
        catch
        {

        }
    }
}

