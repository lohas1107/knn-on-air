using QuickGraph;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KNNonAir.Domain.Entity
{
    [Serializable]
    public class Region : ISerializable
    {
        public int Id { get; set; }
        public List<Vertex> PoIs { get; set; }
        public RoadGraph Road { get; set; }
        public List<Vertex> BorderPoints { get; set; }

        public Region()
        {
            Id = -1;
            PoIs = new List<Vertex>();
            Road = new RoadGraph(false);
            BorderPoints = new List<Vertex>();
        }

        public void AddNVC(VoronoiCell nvc)
        {
            PoIs.Add(nvc.PoI);
            foreach (Edge<Vertex> edge in nvc.Road.Graph.Edges) Road.Graph.AddVerticesAndEdge(edge);
            foreach(Vertex borderPoint in nvc.BorderPoints) BorderPoints.Add(borderPoint);
        }

        public void RemoveSameBorder()
        {
            List<Vertex> tempBorders = new List<Vertex>();
            foreach (BorderPoint borderPoint in BorderPoints)
            {
                foreach(Vertex poi in borderPoint.PoIs)
                {
                    if (!PoIs.Contains(poi))
                    {
                        tempBorders.Add(borderPoint);
                        break;
                    }
                }
            }

            BorderPoints.Clear();
            foreach (Vertex border in tempBorders) BorderPoints.Add(border);
        }

        public void SetVerticesId()
        {
            foreach (Vertex vertex in Road.Graph.Vertices)
            {
                if (!(vertex is BorderPoint)) vertex.RegionId = Id;
            }            
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Id", Id);
            info.AddValue("PoIs", PoIs);
            info.AddValue("Road", Road);
        }
    }
}
