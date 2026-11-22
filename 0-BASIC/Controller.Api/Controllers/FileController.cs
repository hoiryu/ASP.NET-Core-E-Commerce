using Microsoft.AspNetCore.Mvc;

namespace Controller.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class FileController : ControllerBase
	{
		[HttpGet("download")]
		public IActionResult GetDownload()
		{
			return File("pdfs/sample.pdf", "application/pdf", "test.pdf");
		}
	}
}
