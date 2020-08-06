namespace BuyingHistory.Migrations
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    //using System.Data.Entity;
    using System.Data.Entity.Migrations;
    //using System.Linq;
    using System.Xml;
    using BuyingHistory.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<BuyingHistory.Models.BuyingHistoryContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "BuyingHistory.Models.BuyingHistoryContext";
        }

        protected override void Seed(BuyingHistory.Models.BuyingHistoryContext context)
        {
            //  This method will be called after migrating to the latest version.
            
            var doc = new XmlDocument();

            // Moved bh.xml from BuyingHistory/bin/Debug/ to BuyingHistory/
            string projectDir = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            doc.Load(projectDir + Path.DirectorySeparatorChar + "bh.xml");

            var saleNodes = doc.GetElementsByTagName("sale");
            var sales = new List<Sale>();
            foreach (XmlElement saleNode in saleNodes)
            {
                var sale = new Sale
                {
                    Store = saleNode.GetElementsByTagName("store")[0].InnerText,
                    Seller = saleNode.GetElementsByTagName("seller")[0].InnerText,
                    Total = Convert.ToDecimal(saleNode.GetElementsByTagName("total")[0].InnerText),
                    Date = Convert.ToDateTime(saleNode.GetElementsByTagName("date")[0].InnerText),
                    Items = new List<Item>()
                };

                foreach (XmlElement itemNode in saleNode.GetElementsByTagName("item"))
                {
                    var item = new Item
                    {
                        Price = Convert.ToDecimal(itemNode.FirstChild.InnerText),
                        Albums = new List<Album>()
                    };

                    foreach (XmlElement albumNode in itemNode.GetElementsByTagName("album"))
                    {
                        var album = new Album
                        {
                            Band = albumNode.GetElementsByTagName("band")[0].FirstChild.InnerText,
                            Title = albumNode.GetElementsByTagName("title")[0].FirstChild.InnerText,
                            Format = albumNode.GetElementsByTagName("format")[0].FirstChild.InnerText
                        };
                        item.Albums.Add(album);
                    }
                    sale.Items.Add(item);
                }
                sales.Add(sale);
            }
            sales.ForEach(sale => context.Sales.AddOrUpdate(s => new { s.Store, s.Seller, s.Date, s.Total }, sale));
            context.SaveChanges();
        }

    }
}
