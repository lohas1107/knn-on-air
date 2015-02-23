using Geo;
using System;

namespace KNNonAir
{
    class Arithmetics
    {
        public static double CalculateDistance(Coordinate source, Coordinate target)
        {
            return Math.Sqrt(Math.Pow(target.Latitude - source.Latitude, 2) + Math.Pow(target.Longitude - source.Longitude, 2));
        }

        public static Vertex Project(Coordinate source, Coordinate target, Coordinate vertex)
        {
            double U = ((vertex.Latitude - source.Latitude) * (target.Latitude - source.Latitude)) + ((vertex.Longitude - source.Longitude) * (target.Longitude - source.Longitude));
            double Udenom = Math.Pow(target.Latitude - source.Latitude, 2) + Math.Pow(target.Longitude - source.Longitude, 2);
            U /= Udenom;

            double latitude = source.Latitude + (U * (target.Latitude - source.Latitude));
            double longitude = source.Longitude + (U * (target.Longitude - source.Longitude));
            Vertex result = new InterestPoint(latitude, longitude);

            double minX, maxX, minY, maxY;
            minX = Math.Min(source.Latitude, target.Latitude);
            maxX = Math.Max(source.Latitude, target.Latitude);
            minY = Math.Min(source.Longitude, target.Longitude);
            maxY = Math.Max(source.Longitude, target.Longitude);

            bool isValid = (result.Coordinate.Latitude >= minX && result.Coordinate.Latitude <= maxX) && (result.Coordinate.Longitude >= minY && result.Coordinate.Longitude <= maxY);
            return isValid ? result : null;
        }

        public static Vertex FindDivisionPoint(double distance, Coordinate source, Coordinate target)
        {
            double m = distance;
            double n = CalculateDistance(source, target) - distance;

            double latitude = (m * target.Latitude + n * source.Latitude) / (m + n);
            double longitude = (m * target.Longitude + n * source.Longitude) / (m + n);

            return new Vertex(latitude, longitude);
        }

        public static double GetSlope(QuickGraph.Edge<Vertex> edge)
        {
            double x = edge.Target.Coordinate.Latitude - edge.Source.Coordinate.Latitude;
            double y = edge.Target.Coordinate.Longitude - edge.Source.Coordinate.Longitude;

            if (x == 0) return double.MaxValue;
            else return y / x;
        }

        public static double GetIncludedAngle(double slope1, double slope2)
        {
            return Math.Abs(Math.Atan(slope2) - Math.Atan(slope1));
        }
    }
}
