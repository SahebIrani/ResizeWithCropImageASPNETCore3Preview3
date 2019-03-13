using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

using System.IO;

namespace ImageServies.Extensions
{
	public static class Extends02
	{
		public static void ResizeAndSaveImage(Stream stream, string filename)
		{
			using (Image<Rgba32> image = Image.Load(stream))
			{
				image.Mutate(x => x.Resize(image.Width / 2, image.Height / 2));
				image.Save(filename);
			}
		}
	}
}
