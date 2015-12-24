using EmnityDX.Engine;
using EmnityDX.Objects.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EmnityDX.Engine
{
    public class Camera
    {
        public float Rotation { get; set; }
        public Vector3 Scale { get; set; }
        public Vector2 Position { get; set;} 
        public Rectangle Bounds { get; set; }

        public Camera(Vector2 position, Vector2 origin, float rotation, Vector2 scale, GraphicsDeviceManager graphics)
        {
            ResetCamera(position, origin, rotation, scale, graphics);
        }

        public void ResetCamera(Vector2 position, Vector2 origin, float rotation, Vector2 scale, GraphicsDeviceManager graphics)
        {
            Rotation = rotation;
            Scale = ResetScreenScale(graphics, scale);
            Position = position;
            Bounds = graphics.GraphicsDevice.Viewport.Bounds;
        }

        public Vector3 ResetScreenScale(GraphicsDeviceManager graphics, Vector2 screenScale)
        {
            var scaleX = (float)graphics.GraphicsDevice.Viewport.Width / screenScale.X;
            var scaleY = (float)graphics.GraphicsDevice.Viewport.Height / screenScale.Y;
            return Scale = new Vector3(scaleX, scaleY, 1.0f);
        }

        public Matrix GetMatrix()
        {
                return
                    Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) *
                    Matrix.CreateRotationZ(Rotation) *
                    Matrix.CreateScale(Scale) *
                    Matrix.CreateTranslation(new Vector3(Bounds.Width * 0.5f, Bounds.Height * 0.5f, 0));
        }

        public Matrix GetInverseMatrix()
        {
            return
                Matrix.Invert(
                    Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) *
                    Matrix.CreateRotationZ(Rotation) *
                    Matrix.CreateScale(Scale) *
                    Matrix.CreateTranslation(new Vector3(Bounds.Width * 0.5f, Bounds.Height * 0.5f, 0)));
        }

        public Rectangle GetVisibleArea(GraphicsDeviceManager graphics)
        {
                var inverseViewMatrix = Matrix.Invert(GetMatrix());
                var tl = Vector2.Transform(Vector2.Zero, inverseViewMatrix);
                var tr = Vector2.Transform(new Vector2(graphics.GraphicsDevice.Viewport.X, 0), inverseViewMatrix);
                var bl = Vector2.Transform(new Vector2(0, graphics.GraphicsDevice.Viewport.Y), inverseViewMatrix);
                var br = Vector2.Transform(new Vector2(graphics.GraphicsDevice.Viewport.X, graphics.GraphicsDevice.Viewport.Y), inverseViewMatrix);
                var min = new Vector2(
                    MathHelper.Min(tl.X, MathHelper.Min(tr.X, MathHelper.Min(bl.X, br.X))),
                    MathHelper.Min(tl.Y, MathHelper.Min(tr.Y, MathHelper.Min(bl.Y, br.Y))));
                var max = new Vector2(
                    MathHelper.Max(tl.X, MathHelper.Max(tr.X, MathHelper.Max(bl.X, br.X))),
                    MathHelper.Max(tl.Y, MathHelper.Max(tr.Y, MathHelper.Max(bl.Y, br.Y))));
                return new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));
        }

    }
}
