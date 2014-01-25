using System;
using Utilities.Math;
using Utilities.Math.Shape;
using gamejam2014;
using V2 = Microsoft.Xna.Framework.Vector2;

namespace gamejam2014.Jousting
{
    /// <summary>
    /// Represents a player.
    /// </summary>
    public class Jouster : MovementPhysics
    {
        private static JousterPhysicsData PhysData { get { return KarmaWorld.World.PhysicsData; } }


        public class CollisionData
        {
            public float StabStrength1, StabStrength2;
        }
        /// <summary>
        /// Checks for collision between the two given jousters and responds accordingly.
        /// Returns the collision data for this collision, or 'null' if no collision occurred.
        /// </summary>
        public static CollisionData CheckCollision(Jouster first, Jouster second)
        {
            //If the players aren't moving fast enough relative to each other,
            //    exit.
            if ((first.Velocity - second.Velocity).LengthSquared() <
                PhysicsData.GetMinHitSpeed(KarmaWorld.World.CurrentZoom) *
                  PhysicsData.GetMinHitSpeed(KarmaWorld.World.CurrentZoom) ||
                !first.ColShape.Touches(second.ColShape))
                return null;

            CollisionData col = new CollisionData();

            //Get the component of the jousters' velocities that is along their forward movement.
            //Use only this component in figuring out stab strength.
            V2 lookDir1 = UsefulMath.FindDirection(first.Rotation),
               lookDir2 = UsefulMath.FindDirection(second.Rotation),
               moveDir1 = first.Velocity,
               moveDir2 = second.Velocity;

            col.StabStrength1 = V2.Dot(lookDir1, moveDir1);
            col.StabStrength2 = V2.Dot(lookDir2, moveDir2);

            //If neither player really did damage, exit.
            if (col.StabStrength1 <= 0.0f && col.StabStrength2 <= 0.0f) return null;

            return col;
        }


        public Jousters ThisJouster { get; private set; }

        public class BounceEventArgs : EventArgs
        {
            public enum BounceSide
            {
                Left,
                Right,
                Top,
                Bottom,
            }
            public BounceSide Side;
            public BounceEventArgs(BounceSide side) { Side = side; }
        }
        public event EventHandler<BounceEventArgs> OnWallBounce;


        public Jouster(Jousters thisJouster, V2 pos)
            : base(new Polygon(Microsoft.Xna.Framework.Graphics.CullMode.CullClockwiseFace,
                   ArtAssets.GetJousterPolygon(KarmaWorld.World.CurrentZoom).ToArray()),
                   PhysData.Acceleration, PhysData.MaxSpeed)
        {
            ThisJouster = thisJouster;
        }

        protected override V2 ConstrainPosition(V2 input)
        {
            //Check for bounding around the level.
            Rectangle bounds = KarmaWorld.World.WorldBounds;
            if (input.X < bounds.Left)
            {
                input.X = bounds.Left;
                if (Velocity.X < 0.0f)
                {
                    Velocity = new V2(-Velocity.X * PhysData.EdgeBounceEnergyScale, Velocity.Y);
                }

                if (OnWallBounce != null)
                    OnWallBounce(this, new BounceEventArgs(BounceEventArgs.BounceSide.Left));
            }
            if (input.Y < bounds.Top)
            {
                input.Y = bounds.Top;
                if (Velocity.Y < 0.0f)
                {
                    Velocity = new V2(Velocity.X, -Velocity.Y * PhysData.EdgeBounceEnergyScale);
                }

                if (OnWallBounce != null)
                    OnWallBounce(this, new BounceEventArgs(BounceEventArgs.BounceSide.Top));
            }
            if (input.X > bounds.Right)
            {
                input.X = bounds.Right;
                if (Velocity.X > 0.0f)
                {
                    Velocity = new V2(-Velocity.X * PhysData.EdgeBounceEnergyScale, Velocity.Y);
                }

                if (OnWallBounce != null)
                    OnWallBounce(this, new BounceEventArgs(BounceEventArgs.BounceSide.Right));
            }
            if (input.Y > bounds.Bottom)
            {
                input.Y = bounds.Bottom;
                if (Velocity.Y > 0.0f)
                {
                    Velocity = new V2(Velocity.X, -Velocity.Y * PhysData.EdgeBounceEnergyScale);
                }

                if (OnWallBounce != null)
                    OnWallBounce(this, new BounceEventArgs(BounceEventArgs.BounceSide.Bottom));
            }

            return input;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gt)
        {
            //Keep physics up-to-date.
            MaxVelocity = PhysData.MaxSpeed;
            MaxRotSpeed = PhysData.TurnSpeed;
            MaxAcceleration = PhysData.Acceleration;


            //Create friction.
            //If the friction this frame is strong enough to stop the player, manually stop him.
            float fricPerFrame = PhysData.Friction * (float)gt.ElapsedGameTime.TotalSeconds;
            if (fricPerFrame * fricPerFrame >= Velocity.LengthSquared())
            {
                Velocity = V2.Zero;
            }
            //Otherwise, apply friction like normal.
            else
            {
                V2 vel = Velocity;
                vel.Normalize();
                Acceleration = -vel * PhysData.Friction;
            }


            //Update input.
            V2 movement = JoustingInput.GetMovement(ThisJouster);
            RotVelocity = movement.X;
            if (movement.Y > 0.0f)
            {
                Acceleration += UsefulMath.FindDirection(Rotation) * MaxAcceleration;
            }
            else if (movement.Y < 0.0f)
            {
                Acceleration += UsefulMath.FindDirection(Rotation) * -MaxAcceleration;
            }


            //Now update movement physics.
            base.Update(gt);
        }
    }
}
