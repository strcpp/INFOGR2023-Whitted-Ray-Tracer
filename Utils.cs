using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace INFOGR2023Template
{
    internal class Utils
    {

        public const float EPSILON = 0.001f;

        public static int ConvertColor(Vector3 color)
        {
            return new Color4(
                (byte)(Math.Min(1.0f, color.X) * 255),
                (byte)(Math.Min(1.0f, color.Y) * 255),
                (byte)(Math.Min(1.0f, color.Z) * 255),
                255
            ).ToArgb();
        }

        public static Vector3 Reflect(Vector3 incident, Vector3 normal)
        {
            return incident - 2 * Vector3.Dot(incident, normal) * normal;
        }

        public static Vector3 Refract(Vector3 I, Vector3 N, float refractionIndex)
        {
            float cosi = Vector3.Dot(I, N);
            float etai = 1, etat = refractionIndex;
            Vector3 n = N;
            if (cosi < 0) { cosi = -cosi; }
            else
            {
                float temp = etai;
                etai = etat;
                etat = temp;
                n = -N;
            }
            float eta = etai / etat;
            float k = 1 - eta * eta * (1 - cosi * cosi);
            return k < 0 ? Vector3.Zero : eta * I + (eta * cosi - MathF.Sqrt(k)) * n;
        }

        public static float Fresnel(Vector3 I, Vector3 N, float refractionIndex)
        {
            float cosi = Math.Clamp(Vector3.Dot(I, N), -1.0f, 1.0f);
            float etai = 1, etat = refractionIndex;
            if (cosi > 0)
            {
                float temp = etai;
                etai = etat;
                etat = temp;
            }
            // Compute sini using Snell's law
            float sint = etai / etat * MathF.Sqrt(Math.Max(0.0f, 1 - cosi * cosi));
            // Total internal reflection
            if (sint >= 1)
            {
                return 1;
            }
            else
            {
                float cost = MathF.Sqrt(Math.Max(0.0f, 1 - sint * sint));
                cosi = MathF.Abs(cosi);
                float Rs = ((etat * cosi) - (etai * cost)) / ((etat * cosi) + (etai * cost));
                float Rp = ((etai * cosi) - (etat * cost)) / ((etai * cosi) + (etat * cost));
                return (Rs * Rs + Rp * Rp) / 2;
            }
        }
        public static float RandomFloat()
        {
            Random random = new Random();
            double range = random.NextDouble(); 
            return (float)range; 
        }
    }
}
