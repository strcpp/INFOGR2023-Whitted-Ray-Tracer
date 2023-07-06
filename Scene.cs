using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace INFOGR2023Template
{
    internal class Scene
    {
        public List<Primitive> primitives { get; set; }
        public List<Light> lights { get; set; }

        public Scene()
        {
            primitives = new List<Primitive>();
            lights = new List<Light>();
            var radius = 0.5f;
            var ka = 0.1f;
            var kd = 0.6f;
            var ks = 0.4f;
            var shin = 10;

            Material mirror = new Material(new Vector3(1, 1, 1), ka, kd, ks, shin, MaterialType.MIRROR);
            Material greenDiffuse = new Material(new Vector3(0, 1, 0), ka, kd, ks, shin, MaterialType.DIFFUSE);
            Material glass = new Material(new Vector3(1, 1, 1), ka, kd, ks, shin, MaterialType.DIELECTRIC, 1.52f);
            
            Material whiteDiffuse = new Material(new Vector3(1, 1, 1), ka, kd, ks, shin, MaterialType.DIFFUSE);

            primitives.Add(new Sphere(radius, new Vector3(0,0,5), mirror));
            primitives.Add(new Sphere(radius, new Vector3(-1.5f, 0, 5), greenDiffuse, "../../../assets/watermelon.jpg"));
            primitives.Add(new Sphere(radius, new Vector3(1.5f, 0,5), glass));
            primitives.Add(new Plane(-1.0f, new Vector3(0.0f, 1.0f, 0.0f), whiteDiffuse, "../../../assets/grass.jpg"));

            lights.Add(new Light(new Vector3(0.0f, 5.0f, 8.0f), new Vector3(2.0f, 2.0f, 2.0f), 0.5f));
            lights.Add(new Light(new Vector3(2.0f, 5.0f, 8.0f), new Vector3(2.0f, 2.0f, 2.0f), 0.5f));

        }

        public Intersection Intersect(Ray ray)
        {
            float distance = float.MaxValue;
            Primitive closest = null;

            foreach (Primitive primitive in primitives)
            {

                primitive.Intersect(ray);
                if(ray.t < distance)
                {
                    distance = ray.t;
                    closest = primitive;
                }
            }

            return new Intersection(distance, closest);
        }

    }
}
