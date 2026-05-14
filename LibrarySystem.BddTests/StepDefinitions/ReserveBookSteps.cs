using FluentAssertions;
using LibrarySystem.Core.Interfaces;
using LibrarySystem.Core.Models;
using LibrarySystem.Core.Results;
using LibrarySystem.Core.Services;
using Moq;
using Reqnroll;

namespace LibrarySystem.BddTests.StepDefinitions;

[Binding]
public class ReserveBookSteps
{
    private readonly Mock<IBookRepository> _bookRepositoryMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<ILoanRepository> _loanRepositoryMock = new();
    private readonly Mock<IReservationRepository> _reservationRepositoryMock = new();

    private readonly LibraryUser _user = new()
    {
        Id = 1,
        Name = "Bob"
    };
    
    private Book? _book;
    private void SetupBookWithAvailableCopies(int availableCopies)
    {
        _book = new Book
        {
            Id = 1,
            Title = "Test Book",
            TotalCopies = 4,
            AvailableCopies = availableCopies
        };
         _bookRepositoryMock.Setup(x => x.GetBook(1)).Returns(_book);
    }
    
    private ReserveResult _result;

    private LibraryService CreateService()
    {
        return new LibraryService(_userRepositoryMock.Object, _bookRepositoryMock.Object, 
            _loanRepositoryMock.Object, _reservationRepositoryMock.Object);
    }

    
    
    [Given("a user exists")]
    public void GivenAUserExists()
    {
        _userRepositoryMock.Setup(x => x.GetById(1)).Returns(_user);
    }

    [Given("a book exists with (.*) available copies")]
    public void GivenABookExistsWithAvailableCopies(int availableCopies)
    {
        SetupBookWithAvailableCopies(availableCopies);
    }

    [Given("the user has (.*) borrowed the book")]
    public void GivenTheUserHasBorrowedTheBook(string loanStatus)
    {
        if (loanStatus == "already")
        {
            _loanRepositoryMock.Setup(x => x.GetLoan(1, 1)).Returns(new Loan
            {
                UserId = 1,
                BookId = 1
            });
        }
        else
        {
            _loanRepositoryMock.Setup(x => x.GetLoan(1, 1)).Returns((Loan?)null);
        }
    }

    [Given("the user has (.*) reserved the book")]
    public void GivenTheUserHasReservedTheBook(string reservationStatus)
    {
        if (reservationStatus == "already")
        {
            _reservationRepositoryMock.Setup(x => x.GetReservationsForBook(1)).Returns(new List<Reservation>
            {
                new Reservation
                {
                    UserId = 1,
                    BookId = 1,
                    ReservedAt = DateTime.Now
                }
            });
        }
        else
        {
            _reservationRepositoryMock.Setup(x => x.GetReservation(1,1)).Returns((Reservation?) null);
        }
    }

    [When("the user attempts to reserve the book")]
    public void WhenTheUserAttemptsToReserveTheBook()
    {
        var service = CreateService();
        _result = service.ReserveBook(1, 1);
    }

    [Then("the reservation should be succesful")]
    public void ThenTheReservationShouldBeSuccesful()
    {
        _result.Should().Be(ReserveResult.Success);
    }

    [Then("the user should be added to the reservation queue for the book")]
    public void ThenTheUserShouldBeAddedToTheReservationQueue()
    {
        _reservationRepositoryMock.Verify(x => 
            x.Add(It.Is<Reservation>(r => r.UserId == 1 && r.BookId == 1)), Times.Once);
    }

    [Then("the result should be (.*)")]
    public void ThenTheResultShouldBe(string expectedResult)
    {
        var expected = Enum.Parse<ReserveResult>(expectedResult);
        _result.Should().Be(expected);
    }
}