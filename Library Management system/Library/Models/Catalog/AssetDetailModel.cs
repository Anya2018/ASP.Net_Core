using LibraryData.Models;
using System;
using System.Collections.Generic;

namespace Library.Models.Catalog
{
    public class AssetDetailModel
    {
        public int AssetId { get; set; }
        public string Title { get; set; }
        public string AuthorOrDirector { get; set; }
        public string Type { get; set; }
        public int Year { get; set; }
        public string ISBN { get; set; }
        public string DeweyCallNumber { get; set; }
        public string Status { get; set; }
        public decimal Cost { get; set; }
        public string CurrentLocation { get; set; }
        public string ImageURL { get; set; }
        public string PatronName { get; set; }
        public Checkout LatestChekout { get; set; }
        //??:
        public LibraryCard CurrentAssociatedLibraryCard { get; set; }
        //represents set of all the checkouts that our patrons have placed on this particular asset:
        public IEnumerable<CheckoutHistory> CheckoutHistory { get; set; }
        //it will call current holds,I'll just define this class here in the same file:
        public IEnumerable<AssetHoldModel> CurrentHolds { get; set; }
    }
    public class AssetHoldModel
    {
        public string PatronName { get; set; }
        //you can also store as a string:
        public DateTime HoldPlaced { get; set; }
    }
}
