using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using StbImageSharp;

namespace INFOGR2023Template
{
    internal class Plane : Primitive
    {
        public float distance;
        public Vector3 normal;
        public ImageResult? texture = null;

        public Plane(float distance, Vector3 normal, Material material, string texturePath = "") : base(material)
        {
            this.distance = distance;
            this.normal = normal;

            if (!string.IsNullOrEmpty(texturePath))
            {
                using (var stream = File.OpenRead(texturePath))
                {
                    texture = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
                }
            }
        }

        public override Vector3 GetAlbedo(Vector3 I = default)
        {
            if (texture != null)
            {

                int u = (int)MathF.Abs(I.X * texture.Width);
                int v = (int)MathF.Abs(I.Z * texture.Height);

                u = u % texture.Width;
                v = v % texture.Height;

                int index = (v * texture.Width + u) * 4;  

                return new Vector3(texture.Data[index] / 255f, texture.Data[index + 1] / 255f, texture.Data[index + 2] / 255f);
            }

            return material.color;
        }

        public override Vector3 GetNormal(Vector3 I)
        {
            return normal;
        }

        public override void Intersect(Ray ray)
        {
            float denom = Vector3.Dot(normal, ray.direction);
            if (Math.Abs(denom) > 0.0001f) // making sure we don't divide by zero..
            {
                float t = (distance - Vector3.Dot(normal, ray.origin)) / denom;
                if (t >= 0 && t < ray.t)
                {
                    ray.t = t;
                }
            }
        }
    }
}
