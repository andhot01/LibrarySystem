namespace LibrarySystem.Core.Interfaces;
using LibrarySystem.Core.Models;

public interface IUserRepository
{
    LibraryUser? GetById(int userId);
}