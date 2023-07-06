using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using StbImageSharp;

namespace INFOGR2023Template
{
    internal class Sphere : Primitive
    {

        public float radius;
        public Vector3 position;
        public ImageResult? texture = null;
        
        public Sphere(
            float radius, Vector3 position, Material material, string? texturePath = ""): base(material)
        {
            this.radius = radius;
            this.position = position;

            if(!string.IsNullOrEmpty(texturePath))
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
                Vector3 normal = GetNormal(I);
                float u = 0.5f + (MathF.Atan2(normal.Z, normal.X) / (2 * MathF.PI));
                float v = 0.5f - (MathF.Asin(normal.Y) / MathF.PI);

                int x = (int)(u * texture.Width);
                int y = (int)(v * texture.Height);

                x = Math.Clamp(x, 0, texture.Width - 1);
                y = Math.Clamp(y, 0, texture.Height - 1);

                int index = (y * texture.Width + x) * 4; 

                return new Vector3(texture.Data[index] / 255f, texture.Data[index + 1] / 255f, texture.Data[index + 2] / 255f);
            }

            return material.color;
        }

        public override Vector3 GetNormal(Vector3 I)
        {

            return Vector3.Subtract(I, position).Normalized();
        }

        public override void Intersect(Ray ray)
        {
            Vector3 l = ray.origin - position;

            float a = Vector3.Dot(ray.direction, ray.direction);
            float b = 2.0f * Vector3.Dot(l, ray.direction);
            float c = Vector3.Dot(l, l) - this.radius* this.radius;
            float discriminant = b * b - 4 * a * c;

            if (discriminant >= 0)
            {
                float t1 = (-b - MathF.Sqrt(discriminant)) / (2.0f * a);
                float t2 = (-b + MathF.Sqrt(discriminant)) / (2.0f * a);

                if (t1 > t2)
                {
                    float temp = t1;
                    t1 = t2;
                    t2 = temp;
                }

                if (t1 > 0 && (t1 < ray.t || ray.t == 0))
                {
                    ray.t = t1;
                }
                else if (t2 > 0 && (t2 < ray.t || ray.t == 0))
                {
                    ray.t = t2;
                }
            }
        }
    }
}
