namespace LibrarySystem.Tests;

using FluentAssertions;
using LibrarySystem.Core.Services;
using LibrarySystem.Core.Results;
using LibrarySystem.Core.Interfaces;
using LibrarySystem.Core.Models;
using Moq;

public class ReserveBookTests
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
    public void ReserveBook_ShouldReturnSuccess_WhenBookCanBeReserved()
    {
        //Arrange
        _book.AvailableCopies = 0;
        SetupExistingUserAndBook();
        _loanRepositoryMock.Setup(x => x.GetLoan(1, 1)).Returns((Loan?)null);
        _reservationRepositoryMock.Setup(x => x.GetReservation(1, 1)).Returns((Reservation?)null);
        
        //Act
        var result = _libraryService.ReserveBook(1, 1);
        
        //Assert
        result.Should().Be(ReserveResult.Success);
        _reservationRepositoryMock.Verify(x => x.Add(It.Is<Reservation>(
            r => r.UserId == 1 && r.BookId == 1)), Times.Once);
    }
}