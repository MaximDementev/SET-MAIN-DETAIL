using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SET_MAIN_DETAIL
{
    public class DimensionBox
    {
        public Autodesk.Revit.DB.XYZ MinPoint { get; set; }
        public Autodesk.Revit.DB.XYZ MaxPoint {  get; set; }
        public double Radius { get; set; }

        public DimensionBox(Autodesk.Revit.DB.XYZ minPoint, Autodesk.Revit.DB.XYZ maxPoint)
        {
            MinPoint = minPoint;
            MaxPoint = maxPoint;
        }

        public DimensionBox(Autodesk.Revit.DB.XYZ CenterPoint, double radius)
        {
            if (CenterPoint == null) throw new Exception("Центральная точка пустая");
            if (radius == null || radius == 0) throw new Exception("Радиус пустой или нулевой");
            double minX = CenterPoint.X - radius;
            double minY = CenterPoint.Y - radius;
            double maxX = CenterPoint.X + radius;
            double maxY = CenterPoint.Y + radius;

            MinPoint = new XYZ(minX, minY, 0);
            MaxPoint = new XYZ(maxX, maxY, 0);
        }

        public DimensionBox(List<Autodesk.Revit.DB.XYZ> minPointList, List<Autodesk.Revit.DB.XYZ> maxPointList)
        {
            if (minPointList == null || maxPointList == null || !minPointList.Any() || !maxPointList.Any())
            {
                throw new ArgumentException("Списки точек не должны быть пустыми или null.");
            }

            double minPointX = minPointList.Min(point => point.X);
            double minPointY = minPointList.Min(point => point.Y);

            double maxPointX = maxPointList.Max(point => point.X);
            double maxPointY = maxPointList.Max(point => point.Y);

            MinPoint = new Autodesk.Revit.DB.XYZ(minPointX, minPointY, 0);
            MaxPoint = new Autodesk.Revit.DB.XYZ(maxPointX, maxPointY, 0);
        }

        public void AddPoint(Autodesk.Revit.DB.XYZ Point)
        { 
            if (CheckPointIsInside(Point)) return;

            if(Point.X < MinPoint.X || Point.Y < MinPoint.Y)
            {
                double minX = MinPoint.X; double minY = MinPoint.Y;
                if (Point.X < MinPoint.X) minX = Point.X;
                if (Point.Y < MinPoint.Y) minY = Point.Y;
                MinPoint = new XYZ(minX, minY, 0);
                return;
            }

            if (Point.X > MaxPoint.X || Point.Y > MaxPoint.Y)
            {
                double maxX = MaxPoint.X; double maxY = MaxPoint.Y;
                if (Point.X > MaxPoint.X) maxX = Point.X;
                if (Point.Y > MaxPoint.Y) maxY = Point.Y;
                MaxPoint = new XYZ(maxX, maxY, 0);
                return;
            }
        }

        public bool CheckPointIsInside (Autodesk.Revit.DB.XYZ Point)
        {
            if (Point == null) throw new Exception("Точка не имеет координат");

            if(Point.X > MinPoint.X && Point.Y > MinPoint.Y && Point.X < MaxPoint.X && Point.Y < MaxPoint.Y) return true;
            else return false;
        }

        public double GetRadius()
        {
            if (MaxPoint == null || MinPoint == null)
                throw new InvalidOperationException("MaxPoint или MinPoint не инициализированы.");

            double dx = MaxPoint.X - MinPoint.X;
            double dy = MaxPoint.Y - MinPoint.Y;

            if (dx == 0 && dy == 0)
                throw new InvalidOperationException("MaxPoint и MinPoint совпадают.");

            double sumOfSquares = dx * dx + dy * dy;
            double radius = Math.Sqrt(sumOfSquares);

            if (double.IsInfinity(radius) || double.IsNaN(radius))
                throw new InvalidOperationException("Ошибка вычисления радиуса: переполнение или NaN.");

            return radius;
        }
    }
}
