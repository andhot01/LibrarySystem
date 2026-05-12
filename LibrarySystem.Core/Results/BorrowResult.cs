namespace LibrarySystem.Core.Results;

public enum BorrowResult
{
    Success,
    UserNotFound,
    BookNotFound,
    BookAlreadyBorrowed,
    ReservedForAnotherUser,
    NoAvailableCopies
}