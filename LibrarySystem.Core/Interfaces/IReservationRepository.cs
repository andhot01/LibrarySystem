namespace LibrarySystem.Core.Interfaces;
using LibrarySystem.Core.Models;

public interface IReservationRepository
{
    List<Reservation> GetReservationsForBook(int bookId);
    Reservation? GetReservation(int userId, int bookId);
    void Add(Reservation reservation);
    void Remove(Reservation reservation);
}