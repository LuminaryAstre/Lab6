using System;
using Microsoft.Xna.Framework;

namespace Lab6.UI;

public enum NineSliceSegmentType
{
    Edge = 1,
    Corner = 2,
    Center = 4
}

public enum NineSliceAlignment
{
    TopLeft = 0,
    TopCenter = 1,
    TopRight = 2,
    CenterLeft = 3,
    CenterCenter = 4,
    CenterRight = 5,
    BottomLeft = 6,
    BottomCenter = 7,
    BottomRight = 8
}
public enum NineSliceDirectionality
{
    Horizontal = 1,
    Vertical = 2,
    Both = 4,
    Neither = 8
}

public class NineSliceSegment
{
    public NineSliceAlignment Alignment;
    public NineSliceSegmentType Type;
    public NineSliceDirectionality Directionality;

    public bool Horizontal => Directionality == NineSliceDirectionality.Both ||
                              Directionality == NineSliceDirectionality.Horizontal;

    public bool Vertical => Directionality == NineSliceDirectionality.Both ||
                            Directionality == NineSliceDirectionality.Vertical;

    public bool IsStartOfLine => Alignment == NineSliceAlignment.BottomLeft ||
                                 Alignment == NineSliceAlignment.CenterLeft || 
                                 Alignment == NineSliceAlignment.TopLeft;
    
    public Point Slice(Point fullSize, Point sliceDims) 
    {
        Point ret = Point.Zero;
        ret.X = Horizontal ? fullSize.X - sliceDims.X * 2 : sliceDims.X;
        ret.Y = Vertical ? fullSize.Y - sliceDims.Y * 2 : sliceDims.Y;
        return ret;
    }

    public NineSliceSegment(int seg)
    {
        if (!Enum.TryParse(seg.ToString(), out Alignment))
        {
            throw new ArgumentException($"{seg} is not parsable as a nineslice alignment.");
        }

        if (Alignment == NineSliceAlignment.TopLeft || Alignment == NineSliceAlignment.TopRight ||
            Alignment == NineSliceAlignment.BottomLeft || Alignment == NineSliceAlignment.BottomRight)
        {
            Type = NineSliceSegmentType.Corner;
            Directionality = NineSliceDirectionality.Neither;
        }
        else if (Alignment == NineSliceAlignment.CenterCenter)
        {
            Type = NineSliceSegmentType.Center;
            Directionality = NineSliceDirectionality.Both;
        }
        else
        {
            Type = NineSliceSegmentType.Edge;
            if (Alignment == NineSliceAlignment.TopCenter || Alignment == NineSliceAlignment.BottomCenter)
                Directionality = NineSliceDirectionality.Horizontal;
            else
                Directionality = NineSliceDirectionality.Vertical;
        }
    }
    
}
