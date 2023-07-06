using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StbImageSharp;
using OpenTK.Mathematics;

namespace INFOGR2023Template
{
    internal class Skybox
    {
        int width { get; set; }
        int height { get; set; }
        float[] pixels { get; set; }

        public Skybox(string filename)
        {
            using (Stream stream = File.OpenRead(filename))
            {
                ImageResultFloat result = ImageResultFloat.FromStream(stream, ColorComponents.RedGreenBlue);

                if (result == null)
                {
                    throw new Exception("Could not load skydome hdr, please check if the file is in the assets folder");
                }

                width = result.Width;
                height = result.Height;

                pixels = result.Data;
            }
        }

        public Vector3 Sample(Ray ray)
        {
            var dir = ray.direction;

            double theta = Math.Atan2(ray.direction.Z, ray.direction.X);
            double phi = Math.Acos(ray.direction.Y);

            // shift theta range from [-π, π] to [0, 2π]
            if (theta < 0)
            {
                theta = theta + 2 * Math.PI;
            }

            int u = (int)((width * (theta) / (2 * Math.PI)) % width);
            int v = (int)((height * phi / Math.PI) % height);

            int id = (u + v * width) % (width * height);

            return new Vector3(pixels[id * 3], pixels[id * 3 + 1], pixels[id * 3 + 2]) * 0.5f;
        }
    }
}
