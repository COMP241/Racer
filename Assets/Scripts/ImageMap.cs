using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class ImageMap
{
    public int Id;
    public float Ratio;
    public Line[] Lines;

    public static ImageMap FromJson(string json)
    {
        return JsonUtility.FromJson<ImageMap>(json);
    }

    public override string ToString()
    {
        return string.Format("MAP\nid:{0}\nratio:{1}\nLines:\n\t{2}", Id, Ratio, string.Join("\n\t", Lines.Select(l => l.ToString()).ToArray()));
    }
}

[Serializable]
public enum MapColor
{
    Black = 0,
    Red = 1,
    Green = 2,
    Blue = 3,
    Cyan = 4,
    Magenta = 5,
    Yellow = 6
}

public static class MapColorMethods
{
    public static Color GetColor(this MapColor color)
    {
        switch (color)
        {
            case MapColor.Black:
                return Color.black;
            case MapColor.Red:
                return Color.red;
            case MapColor.Green:
                return Color.green;
            case MapColor.Blue:
                return Color.blue;
            case MapColor.Cyan:
                return Color.cyan;
            case MapColor.Magenta:
                return Color.magenta;
            case MapColor.Yellow:
                return Color.yellow;
            default:
                throw new ArgumentOutOfRangeException("color", color, null);
        }
    }
}

[Serializable]
public class Line
{
    public MapColor Color;
    public bool Loop;
    public Point[] Points;

    public override string ToString()
    {
        return string.Format("{0} {1} [{2}]", Color, Loop ? "Looping" : "Open", string.Join(",", Points.Select(p => p.ToString()).ToArray()));
    }

    public Point AveragePoint()
    {
        Point result = new Point()
        {
            X = Points.Average(p => p.X),
            Y = Points.Average(p => p.Y)
        };
        return result;
    }

    public float AverageSqrDistanceFrom(Point p)
    {
        return Points.Average(pnt => (pnt.X - p.X) * (pnt.X - p.X) + (pnt.Y - p.Y) * (pnt.Y - p.Y));
    }
}

[Serializable]
public class Point
{
    public float X;
    public float Y;

    public override string ToString()
    {
        return string.Format("({0}, {1})", X, Y);
    }
    public Point(float x, float y)
    {
        X = x;
        Y = y;
    }

    public Point() {
    }
  
}