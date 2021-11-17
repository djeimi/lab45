using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace lab4_5
{
    public class Plot
    {
        private GameWindow windowsforms;
        private float view = 10.0f;
        private float x = 0.0f;
        private float y = 0.0f;
        private float[] lp = { 20.0f, 20.0f, 20.0f };

        private List<float[]> p0 = new List<float[]>();
        private List<float[]> p1 = new List<float[]>();
        private List<float[]> p2 = new List<float[]>();
        private List<float[]> p3 = new List<float[]>();

        private int accuracy = 10;
        private int radius = 2;

        private float r = 106.0f;
        private float g = 90.0f;
        private float b = 205.0f;

        public int Radius
        {
            get { return radius; }
            set
            {
                if (radius <= 0)
                    radius = 1;
                else
                    radius = value;
            }
        }

        public int Accuracy
        {
            get { return accuracy; }
            set
            {
                if (value <= 2)
                    accuracy = 2;
                else
                    accuracy = value;
            }
        }

        private float X
        {
            get { return x; }
            set
            {
                if (value > 360.0f)
                    x = 0.0f;
                else if (value < 0.0f)
                    x = 360.0f;
                else
                    x = value;
            }
        }

        private float Y
        {
            get { return y; }
            set
            {
                if (value > 360.0f)
                    y = 0.0f;
                else if (value < 0.0f)
                    y = 360.0f;
                else
                    y = value;
            }
        }

        public Plot(int w, int h)
        {
            windowsforms = new GameWindow(w, h, GraphicsMode.Default, "Даниленко, Шар");

            windowsforms.Load += Window_Load;
            windowsforms.Resize += Window_Resize;
            windowsforms.RenderFrame += Window_RenderFrame;
            windowsforms.UpdateFrame += Window_UpdateFrame;
            windowsforms.KeyDown += Window_KeyDown;

            windowsforms.Run(1.0 / 60.0);
        }

        private void Window_Load(object sender, EventArgs e)
        {
            GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);
        }

        private void Window_Resize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, windowsforms.Width, windowsforms.Height);
            GL.MatrixMode(MatrixMode.Projection);

            GL.LoadIdentity();

            Matrix4 matrix = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, windowsforms.Width / windowsforms.Height, 1.0f, 100.0f);
            GL.LoadMatrix(ref matrix);
            GL.MatrixMode(MatrixMode.Modelview);
        }

        private void Window_UpdateFrame(object sender, FrameEventArgs e)
        {
            p0.Clear();
            p1.Clear();
            p2.Clear();
            p3.Clear();

            float endPhi = (float)Math.PI * 2.0f;
            float endTheta = (float)Math.PI;
            float dPhi = endPhi / accuracy;
            float dTheta = endTheta / accuracy;

            for (var pointPhi = 0; pointPhi < accuracy; pointPhi++)
            {
                for (var pointTheta = 0; pointTheta < accuracy; pointTheta++)
                {
                    float phi = pointPhi * dPhi;
                    float theta = pointTheta * dTheta;
                    float phit = (pointPhi + 1 == accuracy) ? endPhi : (pointPhi + 1) * dPhi;
                    float thetat = (pointTheta + 1 == accuracy) ? endTheta : (pointTheta + 1) * dTheta;

                    p0.Add(new float[] { radius * (float)Math.Sin(theta) * (float)Math.Cos(phi), radius * (float)Math.Sin(theta) * (float)Math.Sin(phi), radius * (float)Math.Cos(theta) });
                    p1.Add(new float[] { radius * (float)Math.Sin(thetat) * (float)Math.Cos(phi), radius * (float)Math.Sin(thetat) * (float)Math.Sin(phi), radius * (float)Math.Cos(thetat) });
                    p2.Add(new float[] { radius * (float)Math.Sin(theta) * (float)Math.Cos(phit), radius * (float)Math.Sin(theta) * (float)Math.Sin(phit), radius * (float)Math.Cos(theta) });
                    p3.Add(new float[] { radius * (float)Math.Sin(thetat) * (float)Math.Cos(phit), radius * (float)Math.Sin(thetat) * (float)Math.Sin(phit), radius * (float)Math.Cos(thetat) });
                }
            }
        }

        private void Window_KeyDown(object sender, KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.Plus)
            {
                view -= 0.5f;
            }
            else if (e.Key == Key.Minus)
            {
                view += 0.5f;
            }

            if (e.Key == Key.Down)
            {
                X += 5.0f;
            }
            else if (e.Key == Key.Up)
            {
                X -= 5.0f;
            }
            else if (e.Key == Key.Right)
            {
                Y += 5.0f;
            }
            else if (e.Key == Key.Left)
            {
                Y -= 5.0f;
            }

            if (e.Key == Key.M)
            {
                Accuracy += 1;
            }
            else if (e.Key == Key.N)
            {
                Accuracy -= 1;
            }

            if (e.Key == Key.V)
            {
                lp[0] -= 5.0f;
            }
            else if (e.Key == Key.B)
            {
                lp[0] += 5.0f;
            }

            if (e.Key == Key.L)
            {
                if (GL.IsEnabled(EnableCap.Light0))
                {
                    GL.Disable(EnableCap.Light0);
                }
                else
                {
                    GL.Enable(EnableCap.Light0);
                }
            }
        }

        private void Window_RenderFrame(object sender, FrameEventArgs e)
        {
            GL.LoadIdentity();
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Translate(0.0, 0.0, -view);
            GL.Rotate(x, 1.0, 0.0, 0.0);
            GL.Rotate(y, 0.0, 1.0, 0.0);

            for(int i = 0; i < p0.Count; i++)
            {
                GL.Begin(PrimitiveType.Triangles);

                GL.Normal3(p0[i][0] / radius, p0[i][1] / radius, p0[i][2] / radius);
                GL.Vertex3(p0[i][0], p0[i][1], p0[i][2]);
                GL.Normal3(p2[i][0] / radius, p2[i][1] / radius, p2[i][2] / radius);
                GL.Vertex3(p2[i][0], p2[i][1], p2[i][2]);
                GL.Normal3(p1[i][0] / radius, p1[i][1] / radius, p1[i][2] / radius);
                GL.Vertex3(p1[i][0], p1[i][1], p1[i][2]);

                GL.Normal3(p3[i][0] / radius, p3[i][1] / radius, p3[i][2] / radius);
                GL.Vertex3(p3[i][0], p3[i][1], p3[i][2]);
                GL.Normal3(p1[i][0] / radius, p1[i][1] / radius, p1[i][2] / radius);
                GL.Vertex3(p1[i][0], p1[i][1], p1[i][2]);
                GL.Normal3(p2[i][0] / radius, p2[i][1] / radius, p2[i][2] / radius);
                GL.Vertex3(p2[i][0], p2[i][1], p2[i][2]);

                GL.Color3(r / 256.0f, g / 256.0f, b / 256.0f);

                GL.End();
            }

            LightOn(r / 256.0f, g / 256.0f, b / 256.0f);

            windowsforms.SwapBuffers();
        }

        private void LightOn(float r, float g, float b)
        {
            Color4 ld = new Color4(r, g, b, 1.0f);

            GL.Light(LightName.Light0, LightParameter.Position, lp);
            GL.Light(LightName.Light0, LightParameter.Diffuse, ld);
            GL.Light(LightName.Light0, LightParameter.Ambient, ld);
        }
    }

    class MainClass
    {
        public static void Main(string[] args)
        {
            var window = new Plot(400, 400);
        }
    }
}
