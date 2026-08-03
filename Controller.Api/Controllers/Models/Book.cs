namespace Controller.Api.Controllers.Models;

public class Book
{
	public int? BookId { get; set; }

	public string? Author { get; set; }

	public override string ToString()
	{
		return $"Book Id: {BookId}, Author: {Author}";
	}
}
