
using Docnet.Core;
using Docnet.Core.Models;
using Docnet.Core.Readers;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace PdfToImage
{
    public static class ExampleOne
    {
        internal static List<byte[]> ConvertPDFToImage(string path)
        {
            using (var library = DocLib.Instance)
            {
                using (var docReader = library.GetDocReader(path, new PageDimensions(780, 1080)))
                {
                    List<byte[]> bytesList = new List<byte[]>();
                    for (int i = 0; i < docReader.GetPageCount(); i++)
                    {
                        using (var pageReader = docReader.GetPageReader(i))
                        {
                            var bytes = GetModifiedImage(pageReader);
                            bytesList.Add(bytes);
                        }
                    }
                    return bytesList;
                }
            }
        }

        internal static byte[] ConvertPDFToImageByPage(string path,int pageIndex)
        {
            using (var library = DocLib.Instance)
            {
                using (var docReader = library.GetDocReader(path, new PageDimensions(780, 1080)))
                {
                    if (pageIndex <= docReader.GetPageCount())
                    {
                        using (var pageReader = docReader.GetPageReader(pageIndex))
                        {
                            var bytes = GetModifiedImage(pageReader);
                            return bytes;
                        }
                    }
                    return new byte[0];
                }
            }
        }

        private static byte[] GetModifiedImage(IPageReader pageReader)
        {
            var rawBytes = pageReader.GetImage();

            var width = pageReader.GetPageWidth();
            var height = pageReader.GetPageHeight();

            var characters = pageReader.GetCharacters();

            using (var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb))
            {
                bmp.AddBytes(rawBytes);

                //bmp.DrawRectangles(characters);

                using (var stream = new MemoryStream())
                {
                    bmp.Save(stream, ImageFormat.Png);

                    return stream.ToArray();
                }
            }
        }

        private static void AddBytes(this Bitmap bmp, byte[] rawBytes)
        {
            var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);

            var bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, bmp.PixelFormat);
            var pNative = bmpData.Scan0;

            Marshal.Copy(rawBytes, 0, pNative, rawBytes.Length);
            bmp.UnlockBits(bmpData);
        }

        /// <summary>
        /// 文字加标注
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="characters"></param>
        private static void DrawRectangles(this Bitmap bmp, IEnumerable<Character> characters)
        {
            var pen = new Pen(Color.Red);

            using (var graphics = Graphics.FromImage(bmp))
            {
                foreach (var c in characters)
                {
                    var rect = new Rectangle(c.Box.Left, c.Box.Top, c.Box.Right - c.Box.Left, c.Box.Bottom - c.Box.Top);
                    graphics.DrawRectangle(pen, rect);
                }
            }
        }
    }
}