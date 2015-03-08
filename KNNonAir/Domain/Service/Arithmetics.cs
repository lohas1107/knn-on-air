using Geo;
using KNNonAir.Domain.Entity;
using QuickGraph;
using System;

namespace KNNonAir.Domain.Service
{
    class Arithmetics
    {
        public static double GetDistance(Vertex source, Vertex target)
        {
            return Math.Sqrt(Math.Pow(target.Coordinate.Latitude - source.Coordinate.Latitude, 2) + Math.Pow(target.Coordinate.Longitude - source.Coordinate.Longitude, 2));
        }

        public static Vertex Project(Vertex source, Vertex target, Vertex vertex)
        {
            double U = ((vertex.Coordinate.Latitude - source.Coordinate.Latitude) * (target.Coordinate.Latitude - source.Coordinate.Latitude)) + ((vertex.Coordinate.Longitude - source.Coordinate.Longitude) * (target.Coordinate.Longitude - source.Coordinate.Longitude));
            double Udenom = Math.Pow(target.Coordinate.Latitude - source.Coordinate.Latitude, 2) + Math.Pow(target.Coordinate.Longitude - source.Coordinate.Longitude, 2);
            U /= Udenom;

            double latitude = source.Coordinate.Latitude + (U * (target.Coordinate.Latitude - source.Coordinate.Latitude));
            double longitude = source.Coordinate.Longitude + (U * (target.Coordinate.Longitude - source.Coordinate.Longitude));
            Vertex result = new InterestPoint(latitude, longitude);

            double minX, maxX, minY, maxY;
            minX = Math.Min(source.Coordinate.Latitude, target.Coordinate.Latitude);
            maxX = Math.Max(source.Coordinate.Latitude, target.Coordinate.Latitude);
            minY = Math.Min(source.Coordinate.Longitude, target.Coordinate.Longitude);
            maxY = Math.Max(source.Coordinate.Longitude, target.Coordinate.Longitude);

            bool isValid = (result.Coordinate.Latitude >= minX && result.Coordinate.Latitude <= maxX) && (result.Coordinate.Longitude >= minY && result.Coordinate.Longitude <= maxY);
            return isValid ? result : null;
        }

        public static Vertex FindDivisionPoint(double distance, Vertex source, Vertex target)
        {
            double m = distance;
            double n = GetDistance(source, target) - distance;

            double latitude = (m * target.Coordinate.Latitude + n * source.Coordinate.Latitude) / (m + n);
            double longitude = (m * target.Coordinate.Longitude + n * source.Coordinate.Longitude) / (m + n);

            return new Vertex(latitude, longitude);
        }

        public static double GetSlope(Edge<Vertex> edge)
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
