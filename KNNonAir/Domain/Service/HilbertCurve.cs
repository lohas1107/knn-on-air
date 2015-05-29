﻿using KNNonAir.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace KNNonAir.Domain.Service
{
    class HilbertCurve
    {
        private int[,] hilbertArray;

        public HilbertCurve()
        {
            hilbertArray = new int[16, 16] 
            {
                { 1, 2, 15, 16, 17, 20, 21, 22, 235, 236, 237, 240, 241, 242, 255, 256 },
                { 4, 3, 14, 13, 18, 19, 24, 23, 234, 233, 238, 239, 244, 243, 254, 253 },
                { 5, 8, 9, 12, 31, 30, 25, 26, 231, 232, 227, 226, 245, 248, 249, 252 },
                { 6, 7, 10, 11, 32, 29, 28, 27, 230, 229, 228, 225, 246, 247, 250, 251 },
                { 59, 58, 55, 54, 33, 36, 37, 38, 219, 220, 221, 224, 203, 202, 199, 198 },
                { 60, 57, 56, 53, 34, 35, 40, 39, 218, 217, 222, 223, 204, 201, 200, 197 },
                { 61, 62, 51, 52, 47, 46, 41, 42, 215, 216, 211, 210, 205, 206, 195, 196 },
                { 64, 63, 50, 49, 48, 45, 44, 43, 214, 213, 212, 209, 208, 207, 194, 193 },
                { 65, 68, 69, 70, 123, 124, 125, 128, 129, 132, 133, 134, 187, 188, 189, 192 },
                { 66, 67, 72, 71, 122, 121, 126, 127, 130, 131, 136, 135, 186, 185, 190, 191 },
                { 79, 78, 73, 74, 119, 120, 115, 114, 143, 142, 137, 138, 183, 184, 179, 178 },
                { 80, 77, 76, 75, 118, 117, 116, 113, 144, 141, 140, 139, 182, 181, 180, 177 },
                { 81, 82, 95, 96, 97, 98, 111, 112, 145, 146, 159, 160, 161, 162, 175, 176 },
                { 84, 83, 94, 93, 100, 99, 110, 109, 148, 147, 158, 157, 164, 163, 174, 173 },
                { 85, 88, 89, 92, 101, 104, 105, 108, 149, 152, 153, 156, 165, 168, 169, 172 },
                { 86, 87, 90, 91, 102, 103, 106, 107, 150, 151, 154, 155, 166, 167, 170, 171 }
            };
        }

        public List<Region> OrderByHilbert(List<Region> regions)
        {
            Dictionary<int, Region> hilbertRegion = new Dictionary<int, Region>();
            int n = (int)Math.Sqrt(regions.Count);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    hilbertRegion.Add(hilbertArray[i, j], regions[i * n + j]);
                }
            }

            return hilbertRegion.OrderBy(i => i.Key).ToDictionary(i => i.Key, i => i.Value).Values.ToList();
        }
    }
}
