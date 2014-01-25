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
        /// <summary>
        /// Whether or not this thing moves.
        /// </summary>
        public bool IsMovable { get { return MaxVelocity > 0.0f; } }

        public float Mass;

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

        public Utilities.Graphics.AnimatedSprite Sprite;

        public Blocker(Utilities.Graphics.AnimatedSprite sprite, Shape colShape, float maxSpeed = 0.0f, float mass = 1.0f)
            : base(colShape, Single.PositiveInfinity, maxSpeed)
        {
            Sprite = sprite;
            Mass = mass;
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

        public override void Update(Microsoft.Xna.Framework.GameTime gt)
        {
            Acceleration /= Mass;
            base.Update(gt);
        }

        public void OnHitJouster(Jouster joust, float stabForce)
        {
            if (OnHitByJouster != null) OnHitByJouster(this, new Jouster.HurtEventArgs(joust, stabForce));

            if (IsMovable)
            {
                //TODO: Fix.
                //Dull the jouster's momentum, and push this Blocker forward.
                V2 newVel = joust.Velocity * KarmaWorld.World.PhysicsData.BounceEnergyScale * PhysicsData.VelocityDampFromHit(stabForce, joust.MaxVelocity);
                V2 delta = joust.Velocity - newVel;
                joust.Velocity = newVel;
                Velocity += delta;
            }
            else
            {
                //Push the jouster away from this Blocker.
                V2 away = UsefulMath.FindDirection(Pos, joust.Pos);
                Utilities.Conversions.ParallelPerp pp = Utilities.Conversions.SplitIntoComponents(joust.Velocity, away);
                joust.Velocity = pp.Perpendicular + (KarmaWorld.World.PhysicsData.BounceEnergyScale * -pp.Parallel);
            }
        }
        public void OnHitBlocker(Blocker other)
        {
            //No hitting if both blockers don't move.
            if (!IsMovable && !other.IsMovable) return;

            //Raise collision events.
            if (OnHitByBlocker != null) OnHitByBlocker(this, new HitBlockerEventArgs(other));
            if (other.OnHitByBlocker != null) other.OnHitByBlocker(this, new HitBlockerEventArgs(this));

            //If both blockers are movable, react accordingly.
            if (IsMovable && other.IsMovable)
            {
                //Define "tangent" as the line perpendicular to the line from one Blocker to the other.
                //Both entities keep the portion of momentum parallel to their tangent,
                //But swap the portion of momentum perpendicular to their tangent.

                V2 toOther = other.Pos - Pos;
                V2 tangent = V2.Normalize(Utilities.Conversions.GetPerp(toOther));

                Utilities.Conversions.ParallelPerp thisPP = Utilities.Conversions.SplitIntoComponents(Velocity, tangent),
                                                   otherPP = Utilities.Conversions.SplitIntoComponents(other.Velocity, tangent);

                Velocity = thisPP.Parallel + (BounceVelocityDamp * otherPP.Perpendicular * other.Mass / Mass);
                other.Velocity = otherPP.Parallel + (other.BounceVelocityDamp * thisPP.Perpendicular * Mass / other.Mass);
            }
            //Otherwise, one will bounce off.
            else
            {
                Blocker movable = (IsMovable ? this : other);
                Blocker nonMovable = (movable == this ? other : this);

                V2 away = UsefulMath.FindDirection(nonMovable.Pos, movable.Pos);
                Utilities.Conversions.ParallelPerp pp = Utilities.Conversions.SplitIntoComponents(movable.Velocity, away);
                movable.Velocity = pp.Perpendicular + (movable.BounceVelocityDamp * -pp.Parallel);
            }
        }
    }
}
