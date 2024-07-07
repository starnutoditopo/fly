using Mapsui.Rendering;
using Mapsui.Rendering.Skia.Extensions;
using Mapsui.Styles;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Brush = Mapsui.Styles.Brush;
using Color = Mapsui.Styles.Color;
using Font = Mapsui.Styles.Font;
namespace Fly.StyleRenderers;

public enum LineBreakMode : short
{
    /// <summary>
    /// Do not wrap text
    /// </summary>
    NoWrap,
    /// <summary>
    /// Wrap at character boundaries
    /// </summary>
    CharacterWrap,
    /// <summary>
    /// Truncate the head of text
    /// </summary>
    HeadTruncation,
    /// <summary>
    /// Truncate the middle of text. This may be done, for example, by replacing it with an ellipsis
    /// </summary>
    MiddleTruncation,
    /// <summary>
    /// Truncate the tail of text
    /// </summary>
    TailTruncation,
    /// <summary>
    /// Wrap at word boundaries
    /// </summary>
    WordWrap
}

/// <summary>
/// Label text alignment
/// </summary>
public enum HorizontalAlignmentEnum : short
{
    /// <summary>
    /// Left oriented
    /// </summary>
    Left = 0,
    /// <summary>
    /// Right oriented
    /// </summary>
    Right = 2,
    /// <summary>
    /// Centered
    /// </summary>
    Center = 1
}

/// <summary>
/// Label text alignment
/// </summary>
public enum VerticalAlignmentEnum : short
{
    /// <summary>
    /// Left oriented
    /// </summary>
    Bottom = 2,
    /// <summary>
    /// Right oriented
    /// </summary>
    Top = 0,
    /// <summary>
    /// Centered
    /// </summary>
    Center = 1
}

public interface IFlyLabelStyle : IStyle
{
    /// <summary>
    /// Label Font
    /// </summary>
    Font Font { get; }

    /// <summary>
    /// Font color
    /// </summary>
    Color ForeColor { get; }

    /// <summary>
    /// Maximum width of text in em. If text is wider than this, text is shorten or 
    /// word wrapped regarding WordWrap.
    /// </summary>
    double MaxWidth { get; }

    /// <summary>
    /// Line break mode for text, if width is bigger than MaxWidth
    /// </summary>
    LineBreakMode WordWrap { get; }
    /// <summary>
    /// Space from one text line to next text line in em
    /// </summary>
    double LineHeight { get; }

    /// <summary>
    /// The horizontal alignment of the text in relation to the label point
    /// </summary>
    HorizontalAlignmentEnum HorizontalAlignment { get; }

    /// <summary>
    /// The horizontal alignment of the text in relation to the label point
    /// </summary>
    VerticalAlignmentEnum VerticalAlignment { get; }

    /// <summary>
    /// The background color of the label. Set to transparent brush or null if background isn't needed
    /// </summary>
    Brush? BackColor { get; } // todo: rename

    /// <summary>
    /// Specifies relative position of labels with respect to objects label point
    /// </summary>
    Offset Offset { get; }

    /// <summary>
    /// The color of the border around the background.
    /// </summary>
    Color BorderColor { get; }

    /// <summary>
    /// The thickness of the border around the background.
    /// </summary>
    double BorderThickness { get; }

    /// <summary>
    /// The radius of the oval used to round the corners of the background. See <see cref="SkiaSharp.SkCanvas.DrawRoundRect"/>.
    /// </summary>
    int CornerRounding { get; }

    /// <summary>
    /// Creates a halo around the text
    /// </summary>
    Pen? Halo { get; }

}

public static class LabelDrawingHelper
{
    public static void DrawLabel(SKCanvas target, float x, float y, IFlyLabelStyle style, string? text, float layerOpacity, ILabelCache labelCache, float mustFitIn, SKPaint _paint)
    {
        UpdatePaint(style, layerOpacity, _paint, labelCache);

        var rect = new SKRect();

        Line[]? lines = null;

        float emHeight = 0;
        float maxWidth = 0;
        var hasNewline = text?.Contains('\n') ?? false; // There could be a multi line text by newline

        // Get default values for unit em
        if (style.MaxWidth > 0 || hasNewline)
        {
            _paint.MeasureText("M", ref rect);
            emHeight = _paint.FontSpacing;
            maxWidth = (float)style.MaxWidth * rect.Width;
        }

        _paint.MeasureText(text, ref rect);

        var baseline = -rect.Top;  // Distance from top to baseline of text

        var drawRect = new SKRect(0, 0, rect.Right - rect.Left, rect.Bottom - rect.Top);

        var diagonalLength = Math.Sqrt(rect.Width * rect.Width + rect.Height * rect.Height);
        if (diagonalLength > Math.Abs(mustFitIn))
        {
            return;
        }

        if ((style.MaxWidth > 0 && drawRect.Width > maxWidth) || hasNewline)
        {
            // Text has a line feed or should be shorten by character wrap
            if (hasNewline || style.WordWrap == LineBreakMode.CharacterWrap)
            {
                lines = SplitLines(text, _paint, hasNewline ? drawRect.Width : maxWidth, string.Empty);
                var width = 0f;
                for (var i = 0; i < lines.Length; i++)
                {
                    lines[i].Baseline = baseline + (float)(style.LineHeight * emHeight * i);
                    width = Math.Max(lines[i].Width, width);
                }

                drawRect = new SKRect(0, 0, width, (float)(drawRect.Height + style.LineHeight * emHeight * (lines.Length - 1)));
            }

            // Text is to long, so wrap it by words
            if (style.WordWrap == LineBreakMode.WordWrap)
            {
                lines = SplitLines(text, _paint, maxWidth, " ");
                var width = 0f;
                for (var i = 0; i < lines.Length; i++)
                {
                    lines[i].Baseline = baseline + (float)(style.LineHeight * emHeight * i);
                    width = Math.Max(lines[i].Width, width);
                }

                drawRect = new SKRect(0, 0, width, (float)(drawRect.Height + style.LineHeight * emHeight * (lines.Length - 1)));
            }

            // Shorten it at beginning
            if (style.WordWrap == LineBreakMode.HeadTruncation)
            {
                var result = text?[(text.Length - (int)style.MaxWidth - 2)..];
                while (result?.Length > 1 && _paint.MeasureText("..." + result) > maxWidth)
                    result = result[1..];
                text = "..." + result;
                _paint.MeasureText(text, ref rect);
                drawRect = new SKRect(0, 0, rect.Right - rect.Left, rect.Bottom - rect.Top);
            }

            // Shorten it at end
            if (style.WordWrap == LineBreakMode.TailTruncation)
            {
                var result = text?[..((int)style.MaxWidth + 2)];
                while (result?.Length > 1 && _paint.MeasureText(result + "...") > maxWidth)
                    result = result[..^1];
                text = result + "...";
                _paint.MeasureText(text, ref rect);
                drawRect = new SKRect(0, 0, rect.Right - rect.Left, rect.Bottom - rect.Top);
            }

            // Shorten it in the middle
            if (style.WordWrap == LineBreakMode.MiddleTruncation)
            {
                var result1 = text?[..((int)(style.MaxWidth / 2) + 1)];
                var result2 = text?[(text.Length - (int)(style.MaxWidth / 2) - 1)..];
                while (result1?.Length > 1 && result2?.Length > 1 &&
                       _paint.MeasureText(result1 + "..." + result2) > maxWidth)
                {
                    result1 = result1[..^1];
                    result2 = result2[1..];
                }

                text = result1 + "..." + result2;
                _paint.MeasureText(text, ref rect);
                drawRect = new SKRect(0, 0, rect.Right - rect.Left, rect.Bottom - rect.Top);
            }
        }

        var horizontalAlign = CalcHorizontalAlignment(style.HorizontalAlignment);
        var verticalAlign = CalcVerticalAlignment(style.VerticalAlignment);

        var offsetX = style.Offset is RelativeOffset ? drawRect.Width * style.Offset.X : style.Offset.X;
        var offsetY = style.Offset is RelativeOffset ? drawRect.Height * style.Offset.Y : style.Offset.Y;

        drawRect.Offset(
            x - drawRect.Width * horizontalAlign + (float)offsetX,
            y - drawRect.Height * verticalAlign + (float)offsetY);

        // If style has a background color, than draw background rectangle
        if (style.BackColor != null)
        {
            var backRect = drawRect;
            backRect.Inflate(3, 3);
            DrawBackground(style, backRect, target, layerOpacity);
        }

        // If style has a halo value, than draw halo text
        if (style.Halo != null)
        {
            UpdatePaint(style, layerOpacity, _paint, labelCache);
            _paint.Style = SKPaintStyle.StrokeAndFill;
            _paint.Color = style.Halo.Color.ToSkia(layerOpacity);
            _paint.StrokeWidth = (float)style.Halo.Width * 2;

            if (lines != null)
            {
                var left = drawRect.Left;
                foreach (var line in lines)
                {
                    if (style.HorizontalAlignment == HorizontalAlignmentEnum.Center)
                        target.DrawText(line.Value, (float)(left + (drawRect.Width - line.Width) * 0.5), drawRect.Top + line.Baseline, _paint);
                    else if (style.HorizontalAlignment == HorizontalAlignmentEnum.Right)
                        target.DrawText(line.Value, left + drawRect.Width - line.Width, drawRect.Top + line.Baseline, _paint);
                    else
                        target.DrawText(line.Value, left, drawRect.Top + line.Baseline, _paint);
                }
            }
            else
                target.DrawText(text, drawRect.Left, drawRect.Top + baseline, _paint);
        }

        UpdatePaint(style, layerOpacity, _paint, labelCache);

        if (lines != null)
        {
            var left = drawRect.Left;
            foreach (var line in lines)
            {
                if (style.HorizontalAlignment == HorizontalAlignmentEnum.Center)
                    target.DrawText(line.Value, (float)(left + (drawRect.Width - line.Width) * 0.5), drawRect.Top + line.Baseline, _paint);
                else if (style.HorizontalAlignment == HorizontalAlignmentEnum.Right)
                    target.DrawText(line.Value, left + drawRect.Width - line.Width, drawRect.Top + line.Baseline, _paint);
                else
                    target.DrawText(line.Value, left, drawRect.Top + line.Baseline, _paint);
            }
        }
        else
            target.DrawText(text, drawRect.Left, drawRect.Top + baseline, _paint);
    }

    private static float CalcHorizontalAlignment(HorizontalAlignmentEnum horizontalAlignment)
    {
        if (horizontalAlignment == HorizontalAlignmentEnum.Center) return 0.5f;
        if (horizontalAlignment == HorizontalAlignmentEnum.Left) return 0f;
        if (horizontalAlignment == HorizontalAlignmentEnum.Right) return 1f;
        throw new ArgumentException($"Unknown {nameof(HorizontalAlignmentEnum)} type '{nameof(horizontalAlignment)}");
    }

    private static float CalcVerticalAlignment(VerticalAlignmentEnum verticalAlignment)
    {
        if (verticalAlignment == VerticalAlignmentEnum.Center) return 0.5f;
        if (verticalAlignment == VerticalAlignmentEnum.Top) return 0f;
        if (verticalAlignment == VerticalAlignmentEnum.Bottom) return 1f;
        throw new ArgumentException($"Unknown {nameof(VerticalAlignmentEnum)} type '{nameof(verticalAlignment)}");
    }

    private static void DrawBackground(IFlyLabelStyle style, SKRect rect, SKCanvas target, float layerOpacity)
    {
        var color = style.BackColor?.Color?.ToSkia(layerOpacity);
        if (color.HasValue)
        {
            var rounding = style.CornerRounding;
            using var backgroundPaint = new SKPaint { Color = color.Value, IsAntialias = true };
            target.DrawRoundRect(rect, rounding, rounding, backgroundPaint);
            if (style.BorderThickness > 0 &&
                style.BorderColor != Color.Transparent)
            {
                using SKPaint borderPaint = new SKPaint
                {
                    Color = style.BorderColor.ToSkia(),
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = (float)style.BorderThickness,
                    IsAntialias = true
                };
                target.DrawRoundRect(rect, rounding, rounding, borderPaint);
            }
        }
    }

    private static void UpdatePaint(IFlyLabelStyle style, float layerOpacity, SKPaint paint, ILabelCache labelCache)
    {
        var typeface = labelCache.GetOrCreateTypeface(style.Font, CreateTypeFace);

        paint.Style = SKPaintStyle.Fill;
        paint.TextSize = (float)style.Font.Size;
        paint.Color = style.ForeColor.ToSkia(layerOpacity);
        paint.Typeface = typeface;
    }


    private static Line[] SplitLines(string? text, SKPaint paint, float maxWidth, string splitCharacter)
    {
        if (text == null)
            return [];

        var spaceWidth = paint.MeasureText(" ");
        var lines = text.Split('\n');

        return lines.SelectMany(line =>
        {
            var result = new List<Line>();
            string[] words;

            if (splitCharacter == string.Empty)
            {
                words = line.ToCharArray().Select(x => x.ToString()).ToArray();
                spaceWidth = 0;
            }
            else
            {
                words = line.Split(new[] { splitCharacter }, StringSplitOptions.None);
            }

            var lineResult = new StringBuilder();
            float width = 0;
            foreach (var word in words)
            {
                var wordWidth = paint.MeasureText(word);
                var wordWithSpaceWidth = wordWidth + spaceWidth;
                var wordWithSpace = word + splitCharacter;

                if (width + wordWidth > maxWidth)
                {
                    result.Add(new Line { Value = lineResult.ToString(), Width = width });
                    lineResult = new StringBuilder(wordWithSpace);
                    width = wordWithSpaceWidth;
                }
                else
                {
                    lineResult.Append(wordWithSpace);
                    width += wordWithSpaceWidth;
                }
            }

            result.Add(new Line { Value = lineResult.ToString(), Width = width });

            return result.ToArray();
        }).ToArray();
    }



    public static SKTypeface CreateTypeFace(Font font)
    {
        return SKTypeface.FromFamilyName(font.FontFamily,
            font.Bold ? SKFontStyleWeight.Bold : SKFontStyleWeight.Normal,
            SKFontStyleWidth.Normal,
            font.Italic ? SKFontStyleSlant.Italic : SKFontStyleSlant.Upright);
    }

    private class Line
    {
        public string? Value { get; set; }
        public float Width { get; set; }
        public float Baseline { get; set; }
    }
}

