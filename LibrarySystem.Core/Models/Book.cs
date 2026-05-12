namespace LibrarySystem.Core.Models;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int TotalCopies { get; set; }
    public int AvailableCopies { get; set; }
}