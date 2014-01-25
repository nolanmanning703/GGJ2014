using System;
using System.Collections.Generic;
using Utilities.Math;
using Utilities.Math.Shape;
using V2 = Microsoft.Xna.Framework.Vector2;

namespace gamejam2014.Jousting
{
    /// <summary>
    /// An entity that participates in physics/collision but isn't a jouster.
    /// </summary>
    public class Blocker : MovementPhysics
    {
        public class BlockerCollisionData
        {
            public Blocker First, Second;
            public V2 FirstMomentum, SecondMomentum;
            public BlockerCollisionData(Blocker first, Blocker second, V2 firstM, V2 secondM) { First = first; Second = second; FirstMomentum = firstM; SecondMomentum = secondM; }
        }
        /// <summary>
        /// Processes a potential collision between two blockers.
        /// Returns either the data for the blockers' collision, or "null" if there was no collision.
        /// </summary>
        public static BlockerCollisionData CheckCollision(Blocker first, Blocker second)
        {
            if (!first.ColShape.Touches(second.ColShape)) return null;

            BlockerCollisionData dat = new BlockerCollisionData(first, second, first.Velocity, second.Velocity);

            first.OnHitBlocker(second);

            return dat;
        }
        /// <summary>
        /// Processes a potential collision between a blocker and a jouster.
        /// Returns either the force of the jouster's thrust at the blocker, or Single.NaN if there was no collision.
        /// </summary>
        public static float CheckCollision(Blocker block, Jouster joust)
        {
            if (!block.ColShape.Touches(joust.ColShape)) return Single.NaN;

            V2 lookDir = UsefulMath.FindDirection(joust.Rotation),
               moveDir = joust.Velocity;
            float stabStrength = V2.Dot(lookDir, moveDir);

            if (stabStrength <= 0.0f) return Single.NaN;

            block.OnHitJouster(joust, stabStrength);

            return stabStrength;
        }


        /// <summary>
        /// Velocity is scaled by this value after the blocker bounces off a wall or object.
        /// </summary>
        public float BounceVelocityDamp = 1.0f;

        public event EventHandler<Jousting.Jouster.BounceEventArgs> OnWallBounce;
        public event EventHandler<Jouster.HurtEventArgs> OnHitByJouster;

        public class HitBlockerEventArgs : EventArgs
        {
            public Blocker Other;
            public HitBlockerEventArgs(Blocker other) { Other = other; }
        }
        /// <summary>
        /// Unlike the other events, this is called BEFORE either blocker's velocity is affected by the collision.
        /// </summary>
        public event EventHandler<HitBlockerEventArgs> OnHitByBlocker;


        public Blocker(Shape colShape, float maxSpeed = Single.PositiveInfinity)
            : base(colShape, Single.PositiveInfinity, maxSpeed)
        {

        }

        protected override V2 ConstrainPosition(V2 input)
        {
            Rectangle bounds = KarmaWorld.World.WorldBounds;
            if (input.X < bounds.Left)
            {
                input.X = bounds.Left;
                if (Velocity.X < 0.0f)
                {
                    Velocity = new V2(-Velocity.X * BounceVelocityDamp, Velocity.Y);
                }

                if (OnWallBounce != null)
                    OnWallBounce(this, new Jousting.Jouster.BounceEventArgs(Jousting.Jouster.BounceEventArgs.BounceSide.Left));
            }
            if (input.Y < bounds.Top)
            {
                input.Y = bounds.Top;
                if (Velocity.Y < 0.0f)
                {
                    Velocity = new V2(Velocity.X, -Velocity.Y * BounceVelocityDamp);
                }

                if (OnWallBounce != null)
                    OnWallBounce(this, new Jousting.Jouster.BounceEventArgs(Jousting.Jouster.BounceEventArgs.BounceSide.Top));
            }
            if (input.X > bounds.Right)
            {
                input.X = bounds.Right;
                if (Velocity.X > 0.0f)
                {
                    Velocity = new V2(-Velocity.X * BounceVelocityDamp, Velocity.Y);
                }

                if (OnWallBounce != null)
                    OnWallBounce(this, new Jousting.Jouster.BounceEventArgs(Jousting.Jouster.BounceEventArgs.BounceSide.Right));
            }
            if (input.Y > bounds.Bottom)
            {
                input.Y = bounds.Bottom;
                if (Velocity.Y > 0.0f)
                {
                    Velocity = new V2(Velocity.X, -Velocity.Y * BounceVelocityDamp);
                }

                if (OnWallBounce != null)
                    OnWallBounce(this, new Jousting.Jouster.BounceEventArgs(Jousting.Jouster.BounceEventArgs.BounceSide.Bottom));
            }

            return input;
        }

        public void OnHitJouster(Jouster other, float stabForce)
        {
            if (OnHitByJouster != null) OnHitByJouster(this, new Jouster.HurtEventArgs(other, stabForce));

            //Dull the jouster's momentum, and push this one forward.
            V2 newVel = other.Velocity * KarmaWorld.World.PhysicsData.BounceEnergyScale * PhysicsData.VelocityDampFromHit(stabForce, other.MaxVelocity);
            V2 delta = other.Velocity - newVel;
            other.Velocity = newVel;
            Velocity += delta;

        }
        public void OnHitBlocker(Blocker other)
        {
            if (OnHitByBlocker != null) OnHitByBlocker(this, new HitBlockerEventArgs(other));
            if (other.OnHitByBlocker != null) other.OnHitByBlocker(this, new HitBlockerEventArgs(this));

            //Both entities keep the portion of momentum parallel to their tangent,
            //But swap the portion of momentum perpendicular to their tangent.

            V2 thisMomentum = Velocity,
               otherMomentum = other.Velocity;

            V2 toOther = other.Pos - Pos;
            V2 tangent = V2.Normalize(Utilities.Conversions.GetPerp(toOther));

            V2 thisMomentumParallel = tangent * V2.Dot(thisMomentum, tangent),
               otherMomentumParallel = tangent * V2.Dot(otherMomentum, tangent);
            V2 thisMomentumPerpendicular = thisMomentum - thisMomentumParallel,
               otherMomentumPerpendicular = otherMomentum - otherMomentumParallel;

            Velocity = thisMomentumParallel + (BounceVelocityDamp * otherMomentumPerpendicular);
            other.Velocity = otherMomentumParallel + (other.BounceVelocityDamp * thisMomentumPerpendicular);
        }
    }
}
