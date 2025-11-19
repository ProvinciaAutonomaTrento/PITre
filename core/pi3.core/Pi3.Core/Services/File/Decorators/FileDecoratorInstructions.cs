// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Core.Services.File.Decorators
{
    public enum LayerRotationsEnum
    {
        None,
        Degrees90,
        Degrees180,
        Degrees270
    }

    public enum LayerDefaultPositionsEnum
    {
        TopLeft,
        TopCenter,
        TopRight,
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        BottomLeft,
        BottomCenter,
        BottomRight,
    }

    public enum TextLayerFontStylesEnum
    {
        Regular,
        Bold,
        Italic,
        Underline,
        Strikeout,
    }

    public class TextLayerFontRgbColor
    {
        public int R { get; set; }

        public int G { get; set; }

        public int B { get; set; }
    }

    public interface ILayer
    {
        LayerRotationsEnum Rotation { get; set; }

        LayerDefaultPositionsEnum? Position { get; set; }

        Point? CustomPosition { get; set; }

        int[] PageNumbersToApplyLayer { get; set; }
    }

    public abstract class Layer : ILayer
    {
        public LayerRotationsEnum Rotation { get; set; }

        public Point? CustomPosition { get; set; }

        public LayerDefaultPositionsEnum? Position { get; set; }

        public int[]? PageNumbersToApplyLayer { get; set; }
    }
    
    public class TextLayer : Layer
    {
        public string? FontName { get; set; } = "Arial";

        public string Text { get; set; }

        public TextLayerFontStylesEnum? FontStyle { get; set; } = TextLayerFontStylesEnum.Regular;

        public TextLayerFontRgbColor? FontForeColor { get; set; } = new TextLayerFontRgbColor { R = 0, G = 0, B = 0 };

        public float? FontSize { get; set; } = 14.0F;
    }

    public class FileDecoratorInstructions
    {
        public FileDecoratorInstructions(params ILayer[] layers)
        {
            this.Layers = layers;
        }

        public ILayer[] Layers { get; set; }
    }

    public enum FileDecoratorOutputFormatsEnum
    {
        ToPdf,
        ToImage
    }
}
