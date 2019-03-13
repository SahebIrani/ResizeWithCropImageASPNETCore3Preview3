using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace AspNetCoreImageResizingService
{
	public class Sample03Controller : Controller
	{
		private readonly IFileProvider _fileProvider;
		public Sample03Controller(IWebHostEnvironment env) => _fileProvider = env.WebRootFileProvider;

		[Route("/image/{width}/{height}/{*url}")]
		public IActionResult ResizeImage(string url, int width, int height)
		{
			if (width < 0 || height < 0) { return BadRequest(); }

			var imagePath = PathString.FromUriComponent("/" + url);
			var fileInfo = _fileProvider.GetFileInfo(imagePath);
			if (!fileInfo.Exists) { return NotFound(); }

			var outputStream = new MemoryStream();
			using (var inputStream = fileInfo.CreateReadStream())
			using (Image<Rgba32> image = Image.Load(inputStream))
			{
				if (image.Width >= width && image.Height >= height) image.Mutate(c => c.Resize(width, height));
				image.Save(outputStream, new JpegEncoder());
			}

			outputStream.Seek(0, SeekOrigin.Begin);

			return File(outputStream, "image/jpg");
		}

		public async Task<IActionResult> Index(string url, int sourceX, int sourceY, int sourceWidth, int sourceHeight, int destinationWidth, int destinationHeight)
		{
			Image<Rgba32> sourceImage = await LoadImageFromUrl(url);

			if (sourceImage != null)
			{
				try
				{
					//sourceImage.Mutate(x => x.Crop(new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight)).Resize(destinationWidth, destinationHeight));
					//sourceImage.Mutate(x => x.Crop(new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight)));
					//sourceImage.Mutate(x => x.Resize(destinationWidth, destinationHeight));
					sourceImage.Mutate(x => x.Resize(new ResizeOptions
					{
						Mode = ResizeMode.BoxPad,
						Position = AnchorPositionMode.Center,
						Size = new Size(destinationWidth, destinationHeight)
					}));
					Stream outputStream = new MemoryStream();
					sourceImage.Save(outputStream, new JpegEncoder());
					outputStream.Seek(0, SeekOrigin.Begin);
					return this.File(outputStream, "image/jpg");
				}
				catch
				{
				}
			}
			return NotFound();
		}

		private async Task<Image<Rgba32>> LoadImageFromUrl(string url)
		{
			Image<Rgba32> image = null;
			try
			{
				HttpClient httpClient = new HttpClient();
				HttpResponseMessage response = await httpClient.GetAsync(url);
				Stream inputStream = await response.Content.ReadAsStreamAsync();
				image = Image.Load(inputStream);
			}
			catch
			{
			}
			return image;
		}
	}
}