using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace ImageServies.Extensions
{
	public static class Extends01
	{
		public static Image Resize(this Image current, int maxWidth, int maxHeight)
		{
			int width, height;
			#region reckon size
			if (current.Width > current.Height)
			{
				width = maxWidth;
				height = Convert.ToInt32(current.Height * maxHeight / (double)current.Width);
			}
			else
			{
				width = Convert.ToInt32(current.Width * maxWidth / (double)current.Height);
				height = maxHeight;
			}
			#endregion

			#region get resized bitmap
			var canvas = new Bitmap(width, height);

			using (var graphics = Graphics.FromImage(canvas))
			{
				graphics.CompositingQuality = CompositingQuality.HighSpeed;
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.CompositingMode = CompositingMode.SourceCopy;
				graphics.DrawImage(current, 0, 0, width, height);
			}

			return canvas;
			#endregion
		}

		public static byte[] ToByteArray(this Image current)
		{
			using (var stream = new MemoryStream())
			{
				current.Save(stream, current.RawFormat);
				return stream.ToArray();
			}
		}

		public static byte[] imageToByteArray2(this System.Drawing.Image imageIn, ImageFormat imageFormat)
		{
			MemoryStream ms = new MemoryStream();
			imageIn.Save(ms, imageFormat);
			return ms.ToArray();
		}

		public static Image byteArrayToImage2(byte[] byteArrayIn)
		{
			MemoryStream ms = new MemoryStream(byteArrayIn);
			Image returnImage = Image.FromStream(ms);
			return returnImage;
		}
	}
}