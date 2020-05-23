using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace IconChanger
{
    public class ImageModifier
    {
        public readonly Size[] RenderSizes = { new Size(256, 256), new Size(150, 150), new Size(128, 128), new Size(64, 64), new Size(48, 48), new Size(32, 32), new Size(16, 16) };

        public Image ResizeImage(Image Input, Size Size)
        {
            if (Input.Width > Size.Width || Input.Height > Size.Height)
            {
                Bitmap NewImage = new Bitmap(Size.Width, Size.Height);
                Graphics G = Graphics.FromImage(NewImage);
                G.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                G.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                G.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                G.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                G.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                G.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                G.DrawImage(Input, new Rectangle(0, 0, Size.Width, Size.Height));
                G.Save();
                return NewImage;
            }
            else
            {
                return Input;
            }
        }

        public Image ConvertToPNG(Image In)
        {
            Console.WriteLine("Converting Image");
            using (MemoryStream MS = new MemoryStream())
            {
                In.Save(MS, ImageFormat.Png);
                Image Ret = Image.FromStream(MS);
                return Ret;
            }
        }

        public void SaveIcon(Image In, string Out)
        {
            if (!(In.RawFormat.Guid == ImageFormat.Png.Guid))
            {
                In = ConvertToPNG(In);
            }
            List<Bitmap> Images = new List<Bitmap>();
            foreach (Size RenderSize in RenderSizes)
            {
                Console.WriteLine($"Rendering at {RenderSize.ToString()}");

                Bitmap Res = (Bitmap)ResizeImage(In, RenderSize);
                Console.WriteLine($"Resized Format: {Res.RawFormat}");
                if (Res.RawFormat.Guid != ImageFormat.Png.Guid)
                {
                    Res = (Bitmap)ConvertToPNG(Res);
                }
                Images.Add(Res);
            }
            IconFactory factory = new IconFactory();
            if (File.Exists(Out))
            {
                File.SetAttributes(Out, FileAttributes.Normal);
                File.Delete(Out);
            }

            using (FileStream FS = new FileStream(Out, FileMode.CreateNew, FileAccess.Write))
            {
                IconFactory.SavePngsAsIcon(Images, FS);
                FS.Close();
            }
        }
    }
}