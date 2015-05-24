using System;
using System.Collections.Generic;
using System.Linq;
using KNNonAir.Domain.Entity;

namespace KNNonAir.Domain.Service
{
    class GridPartition
    {
        private double _sideLength;
        private double _n;
        private double _gridSideLength;

        public List<MBR> Partition(RoadGraph road, int amount)
        {
            List<MBR> grids = new List<MBR>();
            MBR square = new MBR(road.Graph.Vertices);

            if (square.Height > square.Width)
            {
                _sideLength = square.Height;
                square.X -= (_sideLength - square.Width) / 2;
            }
            else
            {
                _sideLength = square.Width;
                square.Y += (_sideLength - square.Height) / 2;
            }

            _n = Math.Sqrt(amount);
            _gridSideLength = _sideLength / _n;

            for (int i = 0; i < _n; i++)
            {
                for (int j = 0; j < _n; j++)
                {
                    MBR mbr = new MBR(square.X + (_gridSideLength * j), square.Y - (_gridSideLength * i), _gridSideLength, _gridSideLength);
                    mbr.Id = Convert.ToInt16(i * _n + j);
                    grids.Add(mbr);
                }
            }

            Queue<Vertex> vertices = new Queue<Vertex>(road.Graph.Vertices);
            while (vertices.Count() > 0)
            {
                Vertex vertex = vertices.Dequeue();
                foreach (MBR grid in grids)
                {
                    if (grid.Contains(vertex))
                    {
                        grid.Vertices.Add(vertex);
                        break;
                    }
                }
            }

            return grids;
        }
    }
}
