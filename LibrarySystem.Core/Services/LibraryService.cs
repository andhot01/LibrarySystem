namespace LibrarySystem.Core.Services;
using LibrarySystem.Core.Results;

public class LibraryService
{
    public BorrowResult BorrowBook(int userId, int bookId)
    {
        //method here
        return BorrowResult.Success;
    }

    public ReserveResult ReserveBook(int userId, int bookId)
    {
        return ReserveResult.Success;
    }

    public ReturnResult ReturnBook(int userId, int bookId)
    {
        return ReturnResult.Success;
    }
}