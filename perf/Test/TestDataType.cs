using System;
namespace perf
{
   public interface TestData
   {
      TestDataType[] Data { get; }
   }

   public class TestDataType
   {
      public float Time;
      public float X;
      public float Y;
      public float Z;
      public float Speed;
      public float PeakA;

      public TestDataType(float t, float x, float y, float z, float s, float p)
      {
         this.Time  = t;
         this.X     = x;
         this.Y     = y;
         this.Z     = z;
         this.Speed = s;
         this.PeakA = p;
      }

      public TestDataType(double t, double x, double y, double z, double s, double p)
      {
         this.Time  = (float)t;
         this.X     = (float)x;
         this.Y     = (float)y;
         this.Z     = (float)z;
         this.Speed = (float)s;
         this.PeakA = (float)p;
      }
   }
}
