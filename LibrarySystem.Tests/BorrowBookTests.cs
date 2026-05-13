namespace LibrarySystem.Tests;

using FluentAssertions;
using LibrarySystem.Core.Services;
using LibrarySystem.Core.Results;
using LibrarySystem.Core.Interfaces;
using LibrarySystem.Core.Models;
using Moq;

public class BorrowBookTests
{
    private Mock<IBookRepository> _bookRepositoryMock = null!;
    private Mock<IUserRepository> _userRepositoryMock = null!;
    private Mock<ILoanRepository> _loanRepositoryMock = null!;
    private Mock<IReservationRepository> _reservationRepositoryMock = null!;
    private LibraryService _libraryService = null!;
    
    private LibraryUser _user = null!;
    private Book _book = null!;

    [SetUp]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _bookRepositoryMock = new Mock<IBookRepository>();
        _loanRepositoryMock = new Mock<ILoanRepository>();
        _reservationRepositoryMock = new Mock<IReservationRepository>();
        
        _libraryService = new LibraryService(_userRepositoryMock.Object, _bookRepositoryMock.Object,
            _loanRepositoryMock.Object, _reservationRepositoryMock.Object);

        _user = new LibraryUser
        {
            Id = 1,
            Name = "Bob"
        };
        
        _book = new Book()
        {
            Id = 1,
            Title = "Test Book",
            TotalCopies = 4,
            AvailableCopies = 1
        };
    }

    private void SetupExistingUserAndBook()
    {
        _userRepositoryMock.Setup(x => x.GetById(1)).Returns(_user);
        _bookRepositoryMock.Setup(x => x.GetBook(1)).Returns(_book);
    }

    [Test]
    public void BorrowBook_ShouldReturnSuccess_WhenBookIsAvailable()
    {
        //Arrange
        SetupExistingUserAndBook();
        _loanRepositoryMock.Setup(x => x.GetLoan(1, 1)).Returns((Loan?)null);
        _reservationRepositoryMock.Setup(x => x.GetReservationsForBook(1)).Returns(new List<Reservation>());
        
        //Act
        var result = _libraryService.BorrowBook(1, 1);
        
        //Assert
        result.Should().Be(BorrowResult.Success);
        _book.AvailableCopies.Should().Be(0);
        _loanRepositoryMock.Verify(x =>
                x.Add(It.Is<Loan>(l => l.UserId == 1 && l.BookId == 1)), Times.Once);
        _bookRepositoryMock.Verify(x => x.Update(_book), Times.Once);
    }
    
    [Test]
    public void BorrowBook_ShouldReturnUserNotFound_WhenUserDoesNotExist()
    {
        //Arrange
        _userRepositoryMock.Setup(x => x.GetById(1)).Returns((LibraryUser?)null);
        
        //Act
        var result = _libraryService.BorrowBook(1, 1);
        
        //Assert
        result.Should().Be(BorrowResult.UserNotFound);
    }
    
    [Test]
    public void BorrowBook_ShouldReturnBookNotFound_WhenBookDoesNotExist()
    {
        //Arrange
        _userRepositoryMock.Setup(x => x.GetById(1)).Returns(_user);
        _bookRepositoryMock.Setup(x => x.GetBook(1)).Returns((Book?)null);
        
        //Act
        var result = _libraryService.BorrowBook(1, 1);
        
        //Assert
        result.Should().Be(BorrowResult.BookNotFound);
    }
    
    [Test]
    public void BorrowBook_ShouldReturnAlreadyBorrowed_WhenUserAlreadyHasBook()
    {
        //Arrange
        var existingLoan = new Loan()
        {
            UserId = 1,
            BookId = 1
        };
        SetupExistingUserAndBook();
        _loanRepositoryMock.Setup(x => x.GetLoan(1, 1)).Returns(existingLoan);
        
        //Act
        var result = _libraryService.BorrowBook(1, 1);
        
        //Assert
        result.Should().Be(BorrowResult.BookAlreadyBorrowed);
    }
    
    [Test]
    public void BorrowBook_ShouldReturnReservedForAnotherUser_WhenBookIsReservedByAnotherUser()
    {
        //Arrange
        var reservation = new List<Reservation>
        {
            new Reservation()
            {
                UserId = 2,
                BookId = 1,
                ReservedAt = DateTime.Now.AddMinutes(-10)
            },
            new Reservation
            {
                UserId = 1,
                BookId = 1,
                ReservedAt = DateTime.Now
            }
        };
        
        SetupExistingUserAndBook();
        _reservationRepositoryMock.Setup(x => x.GetReservationsForBook(1)).Returns(reservation);
        _loanRepositoryMock.Setup(x => x.GetLoan(1, 1)).Returns((Loan?)null);
        
        //Act
        var result = _libraryService.BorrowBook(1, 1);
        
        //Assert
        result.Should().Be(BorrowResult.ReservedForAnotherUser);
    }

    [Test]
    public void BorrowBook_ShouldReturnNoAvailableCopies_WhenBookIsNotAvailable()
    {
        //Arrange
        _book.AvailableCopies = 0;
        SetupExistingUserAndBook();
        _loanRepositoryMock.Setup(x => x.GetLoan(1, 1)).Returns((Loan?)null);
        _reservationRepositoryMock.Setup(x => x.GetReservationsForBook(1)).Returns(new List<Reservation>());
        
        //Act
        var result = _libraryService.BorrowBook(1, 1);
        
        //Assert
        result.Should().Be(BorrowResult.NoAvailableCopies);
    }
}