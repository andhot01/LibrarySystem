namespace LibrarySystem.Core.Results;

public enum ReserveResult
{
    Success,
    UserNotFound,
    BookNotFound,
    AlreadyBorrowed,
    AlreadyReserved,
    BookAvailableForBorrowing
}