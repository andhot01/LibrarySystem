namespace LibrarySystem.Core.Models;

public class Reservation
{
    public int UserId { get; set; }
    public int BookId { get; set; }
    public DateTime ReservedAt { get; set; }
}