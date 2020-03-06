using LibraryData.Models;
using System.Collections.Generic;

namespace LibraryData
{
    public interface ILibraryBranch
    {
        IEnumerable<LibraryBranch> GetAll();
        //patrons of each library branch:
        IEnumerable<Patron> GetPatrons(int branchId);
        //libraryassets for a particular branch:
        IEnumerable<LibraryAsset> GetAssets(int branchId);
        IEnumerable<string> GetBranchHours(int branchId);

        LibraryBranch Get(int branchId);
        void Add(LibraryBranch newBranch);
        bool IsBranchOpen(int branchID);
    }
}
