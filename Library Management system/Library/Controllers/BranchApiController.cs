using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.Models.Branch;
using LibraryData;
using LibraryData.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchApiController : ControllerBase
    {
        private readonly ILibraryBranch _branch;
        public BranchApiController(ILibraryBranch branch)
        {
            _branch = branch;
        }

        //// GET: api/BranchApi
        [HttpGet]
        public IEnumerable<BranchDetailModel> GetLibraryBranches()
        {
            var branches = _branch.GetAll().Select(b => new BranchDetailModel
            {
                Id = b.Id,
                BranchName = b.Name,
                IsOpen = _branch.IsBranchOpen(b.Id),
                NumberOfAssets = _branch.GetAssets(b.Id).Count(),
                NumberOfPatrons = _branch.GetPatrons(b.Id).Count(),
            }); ;
            return branches;
        }       
    }
}