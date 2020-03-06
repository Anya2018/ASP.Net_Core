using Library.Models.Patron;
using LibraryData;
using LibraryData.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Controllers
{
    public class PatronController : Controller
    {
        private readonly IPatron _patrons;
        public PatronController(IPatron patrons)
        {
            _patrons = patrons;
        }

        public IActionResult Index()
        {
            var allPatrons = _patrons.GetAll();

            var patronModels = allPatrons.Select(p => new PatronDetailModel
            {
                Id = p.ID,
                FirstName = p.FirstName,
                LastName = p.LastName,
                LibraryCardId = p.LibraryCard.Id,
                OverdueFees = p.LibraryCard.Fees,
                HomeLibraryBranch = p.HomeLibraryBranch.Name,
            }).ToList();

            var model = new PatronIndexModel()
            {
                Patrons = patronModels
            };
            return View(model);
        }       
        public IActionResult Detail(int id) 
        {
            var patron = _patrons.Get(id); 

            var model = new PatronDetailModel
            {
                LastName = patron.LastName,
                FirstName = patron.FirstName,
                Address = patron.Address,
                HomeLibraryBranch = patron.HomeLibraryBranch.Name,
                MemberSince = patron.LibraryCard.Created,
                OverdueFees = patron.LibraryCard.Fees,
                LibraryCardId = patron.LibraryCard.Id,
                Telephone = patron.TelephoneNum,
                AssetCheckedOut = _patrons.GetCheckouts(id).ToList() ?? new List<Checkout>(),
                CheckoutHistory = _patrons.GetCheckoutHistory(id),
                Holds = _patrons.GetHolds(id),
            };
            return View(model);
        }
    }
}
