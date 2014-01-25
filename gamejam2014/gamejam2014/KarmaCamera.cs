using System;
using Utilities.Camera;
using V2 = Microsoft.Xna.Framework.Vector2;
using Utilities.Math;

namespace gamejam2014
{
    /// <summary>
    /// The camera for this game.
    /// </summary>
    public class KarmaCamera : Camera
    {
        public V2 CamTarget { get { return entity.CameraTarget; } set { entity.CameraTarget = value; } }
        public V2 Position { get { return entity.Pos; } set { entity.Pos = value; } }

        private class NoMoveEntity : Utilities.PositionalEntity { public V2 CameraTarget { get; set; } public V2 Pos { get; set; } }
        private static NoMoveEntity entity = new NoMoveEntity(); //Making this static because there will only be one camera, and otherwise I can't pass it to the base constructor.

        public KarmaCamera(Microsoft.Xna.Framework.Graphics.GraphicsDevice gd)
            : base(entity, gd, CameraZoomData.DefaultData, CameraShakeData.DefaultData, CameraPhysicsData.DefaultValues(gd.PresentationParameters.BackBufferWidth, gd.PresentationParameters.BackBufferHeight),
                   new MovementPhysics(new Utilities.Math.Shape.Circle(V2.Zero, 0.01f), 0, 0))
        {
            ZoomData.MaxZoom = Single.MaxValue;
            ZoomData.MinZoom = Single.Epsilon;
        }
    }
}