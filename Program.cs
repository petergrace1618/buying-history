using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using BuyingHistory.Models;
using System.Linq;

namespace BuyingHistory
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                PrintUsage();
            }
            else if (args[0].ToLower() == "add" && args.Length == 1) 
            {
                AddSale();
            }
            else if (args[0].ToLower() == "print" && args.Length == 1)
            {
                Console.WriteLine("Loading...");
                PrintDb();
            }
            else if (args[0].ToLower() == "xml" && args.Length == 1)
            {
                Console.WriteLine("Loading...");
                PrintDbXml();
            }
            else if (args[0].ToLower() == "xml" && args.Length == 2)
            {
                Console.WriteLine("Loading...");
                PrintDbXml(args[1]);
            }
            else
            {
                PrintUsage();
            }
        }

        static void AddSale()
        {
            // variables needed for UI
            Sale sale;
            Item item;
            Album album;
            int totalSales = 0;
            int totalItems = 0;
            int totalAlbums = 0;
            int nSaleItems = 0;      // num items in a sale
            int nItemAlbums = 0;     // num albums in an item

            using (var db = new BuyingHistoryContext())
            {
                do
                {
                    // Get sale data
                    Console.WriteLine("*** ADD SALE TO BUYING HISTORY ***");
                    Console.WriteLine();
                    Console.WriteLine($"Sale #{++totalSales}");
                    sale = new Sale() { Items = new List<Item>() };

                    GetSaleData(sale);

                    nSaleItems = 0;
                    do
                    {
                        // Get items for each sale
                        totalItems++;
                        nSaleItems++;
                        Console.WriteLine($"    Item #{nSaleItems}");
                        item = new Item() { Albums = new List<Album>() };

                        GetItemData(item);

                        nItemAlbums = 0;
                        do
                        {
                            // Get albums for item
                            totalAlbums++;
                            nItemAlbums++;
                            Console.WriteLine($"      Album #{nItemAlbums}");
                            album = new Album();

                            GetAlbumData(album);

                            // Add album to item
                            item.Albums.Add(album);

                        } while (GetYesNo("\nAdd another album to this item? "));

                        // No more albums in item, so add item
                        sale.Items.Add(item);

                    } while (GetYesNo("\nAdd another item to this sale? "));

                    //Console.WriteLine();
                    //PrintSale(sale);
                    //Console.WriteLine();

                    // No more items so add sale
                    db.Sales.Add(sale);

                } while (GetYesNo("\nAdd another sale? "));

                // No more sales, so save
                Console.WriteLine($"\nSaving {totalSales} sale(s), {totalItems} item(s), and {totalAlbums} album(s)...");
                db.SaveChanges();
            }
        }

        static void GetSaleData(Sale sale)
        {
            Console.Write("  Store: ");
            sale.Store = Console.ReadLine().Trim();

            Console.Write("  Seller: ");
            sale.Seller = Console.ReadLine().Trim();

            Console.Write("  Date: ", DateTime.Now);
            sale.Date = TryGetDate();

            Console.Write("  Total: ");
            sale.Total = TryGetDecimal();
        }

        static void GetItemData(Item item)
        {
            Console.Write("      Price: ");
            item.Price = TryGetDecimal();
        }

        static void GetAlbumData(Album album)
        {
            var formatOptions = new Dictionary<char, string> { { '1', "CASSETTE" }, { '2', "CD" } };
            
            Console.Write("        Band: ");
            album.Band = Console.ReadLine().Trim();

            Console.Write("        Title: ");
            album.Title = Console.ReadLine().Trim();

            //Console.Write("        Format: ");
            album.Format = GetOption("        Format: ", formatOptions);
        }

        static bool GetYesNo(string prompt)
        {
            var yesNoKeys = new Dictionary<char, bool>
            {
                { 'y', true },
                { 'Y', true },
                { 'n', false },
                { 'N', false }
            };
            string response;

            do
            {
                Console.Write(prompt);
                response = Console.ReadLine();
            } while (!yesNoKeys.ContainsKey(response[0]));

            return yesNoKeys[response[0]];
        }

        static string GetOption(string prompt, Dictionary<char, string> options)
        {
            string optionsText = "| ";
            foreach (char o in options.Keys)
            {
                optionsText += $"{o} - {options[o]} | ";
            }

            Console.Write(prompt);
            string response;
            do
            {
                Console.Write($"{optionsText}");
                response = Console.ReadLine();
            } while (!options.ContainsKey(response[0]));

            return options[response[0]];
        }

        static DateTime TryGetDate()
        {
            var d = new DateTime();
            bool fe; // format exception flag

            do
            {
                fe = false;
                try
                {
                    d = Convert.ToDateTime(Console.ReadLine());
                }
                catch (FormatException) {
                    fe = true;
                }
            } while(fe);

            return d;
        }

        static Decimal TryGetDecimal()
        {
            Decimal d = 0;
            bool fe;

            do
            {
                fe = false;
                try
                {
                    d = Convert.ToDecimal(Console.ReadLine());
                }
                catch (FormatException)
                {
                    fe = true;
                }
            } while (fe);

            return d;
        }

        static void PrintSale(Sale sale)
        {
            var sb = new StringBuilder(
                 "Sale\n" +
                $"  Store: \"{sale.Store}\"\n" +
                $"  Seller: \"{sale.Seller}\"\n" +
                $"  Date: {sale.Date:d}\n" +
                $"  Total: {sale.Total}\n"
            );

            foreach (Item item in sale.Items)
            {
                sb.Append( "  Item\n");
                sb.Append($"    Price: {item.Price}\n");
                foreach (Album album in item.Albums)
                {
                    sb.Append( "    Album\n");
                    sb.Append($"      Band: \"{album.Band}\"\n");
                    sb.Append($"      Title: \"{album.Title}\"\n");
                    sb.Append($"      Format: {album.Format}\n");
                }
            }
            Console.Write(sb.ToString());
        }

        static void PrintDb()
        {
            int nSales = 0, nItems = 0, nAlbums = 0;
            using (var db = new BuyingHistoryContext())
            {
                foreach (var sale in db.Sales.OrderBy(s => s.Date))
                {
                    nSales++;
                    nItems += sale.Items.Count;
                    foreach (var item in sale.Items)
                        nAlbums += item.Albums.Count;
                    PrintSale(sale);
                }
                Console.WriteLine($"\n{nSales} sale(s), {nItems} item(s), {nAlbums} album(s)");
            }
        }

        static void PrintDbXml(string filename = null)
        {
            var settings = new XmlWriterSettings { Indent = true };
            XmlWriter xw;
            if (String.IsNullOrEmpty(filename))
            {
                settings.OmitXmlDeclaration = true;
                xw = XmlWriter.Create(Console.Out, settings);
            }
            else
            {
                xw = XmlWriter.Create(filename, settings);
            }

            using (var db = new BuyingHistoryContext())
            {
                xw.WriteStartDocument();
                xw.WriteStartElement("buyinghistory");

                foreach(Sale sale in db.Sales.OrderBy(s => s.Date))
                {
                    xw.WriteStartElement("sale");
                    xw.WriteElementString("store", sale.Store);
                    xw.WriteElementString("seller", sale.Seller);
                    xw.WriteElementString("date", $"{sale.Date:yyyy-MM-dd}");
                    xw.WriteElementString("total", $"{sale.Total:f2}");
                    foreach(Item item in sale.Items)
                    {
                        xw.WriteStartElement("item");
                        xw.WriteElementString("price", $"{item.Price:f2}");
                        foreach(Album album in item.Albums)
                        {
                            xw.WriteStartElement("album");
                            xw.WriteElementString("band", album.Band);
                            xw.WriteElementString("title", album.Title);
                            xw.WriteElementString("format", album.Format);
                            xw.WriteEndElement();
                        }
                        xw.WriteEndElement();
                    }
                    xw.WriteEndElement();
                }
                xw.WriteEndElement();
                xw.WriteEndDocument();

                xw.Flush();
                xw.Close();
            }
            // Add newline if printing to console
            if (String.IsNullOrEmpty(filename))
                Console.WriteLine();
        }

        static void PrintUsage(string errorMsg = null)
        {
            string basename = System.IO.Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]);
            string usageStr =
                "Usage:\n" +
                $"{basename} [ add | print | xml [FILE] ]\n" +
                "\n" +
                "add        Add sale to database\n" +
                "print      Print contents of DB in human readable form to stdout\n" +
                "xml        Print contents of DB as XML to stdout.\n" +
                "xml FILE   Save contents of DB as XML to FILE\n";
            if (!String.IsNullOrEmpty(errorMsg))
                Console.WriteLine($"{basename}: {errorMsg}");
            Console.WriteLine(usageStr);
        }
    }
}
