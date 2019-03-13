using ImageServies.Extensions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ResizeImageASPNETCore.Controllers
{
	public class Sample01Controller : Controller
	{
		[HttpGet] public IActionResult Index() => View();

		[HttpPost]
		public FileStreamResult Index(IList<IFormFile> files)
		{
			using (Image img = Image.FromStream(files[0].OpenReadStream()))
			{
				Stream ms = new MemoryStream(img.Resize(220, 220).imageToByteArray2(ImageFormat.Jpeg));

				return new FileStreamResult(ms, "image/jpg");
			}
		}
	}
}
