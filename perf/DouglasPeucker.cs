using System;
using System.Collections.Generic;

public static class DouglasPeucker
{
   // A generic point with double coordinates
   public struct Point
   {
      public Point(double x, double y)
      {
         this.X = x;
         this.Y = y;
      }

      public double X;
      public double Y;
   }

   // Uses the Douglas Peucker algorithim to reduce the number of points
   public static IList<Point> Reduce(IList<Point> Points, double Tolerance)
   {
      if (Points == null || Points.Count < 3) return Points;

      int firstPoint = 0;
      int lastPoint = Points.Count - 1;
      var pointIndexsToKeep = new List<int>();

      // Add the first and last index to the keepers
      pointIndexsToKeep.Add(firstPoint);
      pointIndexsToKeep.Add(lastPoint);

      // The first and the last point can not be the same
      while (Points[firstPoint].Equals(Points[lastPoint]))
      {
         lastPoint--;
      }

      Reduce(Points, firstPoint, lastPoint, Tolerance, ref pointIndexsToKeep);

      var returnPoints = new List<Point>();
      pointIndexsToKeep.Sort();
      foreach (int index in pointIndexsToKeep)
      {
         returnPoints.Add(Points[index]);
      }

      return returnPoints;
   }

   /// <summary>
   /// Douglases the peucker reduction
   /// </summary>
   /// <param name="points">The points</param>
   /// <param name="firstPoint">The first point</param>
   /// <param name="lastPoint">The last point</param>
   /// <param name="tolerance">The tolerance</param>
   /// <param name="pointIndexsToKeep">The point indexs to keep</param>
   private static void Reduce(IList<Point> points, int firstPoint, int lastPoint, double tolerance, ref List<int> pointIndexsToKeep)
   {
      double maxDistance = 0;
      int indexFarthest = 0;

      for (int index = firstPoint; index < lastPoint; index++)
      {
         double distance = PerpendicularDistance(points[firstPoint], points[lastPoint], points[index]);
         if (distance > maxDistance)
         {
             maxDistance = distance;
             indexFarthest = index;
         }
      }

      if (maxDistance > tolerance && indexFarthest != 0)
      {
         // Add the largest point that exceeds the tolerance
         pointIndexsToKeep.Add(indexFarthest);

         Reduce(points, firstPoint, indexFarthest, tolerance, ref pointIndexsToKeep);
         Reduce(points, indexFarthest, lastPoint, tolerance, ref pointIndexsToKeep);
      }
   }

   // The distance of a point from a line made from point1 and point2.
   public static double PerpendicularDistance(Point Point1, Point Point2, Point Point)
   {
      //Area = |(1/2)(x1y2 + x2y3 + x3y1 - x2y1 - x3y2 - x1y3)|   *Area of triangle
      //Base = √((x1-x2)²+(x1-x2)²)                               *Base of Triangle*
      //Area = .5*Base*H                                          *Solve for height
      //Height = Area/.5/Base

      double area   = Math.Abs(0.5 * (Point1.X * Point2.Y + 
                                      Point2.X * Point.Y  + 
                                      Point.X  * Point1.Y - 
                                      Point2.X * Point1.Y - 
                                      Point.X  * Point2.Y - 
                                      Point1.X * Point.Y));
      
      double bottom = Math.Sqrt(Math.Pow(Point1.X - Point2.X, 2) + 
                                Math.Pow(Point1.Y - Point2.Y, 2));

      double height = area / bottom * 2;

      return height;
   }
}
