using LibraryData.Models;
using System.Collections.Generic;

namespace LibraryData
{
    public interface IPatron
    {
        Patron Get(int id);
        IEnumerable<Patron> GetAll();
        void Add(Patron newPatron);

        IEnumerable<CheckoutHistory> GetCheckoutHistory(int patronId);
        IEnumerable<Hold> GetHolds(int patronId);
        IEnumerable<Checkout> GetCheckouts(int patronId);
    }
}
