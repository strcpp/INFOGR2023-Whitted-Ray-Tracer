using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template;
using OpenTK.Mathematics;

namespace INFOGR2023Template
{

     internal class Line
    {
        public int x0 { get; set; }

        public int y0 { get; set; }

        public int x1 { get; set; }

        public int y1 { get; set;}

        public Line(int x0, int y0, int x1, int y1)
        {
            this.x0 = x0;
            this.y0 = y0;
            this.x1 = x1;
            this.y1 = y1;
        }
    }
    internal class DebugRender
    {

        Surface screen;
        Vector2 camPos;
        Vector2 center;
        Raytracer tracer;

        int xScale = 1;
        int scale = 20;
        int numRays = 10;
        const float MAX_DIST = 5000.0f;
        public DebugRender(Surface screen,Raytracer tracer)
        {
            
            this.screen = screen;
            this.camPos = new Vector2(screen.width / 2.0f + screen.width / 4.0f, screen.height / 2.0f);

            this.tracer = tracer;
        }

        private void DrawCircle(int centerX, int centerY, int radius, int color)
        {
            float lines = 100;
            float angleStep = 2 * (float)Math.PI / lines;

            for (int i = 0; i < lines; i++)
            {
                float angle1 = i * angleStep;
                float angle2 = ((i + 1) % lines) * angleStep;

                int x1 = centerX + (int)(radius * Math.Cos(angle1));
                int y1 = centerY + (int)(radius * Math.Sin(angle1));
                int x2 = centerX + (int)(radius * Math.Cos(angle2));
                int y2 = centerY + (int)(radius * Math.Sin(angle2));

                screen.Line(x1, y1, x2, y2, color);
            }
        }


        public float Trace(Ray ray, out Primitive nearestPrim)
        {
            var intersection = this.tracer.scene.Intersect(ray);

            if (intersection.nearestPrimitive != null)
            {
                nearestPrim = intersection.nearestPrimitive;
                return ray.t;

            }

            nearestPrim = null;
            return MAX_DIST;
        }

        public void Render()
        {


            var halfHeight = screen.height / 2;
            var shadowLines = new List<Line>();
            var reflectedLines = new List<Line>();
            var refractionLines = new List<Line>();

            for (int i = 0; i < screen.width / 2.0f; i++)
            {
                if (i % numRays == 0)
                {
                    var ray = this.tracer.camera.GetPrimaryRay(i, halfHeight, this.screen);
                    Primitive np = null;
                    var t = this.Trace(ray, out np);

                    var linePos = ray.origin + ray.direction * t * scale;

                    Vector2 linePosScreen = new Vector2((linePos.X) * xScale,
                                                        (linePos.Z));


                    this.screen.Line((int)camPos.X, (int)camPos.Y, (int)(camPos.X + linePosScreen.X), (int)(camPos.Y - linePosScreen.Y), 0xff0000);


                    // shadow lines
                    if(np != null && np is not Plane)
                    {

                        
                        foreach (Light light in this.tracer.scene.lights)
                        {
                            var I = ray.origin + t * ray.direction;
                            var L = (light.position - I);
                            var dir = L.Normalized();

                            Ray shadowRay = new Ray(I + Utils.EPSILON* dir, dir);
                            Primitive npShadow = null;
                            var tShadow = this.Trace(shadowRay, out npShadow);

                            if (npShadow is null)
                            {
                                var diff = light.position - this.tracer.camera.position;

                                Vector2 lightPosScreen = new Vector2((diff.X) * scale * xScale,
                                                                      (diff.Z) * scale);


                                shadowLines.Add(new Line((int)(camPos.X + linePosScreen.X),
                                                         (int)(camPos.Y - linePosScreen.Y),
                                                         (int)(camPos.X + lightPosScreen.X),
                                                         (int)(camPos.Y - lightPosScreen.Y)));
                            }
                        }


                        if (np.material.type == MaterialType.MIRROR)
                        {
                            Vector3 I = ray.origin + t * ray.direction;
                            Vector3 normal = np.GetNormal(I);
                            Vector3 reflectionDirection = Utils.Reflect(ray.direction, normal);

                            Ray reflectedRay = new Ray(I + Utils.EPSILON * reflectionDirection, reflectionDirection);

                            Primitive npReflect = null;
                            var tReflect = this.Trace(reflectedRay, out npReflect);

                            if (npReflect != null)
                            {
                                var linePosReflect = reflectedRay.origin + reflectedRay.direction * tReflect;


                                var diff = linePosReflect - this.tracer.camera.position;

                                Vector2 linePosScreenReflect = new Vector2((diff.X) * scale * xScale,
                                                                           (diff.Z) * scale);

                                reflectedLines.Add(new Line((int)(camPos.X + linePosScreen.X),
                                                (int)(camPos.Y - linePosScreen.Y),
                                                (int)(camPos.X + linePosScreenReflect.X),
                                                (int)(camPos.Y - linePosScreenReflect.Y)
                                     ));

                            }
                        }


                        if (np.material.type == MaterialType.DIELECTRIC)
                        {
                            Vector3 I = ray.origin + t * ray.direction;
                            Vector3 normal = np.GetNormal(I);

                            Vector3 reflectedDirection = Utils.Reflect(ray.direction, normal).Normalized();
                            Vector3 refractedDirection = Utils.Refract(ray.direction, normal, np.material.refractionIndex).Normalized();

                            float fresnel = Utils.Fresnel(ray.direction, normal, np.material.refractionIndex);

                            if (fresnel < 1)
                            {

                                // Reflected
                                Ray reflectedRay = new Ray(I + Utils.EPSILON * reflectedDirection, reflectedDirection);

                                Primitive npReflect = null;
                                var tReflect = this.Trace(reflectedRay, out npReflect);

                                if (npReflect != null)
                                {
                                    var linePosReflect = reflectedRay.origin + reflectedRay.direction * tReflect;

                                    var diff = linePosReflect - this.tracer.camera.position;

                                    Vector2 linePosScreenReflect = new Vector2((diff.X) * scale * xScale,
                                                                               (diff.Z) * scale);

                                    reflectedLines.Add(new Line((int)(camPos.X + linePosScreen.X),
                                                                (int)(camPos.Y - linePosScreen.Y),
                                                                (int)(camPos.X + linePosScreenReflect.X),
                                                                (int)(camPos.Y - linePosScreenReflect.Y)
                                                     ));
                                }

                            }


                            Ray refractedRay = new Ray(I + Utils.EPSILON * refractedDirection, refractedDirection);

                            Primitive npRefract = null;
                            var tRefract = this.Trace(refractedRay, out npRefract);

                            if (npRefract != null)
                            {
                                var linePosRefract = refractedRay.origin + refractedRay.direction * tRefract;

                                var diff = linePosRefract - this.tracer.camera.position;

                                Vector2 linePosScreenRefract = new Vector2((diff.X) * scale * xScale,
                                                                           (diff.Z) * scale);

                                refractionLines.Add(new Line((int)(camPos.X + linePosScreen.X),
                                                            (int)(camPos.Y - linePosScreen.Y),
                                                            (int)(camPos.X + linePosScreenRefract.X),
                                                            (int)(camPos.Y - linePosScreenRefract.Y)
                                                 ));
                            }


                        }


                    }
                }
            }

            foreach(var line in shadowLines)
            {
                this.screen.Line(line.x0,
                                 line.y0,
                                 line.x1,
                                 line.y1,
                                 0x00ff00);
            }

            foreach(var line in reflectedLines)
            {
                this.screen.Line(line.x0,
                             line.y0,
                             line.x1,
                             line.y1,
                             0x0000ff);
            }

            foreach (var line in refractionLines)
            {
                this.screen.Line(line.x0,
                             line.y0,
                             line.x1,
                             line.y1,
                             0xffffff);
            }

            foreach (var prim in this.tracer.scene.primitives)
            {
                if (prim is Sphere sphere)
                {
                    var dir = sphere.position - this.tracer.camera.position;

                    int x = (int)(camPos.X + dir.X * scale * xScale);
                    int y = (int)(camPos.Y - dir.Z * scale);

                    int radius = (int)(sphere.radius * scale * xScale);

                    DrawCircle(x, y, radius, Utils.ConvertColor(sphere.material.color));
                }
            }
        }
    }

}
