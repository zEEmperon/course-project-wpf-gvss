using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace gvss_project
{
    public static class RenderVisualService
    {
        private const double defaultDpi = 96.0;
        public static void RenderToPNGFile(Visual targetControl, string filename)
        {
            var renderTargetBitmap = GetRenderTargetBitmapFromControl(targetControl);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

            var result = new BitmapImage();

            try
            {
                using (var fileStream = new FileStream(filename, FileMode.Create))
                {
                    encoder.Save(fileStream);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"There was an error saving the file: {ex.Message}");
            }
        }

        private static BitmapSource GetRenderTargetBitmapFromControl(Visual targetControl, double dpi = defaultDpi)
        {
            if (targetControl == null) return null;

            var bounds = VisualTreeHelper.GetDescendantBounds(targetControl);
            var renderTargetBitmap = new RenderTargetBitmap((int)(bounds.Width * dpi / 96.0),
                                                            (int)(bounds.Height * dpi / 96.0),
                                                            dpi,
                                                            dpi,
                                                            PixelFormats.Pbgra32);

            var drawingVisual = new DrawingVisual();

            using (var drawingContext = drawingVisual.RenderOpen())
            {
                var visualBrush = new VisualBrush(targetControl);

                if (targetControl.GetType() == typeof(Canvas))
                {
                    drawingContext.DrawRectangle(Brushes.White, null, new Rect(new Point(), bounds.Size));
                }

                drawingContext.DrawRectangle(visualBrush, null, new Rect(new Point(), bounds.Size));
            }

            renderTargetBitmap.Render(drawingVisual);
            return renderTargetBitmap;
        }
    }
}
