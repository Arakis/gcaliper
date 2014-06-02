using System;
using System.Collections.Generic;
using Gtk;
using Gdk;
using Cairo;
using System.Drawing;
using POINT = System.Drawing.Point;

namespace gcaliper
{
	public static class funcs
	{
		public static double GetAngleOfLineBetweenTwoPoints (PointF p1, PointF p2)
		{
			double xDiff = p2.X - p1.X;
			double yDiff = p2.Y - p1.Y;
			return Math.Atan2 (yDiff, xDiff); 
		}

		public static void showMessage (string txt)
		{
			new MessageDialog (null, DialogFlags.Modal, MessageType.Other, ButtonsType.Ok, txt).Show ();
		}

		public static PointF rotatePoint (PointF p, PointF center, double angle)
		{
			var x = Math.Cos (angle) * (p.X - center.X) - Math.Sin (angle) * (p.Y - center.Y) + center.X;
			var y = Math.Sin (angle) * (p.X - center.X) + Math.Cos (angle) * (p.Y - center.Y) + center.Y;
			return new PointF ((float)x, (float)y);
		}

		public static System.Drawing.Point rotatePoint (System.Drawing.Point p, System.Drawing.Point center, double angle)
		{
			var x = Math.Cos (angle) * (p.X - center.X) - Math.Sin (angle) * (p.Y - center.Y) + center.X;
			var y = Math.Sin (angle) * (p.X - center.X) + Math.Cos (angle) * (p.Y - center.Y) + center.Y;
			return new System.Drawing.Point ((int)Math.Round (x), (int)Math.Round (y));
		}

		public static System.Drawing.Rectangle rotateRect (System.Drawing.Rectangle r, POINT center, double angle)
		{
			var p1 = new POINT (r.Left, r.Top);
			var p2 = new POINT (r.Right, r.Top);
			var p3 = new POINT (r.Right, r.Bottom);
			var p4 = new POINT (r.Left, r.Bottom);

			var r1 = rotatePoint (p1, center, angle);
			var r2 = rotatePoint (p2, center, angle);
			var r3 = rotatePoint (p3, center, angle);
			var r4 = rotatePoint (p4, center, angle);

			var left = Math.Min (r1.X, Math.Min (r2.X, Math.Min (r3.X, r4.X)));
			var right = Math.Max (r1.X, Math.Max (r2.X, Math.Max (r3.X, r4.X)));

			var top = Math.Min (r1.Y, Math.Min (r2.Y, Math.Min (r3.Y, r4.Y)));
			var bottom = Math.Max (r1.Y, Math.Max (r2.Y, Math.Max (r3.Y, r4.Y)));

			return new System.Drawing.Rectangle (left, top, right - left, bottom - top);
		}
	}

	public static class Extensions
	{
		public static TColor getPixel (this Pixbuf buf, int x, int y)
		{
			if (buf.NChannels == 4)
				return TColor.fromArgbPointer (buf.Pixels + y * buf.Rowstride + x * buf.NChannels);
			if (buf.NChannels == 3)
				return TColor.fromRgbPointer (buf.Pixels + y * buf.Rowstride + x * buf.NChannels);

			throw new NotSupportedException ();
		}

		public unsafe static uint getPixelUInt (this Pixbuf buf, int x, int y)
		{
			if (buf.NChannels == 4)
				return *((uint*)(buf.Pixels + y * buf.Rowstride + x * buf.NChannels));

			throw new NotSupportedException ();
		}

		public unsafe static uint getPixelUInt (this ImageSurface buf, int x, int y)
		{
			var color = getBgraPixelUInt (buf, x, y);
			/*
			byte a = (byte)(color >> 24);
			byte r = (byte)(color >> 16);
			byte g = (byte)(color >> 8);
			byte b = (byte)(color >> 0);
*/

			byte a = (byte)(color >> 24);
			byte r = (byte)(color >> 16);
			byte g = (byte)(color >> 8);
			byte b = (byte)(color >> 0);

			//var gg = (a << 16);

			return (uint)((a << 24) | (b << 16) | (g << 8) | (r << 0));

		}

		public unsafe static uint getBgraPixelUInt (this ImageSurface buf, int x, int y)
		{
			if (buf.Format == Format.Argb32)
				return *((uint*)(buf.DataPtr + y * buf.Stride + x * 4));
			if (buf.Format == Format.Rgb24)
				return *((uint*)(buf.DataPtr + y * buf.Stride + x * 3));
			throw new NotSupportedException ();
		}

		public unsafe static TColor getPixel (this ImageSurface buf, int x, int y)
		{

			if (buf.Format == Format.Argb32) {
				var ptr = buf.DataPtr + y * buf.Stride + x * 4;


				return TColor.fromArgbPointer2 (ptr);
			}

			throw new NotSupportedException ();
		}

		public unsafe static void setPixel (this Pixbuf buf, int x, int y, TColor color)
		{
			byte* pix = (byte*)(buf.Pixels + y * buf.Rowstride + x * buf.NChannels);
			*(pix) = color.r;
			*(pix + 1) = color.g;
			*(pix + 2) = color.b;
			if (buf.NChannels == 4)
				*(pix + 3) = color.a;
		}

		public unsafe static void setPixel (this Pixbuf buf, int x, int y, uint argbcolor)
		{
			uint* pix = (uint*)(buf.Pixels + y * buf.Rowstride + x * buf.NChannels);
			*(pix) = argbcolor;
		}
	}

	public struct TColor
	{
		public byte r;
		public byte g;
		public byte b;
		public byte a;

		public TColor (byte r, byte g, byte b)
		{
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = 0;
		}

		public TColor (byte r, byte g, byte b, byte a)
		{
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = a;
		}

		public unsafe static TColor fromArgbPointer (IntPtr ptr)
		{
			var c = new TColor ();
			byte* pix = (byte*)ptr;
			c.r = *pix;
			c.g = *(pix + 1);
			c.b = *(pix + 2);
			c.a = *(pix + 3);
			return c;
		}

		public unsafe static TColor fromArgbPointer2 (IntPtr ptr)
		{
			var c = new TColor ();
			byte* pix = (byte*)ptr;
			c.r = *(pix + 2);
			c.g = *(pix + 1);
			c.b = *(pix + 0);
			c.a = *(pix + 3);
			return c;
		}

		public unsafe static TColor fromRgbPointer (IntPtr ptr)
		{
			var c = new TColor ();
			byte* pix = (byte*)ptr;
			c.r = *pix;
			c.g = *(pix + 1);
			c.b = *(pix + 2);
			return c;
		}
	}

	public class TPartList: List<TPart>
	{
		public System.Drawing.Rectangle getRotationRect ()
		{
			var rect = new System.Drawing.Rectangle ();
			foreach (var p in this) {
				if (p.rotate) {
					rect = System.Drawing.Rectangle.Union (rect, p.rect);
				}
			}
			return rect;
		}
	}

	public class TPart
	{
		public ImageSurface image;
		public bool rotate;
		public PointD rotationCenter;
		public double rotationAngle;
		public System.Drawing.Rectangle rect;
		//public System.Drawing.Rectangle rotatedRect;
	}

	public class TImagePart : TPart
	{
		public TImagePart (string file)
		{
			image = new ImageSurface (file);
			rect = new System.Drawing.Rectangle (0, 0, image.Width, image.Height);
		}
	}

	public class TCaliperPart1 : TImagePart
	{
		public TCaliperPart1 () : base ("../../template/caliper/1.png")
		{
			rotate = true;
		}
	}

	public class TCaliperPart2 : TImagePart
	{
		public TCaliperPart2 () : base ("../../template/caliper/2.png")
		{
			rotate = true;
		}
	}
}


