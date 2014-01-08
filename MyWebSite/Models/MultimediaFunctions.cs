using System.Drawing;
using System.Drawing.Drawing2D;

namespace MyWebSite.Models
{
    public class MultimediaFunctions
    {

        public static Image ResizeOrigImg(Image image, double nWidth, double nHeight)
        {
            int newWidth, newHeight;
            var coefH = nHeight / image.Height;
            var coefW = nWidth / image.Width;
            if (coefW >= coefH)
            {
                newHeight = (int)(image.Height * coefH);
                newWidth = (int)(image.Width * coefH);
            }
            else
            {
                newHeight = (int)(image.Height * coefW);
                newWidth = (int)(image.Width * coefW);
            }

            Image result = new Bitmap(newWidth, newHeight);
            using (var g = Graphics.FromImage(result))
            {
                g.CompositingQuality = CompositingQuality.HighSpeed;
                g.SmoothingMode = SmoothingMode.HighSpeed;
                g.InterpolationMode = InterpolationMode.Low;

                g.DrawImage(image, 0, 0, newWidth, newHeight);
                g.Dispose();
            }
            return result;
        }

    }
}