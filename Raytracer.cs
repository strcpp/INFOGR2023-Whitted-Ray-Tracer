using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using StbImageSharp;

namespace INFOGR2023Template
{
    internal class Raytracer
    {
        public Scene scene { get; set; }
        public Camera camera { get; set; }
        public Surface screen { get; set; }

        DebugRender DebugRender { get; set; }

        const int MAX_BOUNCES = 5;

        Skybox skybox { get; set; }

        public Raytracer(Surface surface)
        {
            this.screen = surface;
            this.camera = new Camera((screen.width / 2.0f) / (screen.height));
            this.scene = new Scene();
            this.DebugRender = new DebugRender(screen, this);


            this.skybox = new Skybox("../../../assets/skydome2.hdr");

        }

        // cast shadow ray, then apply phong lighting if light is not occluded.
        public Vector3 Illuminate(Vector3 normal, Vector3 I, Material material)
        {
            Vector3 color = new Vector3(0, 0, 0);
            Vector3 ambient = new Vector3(1.0f, 1.0f, 1.0f);

            // Ambient term
            color += material.ka * ambient;

            foreach (Light light in scene.lights)
            {
                var L = (light.position - I);
                var dir = L.Normalized();

                Ray shadowRay = new Ray(I +  Vector3.Multiply(dir, Utils.EPSILON), dir);

                var intersection = scene.Intersect(shadowRay);

                if (intersection.distance < L.Length)
                {
                    continue;
                }

                // Diffuse term
                float nDotL = Vector3.Dot(dir, normal);
                if (nDotL > 0)
                {
                    color += material.kd * light.color * light.intensity * nDotL;
                }

                Vector3 viewDir = (camera.position - I).Normalized();
                // Specular term
                Vector3 reflectDir = Utils.Reflect(-dir, normal);
                float rDotV = Vector3.Dot(reflectDir, viewDir);
                if (rDotV > 0)
                {
                    color += material.ks * light.color * light.intensity * (float) Math.Pow(rDotV, material.shininess) ;
                    
                }
            }

            return color;
        }


        public Vector3 Trace(Ray ray, int bounce)
        {

            if (bounce == MAX_BOUNCES) return skybox.Sample(ray);

            var intersection = scene.Intersect(ray);

            if(intersection.nearestPrimitive != null)
            {

                var material = intersection.nearestPrimitive.material;
                var intersectionPoint = ray.origin + ray.t * ray.direction;
                var normal = intersection.nearestPrimitive.GetNormal(intersectionPoint);
                var albedo = intersection.nearestPrimitive.GetAlbedo(intersectionPoint);

                switch(material.type)
                {
                    case MaterialType.DIFFUSE:
                    {
                        return albedo * Illuminate(normal, intersectionPoint, material);
                    }
                    case MaterialType.MIRROR:
                    {
                        var reflected = Utils.Reflect(ray.direction, normal);
                        return albedo * Trace(new Ray(intersectionPoint + reflected * Utils.EPSILON, reflected), bounce + 1);
                    }
                    case MaterialType.DIELECTRIC:
                    {
                            float refractionIndex = material.refractionIndex;
                            float fresnel = Utils.Fresnel(ray.direction, normal, refractionIndex);
                            Vector3 refracted = new Vector3(0.0f, 0.0f, 0.0f);

                            if (fresnel < 1) 
                            {
                                var refractionDir = Utils.Refract(ray.direction, normal, refractionIndex).Normalized();

                                refracted = Trace(new Ray(intersectionPoint + refractionDir * Utils.EPSILON, refractionDir), bounce + 1);
                            }

                            // reflection
                            Vector3 reflected = Utils.Reflect(ray.direction, normal);

                            var reflection = Trace(new Ray(intersectionPoint + reflected * Utils.EPSILON, reflected), bounce + 1);

                            return albedo * refracted * (1 - fresnel) + albedo * reflection * fresnel;
                    }
                }

            }

            return skybox.Sample(ray);
        }

        private void moveCamera(KeyboardState input) {
            float delta = 0.2f;
            float rotateDelta = 3.0f;
            float fovDelta = 0.1f;
            
            if (input.IsKeyDown(Keys.W))
            {
                this.camera.MoveForward(delta);
            }

            if (input.IsKeyDown(Keys.A))
            {
                this.camera.MoveSide(delta);
            }

            if (input.IsKeyDown(Keys.S))
            {
                this.camera.MoveForward(-delta);

            }

            if (input.IsKeyDown(Keys.D))
            {
                this.camera.MoveSide(-delta);
            }


            if (input.IsKeyDown(Keys.Up))
            {
                this.camera.Rotate(rotateDelta, 0);
            }

            if (input.IsKeyDown(Keys.Left))
            {
                this.camera.Rotate(0, rotateDelta);
            }

            if (input.IsKeyDown(Keys.Down))
            {
                this.camera.Rotate(-rotateDelta, 0);

            }

            if (input.IsKeyDown(Keys.Right))
            {
                this.camera.Rotate(0, -rotateDelta);
            }

            if (input.IsKeyDown(Keys.Q))
            {
                this.camera.FOV += fovDelta;


            }

            if(input.IsKeyDown(Keys.E))
            {
                this.camera.FOV -= fovDelta;

            }
        }
 
        public void Render(KeyboardState input)
        {
            moveCamera(input);

            this.DebugRender.Render();
            const int numSamples = 2;

            Parallel.For(0, screen.width / 2, i =>
            {
                for (int j = 0; j < screen.height; j++)
                {
                    Vector3 color = Vector3.Zero;
                    for (int s = 0; s < numSamples; s++)
                    {
                        for (int t = 0; t < numSamples; t++)
                        {
                            float u = (s + 0.5f) / numSamples / (screen.width / 2);
                            float v = (t + 0.5f) / numSamples / screen.height;

                            Ray ray = this.camera.GetPrimaryRay(i, j, screen);
                            ray.origin += new Vector3(u, v, 0);
                            color += Trace(ray, 0);
                        }
                    }

                    color /= (numSamples * numSamples);

                    screen.Plot(i, j, Utils.ConvertColor(color));
                }
            });
        }
    }
}
