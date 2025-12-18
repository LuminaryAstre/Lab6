using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;

namespace Lab6.UI;

public static class NineSliceDrawer
{

    public static List<Rectangle> Layout9Slice(float borderThicknessMultiplier, Vector2 sliceDims, Vector2 fullSize)
    {
        return Layout9Slice(borderThicknessMultiplier, sliceDims.ToPoint(), fullSize.ToPoint());
    }
    
    public static List<Rectangle> Layout9Slice(float borderThicknessMultiplier, Point sliceDims, Point fullSize)
    {
        Rectangle last = Rectangle.Empty;
        List<Rectangle> ret = [];

        for (int i = 0; i < 9; i++)
        {
            var seg = new NineSliceSegment(i);
            Vector2 mult = Vector2.One;
            if (!seg.Horizontal)
                mult.X += borderThicknessMultiplier;
            if (!seg.Vertical)
                mult.Y += borderThicknessMultiplier;
            var rect = new Rectangle(Point.Zero, seg.Slice(fullSize, sliceDims) * mult.ToPoint());
            if (seg.IsStartOfLine)
                rect.Y += last.Location.Y + last.Height;
            else
            {
                rect.Location += last.Location;
                rect.X += last.Width;
            }
            
            last = rect;
            ret.Add(rect);
        }
        
        return ret;
    }
    
    public static void Draw9Slice(this SpriteBatch sprite,
        Texture2D texture,
        Vector2 pos,
        Vector2 dims,
        Color? color = null,
        float borderScale = 1f,
        int ninesliceBorderWidth = 2)
    {
        var sliceDims = new Point(ninesliceBorderWidth,
            ninesliceBorderWidth);
        var txtDims = texture.Bounds.Size;
        var slices = Layout9Slice(borderScale, sliceDims, dims.ToPoint());

        var centerWidth = txtDims.X - (2 * ninesliceBorderWidth);
        var centerHeight = txtDims.Y - (2 * ninesliceBorderWidth);
        var centerDims = new Point(centerWidth, centerHeight);

        var rightX = txtDims.X - ninesliceBorderWidth;
        var bottomY = txtDims.Y - ninesliceBorderWidth;

        Texture2DRegion[] segments = [
            new(texture, new Rectangle(Point.Zero, sliceDims)),
            new(texture, new Rectangle(
                new Point(sliceDims.X, 0),
                new Point(centerWidth, sliceDims.Y)
            )),
            new(texture, new Rectangle(
                new Point(rightX, 0),
                sliceDims
            )),
            new(texture, new Rectangle(
                new Point(0, sliceDims.Y),
                new Point(sliceDims.X, centerHeight)
            )),
            new(texture, new Rectangle(
                new Point(sliceDims.X, sliceDims.Y),
                centerDims
            )),
            new(texture, new Rectangle(
                new Point(rightX, sliceDims.Y),
                new Point(sliceDims.X, centerHeight)
            )),
            new(texture, new Rectangle(
                new Point(0, bottomY),
                sliceDims
            )),
            new(texture, new Rectangle(
                new Point(sliceDims.X, bottomY),
                new Point(centerWidth, sliceDims.Y)
            )),
            new(texture, new Rectangle(
                new Point(rightX, bottomY),
                sliceDims
            ))
        ];

        for (int i = 0; i < 9; i++)
        {
            var tex = segments[i];
            var seg = slices[i];
            seg.Location += pos.ToPoint();
            sprite.Draw(tex, seg, color ?? Color.White);
        }


    }
}