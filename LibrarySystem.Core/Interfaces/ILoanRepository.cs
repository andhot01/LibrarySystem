namespace LibrarySystem.Core.Interfaces;
using LibrarySystem.Core.Models;

public interface ILoanRepository
{
    Loan? GetLoan(int userId, int bookId);
    void Add(Loan loan);
    void Remove(Loan loan);
}