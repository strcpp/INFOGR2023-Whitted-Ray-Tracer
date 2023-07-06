using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace INFOGR2023Template
{

    internal enum MaterialType
    {
        DIFFUSE,
        MIRROR,
        DIELECTRIC
    }

    internal class Material
    {
        public Vector3 color { get; set; }
        public float ka { get; set; }  
        public float kd { get; set; }  
        public float ks { get; set; }  
        public float shininess { get; set; }  

        public float refractionIndex { get; set; }
        public MaterialType type { get; set; }

        public Material(Vector3 color, float ka, float kd, float ks, float shininess, MaterialType type, float refractionIndex = 0)
        {
            this.color = color;
            this.ka = ka;
            this.kd = kd;
            this.ks = ks;
            this.shininess = shininess;
            this.type = type;
            this.refractionIndex = refractionIndex;
        }
    }
}
