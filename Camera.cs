using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Template;

namespace INFOGR2023Template
{
    internal class Camera
    {
        public Vector3 position { get; set; }
        public Vector3 direction { get; set; }
        public Vector3 tempDir { get; set; }
        public Vector3 up { get; set; }
        public Vector3 right { get; set; }

        public float yaw, pitch;

        float fov = 45.0f;
        public float FOV { get; set; } = 45f;
        public float focalDistance { get; set; } = 1.0f;

        public float aspectRatio { get; set; }

        public Camera(float aspectRatio)
        {
            position = new Vector3(0,0,0);
            direction = new Vector3(0,0,1);
            tempDir = new Vector3(0, 0, 1);
            up = new Vector3(0,1,0);
            right = Vector3.Cross(direction, up);
            this.aspectRatio = aspectRatio;
        }

        public void MoveForward(float delta)
        {
            this.position += this.direction * delta;
        }

        public void MoveSide(float delta)
        {
            this.position += this.right * delta;
        }

        public void Rotate(float deltaX, float deltaY)
        {
            this.pitch += deltaX;
            this.yaw += deltaY;

            var rotationX = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(this.pitch));

            var rotationY = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(this.yaw));

            var temp = rotationY * rotationX * new Vector4(this.tempDir.X, this.tempDir.Y, this.tempDir.Z, 1);

            this.direction = new Vector3(temp.X, temp.Y, temp.Z);
        }

        public Ray GetPrimaryRay(int x, int y, Surface screen)
        {

            float fovSize = MathF.Tan(FOV / 2 * (MathF.PI / 180));

            Vector3 center = position + focalDistance * this.direction;

            Vector3 dir = direction;

            right = Vector3.Cross(this.direction, this.up).Normalized();

            Vector3 up = Vector3.Normalize(Vector3.Cross(right, dir));

            Vector3 topLeft = center - fovSize * focalDistance * aspectRatio * right + fovSize * focalDistance * up;
            Vector3 topRight = center + fovSize * right * focalDistance * aspectRatio + fovSize * focalDistance * up;
            Vector3 bottomLeft = center - fovSize * right * focalDistance * aspectRatio - fovSize * focalDistance * up;

            float u = (float)x * (1.0f / (screen.width >> 1));
            float v = (float)y * (1.0f / screen.height);
            Vector3 P = topLeft + (1 - u) * (topRight - topLeft) + v * (bottomLeft - topLeft);

            return new Ray(position, Vector3.Normalize(P - position));
        }


    }
}
