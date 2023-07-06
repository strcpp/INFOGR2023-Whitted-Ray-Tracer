using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace INFOGR2023Template
{
    abstract internal class Primitive
    {
        public Material material { get; set; }

        public abstract void Intersect(Ray ray);

        public abstract Vector3 GetAlbedo(Vector3 I = default(Vector3));
        public abstract Vector3 GetNormal(Vector3 I = default(Vector3));

        protected Primitive(Material material)
        {
            this.material = material;

        }

    }
}
