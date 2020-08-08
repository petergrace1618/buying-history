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
                    Console.WriteLine("*** ADD SALE TO BUYING HISTORY ***");
                    Console.WriteLine();
                    Console.WriteLine($"Sale #{++totalSales}");
                    sale = new Sale() { Items = new List<Item>() };

                    // SALE TABLE FIELDS
                    sale.Store = GetString("  Store: ");
                    sale.Seller = GetString("  Seller: ", false); // Seller can be null
                    sale.Total = TryGetDecimal("  Total: ");
                    sale.Date = TryGetDate("  Date: ");

                    nSaleItems = 0;
                    do
                    {
                        totalItems++;
                        nSaleItems++;
                        Console.WriteLine($"    Item #{nSaleItems}");
                        item = new Item() { Albums = new List<Album>() };

                        // ITEM TABLE FIELDS
                        item.Price = TryGetDecimal("      Price: ");

                        nItemAlbums = 0;
                        do
                        {
                            totalAlbums++;
                            nItemAlbums++;
                            Console.WriteLine($"      Album #{nItemAlbums}");
                            album = new Album();

                            var formatOptions = new Dictionary<char, string> 
                            { { '1', "CASSETTE" }, { '2', "CD" } };

                            // ALBUM TABLE FIELDS
                            album.Band = GetString("        Band: ");
                            album.Title = GetString("        Title: ");
                            album.Format = GetOption("        Format: ", formatOptions);


                            // Add album to item
                            item.Albums.Add(album);
                            Console.WriteLine();

                        } while (GetYesNo("Add another album to this item? "));

                        // No more albums in item, so add item
                        sale.Items.Add(item);
                        Console.WriteLine();

                    } while (GetYesNo("Add another item to this sale? "));

                    // No more items so add sale
                    db.Sales.Add(sale);
                    Console.WriteLine();

                } while (GetYesNo("Add another sale? "));

                // No more sales, so save
                Console.WriteLine($"\nSaving {totalSales} sale(s), {totalItems} item(s), and {totalAlbums} album(s)...");
                try 
                {
                    db.SaveChanges();
                }
                catch(Exception e)
                {
                    Exception ie = e;
                    // Want the Message in the inner SqlException that says explicity
                    // "Cannot insert duplicate key row", and the value of the key.
                    do
                    {
                        ie = (Exception)ie.InnerException;
                    } while (ie.GetType() != typeof(System.Data.SqlClient.SqlException) &&
                             ie != null);
                    
                    if (ie == null)
                    {
                        // Some other exception
                        Console.WriteLine($"{e.GetType()}: {e.Message}");
                    }
                    else
                    {
                        // SqlException. 
                        Console.WriteLine($"{ie.Message}");
                    }
                }
                
            }
        }

        static string GetString(string prompt, bool notNull = true)
        {
            string s;
            do
            {
                Console.Write(prompt);
                s = Console.ReadLine();
            } while (String.IsNullOrEmpty(s) && notNull);

            return s;
        }

        static DateTime TryGetDate(string prompt)
        {
            var date = new DateTime();
            bool formatException;

            do
            {
                Console.Write(prompt);
                formatException = false;
                try
                {
                    date = Convert.ToDateTime(Console.ReadLine());
                }
                catch (FormatException) {
                    formatException = true;
                }
            } while(formatException);

            return date;
        }

        static Decimal TryGetDecimal(string prompt)
        {
            Decimal d = 0;
            bool formatException;

            do
            {
                Console.Write(prompt);
                formatException = false;
                try
                {
                    d = Convert.ToDecimal(Console.ReadLine());
                }
                catch (FormatException)
                {
                    formatException = true;
                }
            } while (formatException);

            return d;
        }

        static string GetOption(string prompt, Dictionary<char, string> options)
        {
            string optionsText = "| ";
            foreach (char o in options.Keys)
            {
                optionsText += $"{o} - {options[o]} | ";
            }

            string response;
            do
            {
                response = GetString($"{prompt}{optionsText}");
            } while (!options.ContainsKey(response[0]));

            return options[response[0]];
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
                response = GetString(prompt);
            } while (!yesNoKeys.ContainsKey(response[0]));

            return yesNoKeys[response[0]];
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
