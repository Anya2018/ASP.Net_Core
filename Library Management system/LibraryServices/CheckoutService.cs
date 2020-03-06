using LibraryData;
using LibraryData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibraryServices
{
    public class CheckoutService : ICheckout
    {
        private readonly LibraryContext _context;
        public CheckoutService(LibraryContext context)
        {
            _context = context;
        }
        public void Add(Checkout newCheckout)
        {
            _context.Add(newCheckout);
            _context.SaveChanges();
        }

        public IEnumerable<Checkout> GetAll()
        {
            return _context.Checkouts;
        }

        public Checkout GetById(int checkoutId)
        {
            return
                GetAll()
                .FirstOrDefault(c => c.Id == checkoutId);
        }

        public IEnumerable<CheckoutHistory> GetCheckoutHistory(int id)
        {
            return _context.CheckoutHistories
                .Include(a => a.LibraryAsset)
                .Include(a => a.LibraryCard)
                .Where(a => a.LibraryAsset.Id == id);
        }

        public IEnumerable<Hold> GetCurrentHolds(int id)
        {
            return _context.Holds
                .Include(h => h.LibraryAsset)
                //???:
                .Include(h => h.LibraryCard)
                .Where(h => h.LibraryAsset.Id == id);
        }

        public Checkout GetLatestCheckout(int id)
        {
            return _context.Checkouts
                .Where(h => h.LibraryAsset.Id == id)
                .OrderByDescending(h => h.Since)
                .FirstOrDefault();
        }

        public void MarkFound(int assetId)
        {
            var now = DateTime.Now;
            
            UpdateAssetStatus(assetId,"Available");
            RemoveExistingCheckouts(assetId);
            CloseExistingCheckouts(assetId,now);

            _context.SaveChanges();
        }

        private void UpdateAssetStatus(int assetId, string newStatus)
        {
            var item = _context.LibraryAssets
                .FirstOrDefault(a => a.Id == assetId);
            _context.Update(item);
            item.Status = _context.Statuses
                .FirstOrDefault(a => a.Name == newStatus);
        }

        private void CloseExistingCheckouts(int assetId, DateTime now)
        {
            //close any existing checkout histories:
            var history = _context.CheckoutHistories
                .Include(h => h.LibraryAsset)
                .Include(h => h.LibraryCard)
                .FirstOrDefault(h => h.LibraryAsset.Id == assetId && h.CheckedIn == null);

            _context.Update(history);
            if (history != null)
            {
                _context.Update(history);
                history.CheckedIn = now;
            }
        }

        private void RemoveExistingCheckouts(int assetId)
        {
            var checkout = _context.Checkouts
                .Include(a => a.LibraryAsset)
                .Include(a => a.LibraryCard)
                .FirstOrDefault(a => a.LibraryAsset.Id == assetId);

            if (checkout != null)
            {
                _context.Remove(checkout);
            }
        }

        public void MarkLost(int assetId)
        {
            UpdateAssetStatus(assetId, "Lost");
            _context.SaveChanges();
        }

        public void CheckInItem(int assetId)
        {
            var now = DateTime.Now;
            var item = _context.LibraryAssets
                .FirstOrDefault(a => a.Id == assetId);

            RemoveExistingCheckouts(assetId);

            CloseExistingCheckouts(assetId, now);

            var currentHolds = _context.Holds
                .Include(h => h.LibraryAsset)
                .Include(h => h.LibraryCard)
                .Where(h => h.LibraryAsset.Id == assetId);

            if (currentHolds.Any())
            {
                CheckoutToEarliestHold(assetId, currentHolds);
                return;
            }

            UpdateAssetStatus(assetId, "Available");
            _context.SaveChanges();
        }

        private void CheckoutToEarliestHold(int assetId, IQueryable<Hold> currentHolds)
        {
            var earliestHold = currentHolds
                .OrderBy(holds => holds.HoldPlaced)
                .FirstOrDefault();

            var libraryCard = earliestHold.LibraryCard;
            _context.Remove(earliestHold);
            _context.SaveChanges();

            CheckOutItem(assetId,libraryCard.Id);
        }

        public void CheckOutItem(int assetId, int libraryCardId)
        {
            if (IsCheckedOut(assetId))
            {
                return;
                //Add logic to handle feedback to the user
            }
            
            var item = _context.LibraryAssets
                .Include(a => a.Status)
                .FirstOrDefault(a => a.Id == assetId);

            _context.Update(item);
            UpdateAssetStatus(assetId, "Checked Out");

            var libraryCard = _context.LibraryCards
                .Include(card => card.Checkouts)
                .FirstOrDefault(card => card.Id == libraryCardId);

            var now = DateTime.Now;
            var checkout = new Checkout
            {
                LibraryAsset = item,
                LibraryCard = libraryCard,
                Since = now,
                Until = GetDeafultCheckoutTime(now)
            };

            _context.Add(checkout);

            var checkoutHistory = new CheckoutHistory
            {
                CheckedOut = now,
                LibraryAsset = item,
                LibraryCard = libraryCard,
            };

            _context.Add(checkoutHistory);

            _context.SaveChanges();
        }

        private DateTime GetDeafultCheckoutTime(DateTime now)
        {
            return now.AddDays(30);
        }

        public bool IsCheckedOut(int id)
        {
            //It will return false if the item is not checked out:
            //store it as a bool:
            return _context.Checkouts
                .Where(co => co.LibraryAsset.Id == id)
                .Any();
        }

        public void PlaceHold(int assetId, int libraryCardId)
        {
            var now = DateTime.Now;
            var item = _context.LibraryAssets
                .Include(a=>a.Status)
                .FirstOrDefault(a => a.Id == assetId);
            var libraryCard = _context.LibraryCards
                .FirstOrDefault(card => card.Id == libraryCardId);

            // when we place a hold , first we have to check out if it's available:
            if(item.Status.Name == "Availbale")
            {
                UpdateAssetStatus(assetId, "On hold");
            }

            var hold = new Hold
            {
                LibraryAsset = item,
                LibraryCard = libraryCard,
                HoldPlaced = now,
            };

            _context.Add(hold);
            _context.SaveChanges();
        }

        public string GetCurrentHoldPatronName(int holdId)
        {
            //let's retrieve our hold:
            var hold = _context.Holds
                .Include(h => h.LibraryAsset)
                .Include(h => h.LibraryCard)
                .FirstOrDefault(h => h.Id == holdId);

            //getting card I dfrom this hold:
            var cardId = hold?.LibraryCard.Id;

            //from the card id we are able to get patrons name
            var patron = _context.Patrons
                .Include(p => p.LibraryCard)
                .FirstOrDefault(p => p.LibraryCard.Id == cardId);

            return patron?.FirstName + " " + patron?.LastName;
        }

        public DateTime GetCurrentHoldPlaced(int holdId)
        {
            return
                _context.Holds
                .Include(h => h.LibraryAsset)
                .Include(h => h.LibraryCard)
                .FirstOrDefault(h => h.Id == holdId)
                .HoldPlaced;
        }

        public string GetCurrentCheckoutPatron(int assetId)
        {
            var checkout = GetCheckoutByAssetId(assetId);

            if (checkout == null)
            {
                return "";
            };

            var cardId = checkout.LibraryCard.Id;
            var patron = _context.Patrons
                .Include(p => p.LibraryCard)
                .FirstOrDefault(p => p.LibraryCard.Id == assetId);

            return patron.FirstName + " " + patron.LastName;
        }

        private Checkout GetCheckoutByAssetId(int assetId)
        {
            return
                _context.Checkouts
                .Include(co => co.LibraryAsset)
                .Include(co => co.LibraryCard)
                .FirstOrDefault(co => co.LibraryAsset.Id == assetId);
        }

        
    }
}
