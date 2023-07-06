using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace INFOGR2023Template
{
    internal class Light
    {
        public Vector3 position;
        public Vector3 color;
        public float intensity;

        public Light(
            Vector3 position, Vector3 color, float intensity)
        {
            this.position = position;
            this.intensity = intensity;
            this.color = color;
        }
    }
}
