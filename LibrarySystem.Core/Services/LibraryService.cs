namespace LibrarySystem.Core.Services;
using LibrarySystem.Core.Results;
using LibrarySystem.Core.Interfaces;
using LibrarySystem.Core.Models;

public class LibraryService
{
    private readonly IUserRepository _userRepository;
    private readonly IBookRepository _bookRepository;
    private readonly ILoanRepository _loanRepository;
    private readonly IReservationRepository _reservationRepository;

    public LibraryService(IUserRepository userRepository, IBookRepository bookRepository,
        ILoanRepository loanRepository,
        IReservationRepository reservationRepository)
    {
        _userRepository = userRepository;
        _bookRepository = bookRepository;
        _loanRepository = loanRepository;
        _reservationRepository = reservationRepository;
    }
    
    public BorrowResult BorrowBook(int userId, int bookId)
    {
        var user = _userRepository.GetById(userId);
        if (user == null)        
            return BorrowResult.UserNotFound;
        
        var book = _bookRepository.GetBook(bookId);
        if (book == null)
            return BorrowResult.BookNotFound;
        
        var existingLoan = _loanRepository.GetLoan(userId, bookId);
        if (existingLoan != null)
            return BorrowResult.BookAlreadyBorrowed;
        
        var reservations = _reservationRepository.GetReservationsForBook(bookId)
            .OrderBy(r => r.ReservedAt)
            .ToList();
        if (reservations.Any() && reservations.First().UserId != user.Id)
            return BorrowResult.ReservedForAnotherUser;
        
        if (book.AvailableCopies <= 0)
            return BorrowResult.NoAvailableCopies;
        
        _loanRepository.Add(new Loan
        {
            UserId = user.Id,
            BookId = book.Id,
        });
        
        book.AvailableCopies--;
        _bookRepository.Update(book);
        
        var userReservation = reservations.FirstOrDefault(r => r.UserId == user.Id);
        if (userReservation != null)
            _reservationRepository.Remove(userReservation);
        
        return BorrowResult.Success;
    }

    public ReserveResult ReserveBook(int userId, int bookId)
    {
        var user  = _userRepository.GetById(userId);
        if (user == null)
            return ReserveResult.UserNotFound;
        
        var book = _bookRepository.GetBook(bookId);
        if (book == null)
            return ReserveResult.BookNotFound;
        
        var existingLoan = _loanRepository.GetLoan(userId, bookId);
        if (existingLoan != null)
            return ReserveResult.AlreadyReserved;
        
        if (book.AvailableCopies > 0)
            return ReserveResult.BookAvailableForBorrowing;
        
        _reservationRepository.Add(new Reservation
        {
            UserId = user.Id,
            BookId = book.Id,
            ReservedAt = DateTime.UtcNow
        });
        
        return ReserveResult.Success;
    }

    public ReturnResult ReturnBook(int userId, int bookId)
    {
        var  user = _userRepository.GetById(userId);
        if (user == null)
            return ReturnResult.UserNotFound;
        
        var book = _bookRepository.GetBook(bookId);
        if (book == null)
            return ReturnResult.BookNotFound;
        
        var loan = _loanRepository.GetLoan(userId, bookId);
        if (loan == null)
            return ReturnResult.LoanNotFound;
        
        _loanRepository.Reove(loan);
        
        book.AvailableCopies++;
        _bookRepository.Update(book);
        
        return ReturnResult.Success;
    }
}