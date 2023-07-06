using OpenTK.Mathematics;
using INFOGR2023Template;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Template
{
    class MyApplication
    {
        // member variables
        public Surface screen;
        public Raytracer raytracer;
        bool debug = false;
        // constructor
        public MyApplication(Surface screen)
        {
            this.screen = screen;
        }
        // initialize
        public void Init()
        {
            this.raytracer = new Raytracer(screen);
        }
        // tick: renders one frame
        public void Tick(KeyboardState input)
        {
            screen.Clear(0);


            this.raytracer.Render(input);
        }
    }
}