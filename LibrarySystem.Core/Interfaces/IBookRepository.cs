namespace LibrarySystem.Core.Interfaces;
using LibrarySystem.Core.Models;

public interface IBookRepository
{
    Book? GetBook(int bookId);
    void Update(Book book);
}