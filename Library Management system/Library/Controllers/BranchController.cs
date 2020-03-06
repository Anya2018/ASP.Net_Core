using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.Models.Branch;
using LibraryData;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    public class BranchController : Controller
    {
        private readonly ILibraryBranch _branch;
        public BranchController(ILibraryBranch branch)
        {
            _branch = branch;
        }

        public IActionResult Index()
        {
            var branches = _branch.GetAll().Select(b => new BranchDetailModel
            {
                Id = b.Id,
                BranchName = b.Name,
                IsOpen = _branch.IsBranchOpen(b.Id),
                NumberOfAssets = _branch.GetAssets(b.Id).Count(),
                NumberOfPatrons = _branch.GetPatrons(b.Id).Count(),
            }); ;
            var model = new BranchIndexModel
            {
                Branches = branches
            };
            return View(model);
        }

        public IActionResult Detail (int id)
        {
            var branch = _branch.Get(id);
            var model = new BranchDetailModel
            {
                BranchName = branch.Name,
                Description = branch.Description,
                Address = branch.Address,
                Telephone = branch.Telephone,
                OpenDate = branch.FoundationDate.ToString("yyyy-MM-dd"),
                NumberOfPatrons = _branch.GetPatrons(id).Count(),
                NumberOfAssets = _branch.GetAssets(id).Count(),
                TotalAssetValue = _branch.GetAssets(id).Sum(a=>a.Cost),
                ImageURL = branch.ImageUrl,
                HoursOpen = _branch.GetBranchHours(id)
            };

            return View(model);
        }
    }
}