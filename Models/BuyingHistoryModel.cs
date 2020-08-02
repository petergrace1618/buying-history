using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace BuyingHistory.Models
{
    public class Sale
    {
        public int SaleId { get; set; }
        public string Store { get; set; }
        public string Seller { get; set; }
        public decimal Total { get; set; }
        public DateTime Date { get; set; }

        public virtual List<Item> Items { get; set; }
    }

    public class Item
    {
        public int ItemId { get; set; }
        public int SaleId { get; set; }
        public decimal Price { get; set; }
        
        public virtual Sale Sale { get; set; }
        public virtual List<Album> Albums { get; set; }
    }

    public class Album
    {
        public int AlbumId { get; set; }
        public int ItemId { get; set; }
        public string Band { get; set; }
        public string Title { get; set; }
        public string Format { get; set; }
        
        public virtual Item Item { get; set; }
    }

    public class BuyingHistoryContext : DbContext
    {
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Album> Albums { get; set; }
    }
}
