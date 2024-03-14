using SkiaSharp;
namespace BulkDallE;
public class SimpleResizer
{
    public static byte[] ResizeToJpg(byte[] bytes, int width, int height)
        => Resize(bytes, SKEncodedImageFormat.Jpeg, width, height);
    public static byte[] ResizeToPng(byte[] bytes, int width, int height)
        => Resize(bytes, SKEncodedImageFormat.Png, width, height);
    private static byte[] Resize(byte[] bytes, SKEncodedImageFormat output, int width, int height, int quality = 100)
    {
        var sourceBitmap = SKBitmap.Decode(bytes);
        var sourceRect = new SKRect(0, 0, sourceBitmap.Width, sourceBitmap.Height);
        var targetBitmap = new SKBitmap(width, height);
        var targetRect = new SKRect(0, 0, targetBitmap.Width, targetBitmap.Height);
        var resizePaint = new SKPaint()
        {
            IsAntialias = true,
            FilterQuality = SKFilterQuality.High
        };
        using (SKCanvas canvas = new(targetBitmap))
        {
            canvas.DrawBitmap(sourceBitmap, sourceRect, targetRect, resizePaint);
        }
        return targetBitmap.Encode(output, quality).ToArray();
    }
}