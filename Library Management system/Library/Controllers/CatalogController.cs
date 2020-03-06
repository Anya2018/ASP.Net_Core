
using Library.Models.Catalog;
using Library.Models.CheckoutModels;
using LibraryData;
using LibraryServices;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Library.Controllers
{
    public class CatalogController : Controller
    {
        
        private readonly ILibraryAsset _assets;
        private readonly ICheckout _checkouts;

        //let's pass into constructor ILibraryAsset Interface:
        public CatalogController(ILibraryAsset assets, ICheckout checkouts)
        {
            // so we have access to the rest of the controllers:
            _assets = assets;
            _checkouts = checkouts;
        }

        public IActionResult Index()
        {
            
            var assetModels = _assets.GetAll();
            var listingResult = assetModels
                .Select(result => new AssetIndexListingModel
                {
                    Id = result.Id,
                    ImageUrl= result.ImageUrl,
                    //for Author OrDiretor we have to implement GetAuthorOrDirector function
                    //that we wrote in our service layer , and pass it the result.Id
                    AuthorOrDirector = _assets.GetAuthorOrDirector(result.Id),
                    //also use a function we wrote befor eto get it 
                    DeweyCallNumber = _assets.GetDeweyIndex(result.Id),
                    Title = result.Title,
                    //use fucntion again:
                    Type = _assets.GetType(result.Id)

                });

            var model = new AssetIndexModel()
            {
                Assets = listingResult
            };
            return View(model);
                
        }

        public IActionResult Detail(int id)
        {
            //retrieve the asset:
            var asset = _assets.GetById(id);

            var currentHolds = _checkouts.GetCurrentHolds(id)
                .Select(a => new AssetHoldModel
                {
                    HoldPlaced = _checkouts.GetCurrentHoldPlaced(a.Id),
                    PatronName = _checkouts.GetCurrentHoldPatronName(a.Id),
                });

            var model = new AssetDetailModel
            {
                AssetId = id,
                Title = asset.Title,
                Type = _assets.GetType(id),
                Year = asset.Year,
                Cost = asset.Cost,
                //status is the navigation property that itself contains properties,
                //we'llbe looking for the sstatus name here:
                Status = asset.Status.Name,
                ImageURL = asset.ImageUrl,
                AuthorOrDirector = _assets.GetAuthorOrDirector(id),
                CurrentLocation = _assets.GetCurrentLocation(id).Name,
                DeweyCallNumber = _assets.GetDeweyIndex(id),
                ISBN = _assets.GetIsbn(id),
                CheckoutHistory = _checkouts.GetCheckoutHistory(id),
                LatestChekout = _checkouts.GetLatestCheckout(id),
                PatronName = _checkouts.GetCurrentHoldPatronName(id),
                CurrentHolds = currentHolds,
            };
            return View(model);
        }

        public IActionResult Checkout(int id)
        {
            var asset = _assets.GetById(id);

            var model = new CheckoutModel
            {
                AssetId = id,
                ImageUrl = asset.ImageUrl,
                Title = asset.Title,
                LibraryCardId = "",
                IsCheckedOut = _checkouts.IsCheckedOut(id),
            };
            return View(model);
        }
        public IActionResult CheckIn(int id)
        {
            _checkouts.CheckInItem(id);
            return RedirectToAction("Detail", new { id = id });
        }

        public IActionResult Hold(int id)
        {
            var asset = _assets.GetById(id);

            var model = new CheckoutModel
            {
                AssetId = id,
                ImageUrl = asset.ImageUrl,
                Title = asset.Title,
                LibraryCardId = "",
                IsCheckedOut = _checkouts.IsCheckedOut(id),
                HoldCount = _checkouts.GetCurrentHolds(id).Count(),
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult PlaceCheckOut(int assetId, int libraryCardId)
        {
            _checkouts.CheckOutItem(assetId, libraryCardId);
            return RedirectToAction("Detail", new { id = assetId });
        }

        [HttpPost]
        public IActionResult PlaceHold(int assetId, int libraryCardId)
        {
            _checkouts.PlaceHold(assetId, libraryCardId);
            return RedirectToAction("Detail", new { id = assetId });
        }

        public IActionResult MarkLost(int assetId)
        {
            _checkouts.MarkLost(assetId);
            return RedirectToAction("Detail", new { id = assetId });
        }

        public IActionResult MarkFound(int assetId)
        {
            _checkouts.MarkFound(assetId);
            return RedirectToAction("Detail", new { id = assetId });
        }
    }
}

