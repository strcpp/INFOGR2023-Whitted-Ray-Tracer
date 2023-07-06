using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace INFOGR2023Template
{
    internal class Intersection
    {
        public float distance;
        public Primitive nearestPrimitive;
        public Vector3 normal;

        public Intersection(float distance, Primitive nearestPrimitive) { 
            this.distance = distance; 
            this.nearestPrimitive = nearestPrimitive; 
        }
    }
}
