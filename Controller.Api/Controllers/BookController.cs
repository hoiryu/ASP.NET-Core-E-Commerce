using Controller.Api.Controllers.Models;
using Microsoft.AspNetCore.Mvc;

namespace Controller.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BookController : ControllerBase
	{
		[HttpGet("test1/{bookId:int}/{author}")]
		public ActionResult<Book> GetBook([FromRoute] Book book)
		{
			return book;
		}

		[HttpGet("test2")]
		public ActionResult<Book> GetBook2([FromForm] Book book)
		{
			return book;
		}
	}
}
