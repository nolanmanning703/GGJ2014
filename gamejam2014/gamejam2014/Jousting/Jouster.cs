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
        public static JousterPhysicsData PhysData { get { return KarmaWorld.World.PhysicsData; } }


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
            //If the players aren't touching, exit.
            if (!first.ColShape.Touches(second.ColShape))
            {
                first.touchingOtherJouster = false;
                second.touchingOtherJouster = false;
                return null;
            }
            if (first.touchingOtherJouster || second.touchingOtherJouster) return null;
            //If the players aren't moving fast enough relative to each other,
            //    exit.
            if ((first.Velocity - second.Velocity).LengthSquared() <
                PhysicsData.GetMinHitSpeed(KarmaWorld.World.CurrentZoom) *
                  PhysicsData.GetMinHitSpeed(KarmaWorld.World.CurrentZoom))
            {
                return null;
            }

            CollisionData col = new CollisionData();

            //Get the component of the jousters' velocities that is along their forward movement.
            //Use only this component in figuring out stab strength.
            V2 lookDir1 = UsefulMath.FindDirection(first.Rotation),
               lookDir2 = UsefulMath.FindDirection(second.Rotation),
               moveDir1 = first.Velocity,
               moveDir2 = second.Velocity;

            col.StabStrength1 = first.Mass * V2.Dot(lookDir1, moveDir1);
            col.StabStrength2 = second.Mass * V2.Dot(lookDir2, moveDir2);

            if (first.IsSpiky) col.StabStrength1 *= Minigames.Minigame_1.PhysicsData1.SpikePowerScale;
            if (second.IsSpiky) col.StabStrength2 *= Minigames.Minigame_1.PhysicsData1.SpikePowerScale;

            //If neither player really did damage, exit.
            if (col.StabStrength1 <= 0.0f && col.StabStrength2 <= 0.0f) return null;

            first.touchingOtherJouster = true;
            second.touchingOtherJouster = true;

            if (col.StabStrength1 > col.StabStrength2)
            {
                first.Hurt(second, col.StabStrength1);
                second.HurtBy(first, col.StabStrength1);
            }
            else
            {
                first.HurtBy(second, col.StabStrength2);
                second.Hurt(first, col.StabStrength2);
            }

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

        private bool touchingOtherJouster = false;

        public class HurtEventArgs : EventArgs
        {
            public Jouster Enemy;
            public float DamageDealt;
            public HurtEventArgs(Jouster enemy, float damageDealt) { Enemy = enemy; DamageDealt = damageDealt; }
        }
        /// <summary>
        /// Raised when this jouster is hurt by the other player.
        /// </summary>
        public event EventHandler<HurtEventArgs> OnHurtByEnemy;
        /// <summary>
        /// Raised when this jouster hurts the other player.
        /// </summary>
        public event EventHandler<HurtEventArgs> OnHurtEnemy;

        public float Mass;
        public float Health = 1.0f;

        //Abilities.
        public bool IsSpiky_Aura = false;
        public bool IsStunned = false;
        public bool IsSpiky { get { return IsSpiky_Aura && ThisJouster == Jousters.Harmony; } }
        public bool IsAura { get { return IsSpiky_Aura && ThisJouster == Jousters.Dischord; } }


        public Jouster(Jousters thisJouster, V2 pos, ZoomLevels zoom)
            : base(ArtAssets.GetJousterShape(KarmaWorld.World.CurrentZoom, WorldData.ZoomScaleAmount[zoom]),
                   Single.PositiveInfinity, PhysData.MaxSpeed)
        {
            Pos = pos;
            ThisJouster = thisJouster;
            Mass = PhysicsData.JousterStartingMass[zoom];
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
                    Velocity = new V2(-Velocity.X * PhysData.BounceEnergyScale, Velocity.Y);
                }

                if (OnWallBounce != null)
                    OnWallBounce(this, new BounceEventArgs(BounceEventArgs.BounceSide.Left));
            }
            if (input.Y < bounds.Top)
            {
                input.Y = bounds.Top;
                if (Velocity.Y < 0.0f)
                {
                    Velocity = new V2(Velocity.X, -Velocity.Y * PhysData.BounceEnergyScale);
                }

                if (OnWallBounce != null)
                    OnWallBounce(this, new BounceEventArgs(BounceEventArgs.BounceSide.Top));
            }
            if (input.X > bounds.Right)
            {
                input.X = bounds.Right;
                if (Velocity.X > 0.0f)
                {
                    Velocity = new V2(-Velocity.X * PhysData.BounceEnergyScale, Velocity.Y);
                }

                if (OnWallBounce != null)
                    OnWallBounce(this, new BounceEventArgs(BounceEventArgs.BounceSide.Right));
            }
            if (input.Y > bounds.Bottom)
            {
                input.Y = bounds.Bottom;
                if (Velocity.Y > 0.0f)
                {
                    Velocity = new V2(Velocity.X, -Velocity.Y * PhysData.BounceEnergyScale);
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
                Acceleration += -vel * PhysData.Friction;
            }


            //Update input.
            if (!IsStunned)
            {
                V2 movement = JoustingInput.GetMovement(ThisJouster);
                RotVelocity = PhysData.TurnSpeed * movement.X;
                if (movement.Y > 0.0f)
                {
                    Acceleration += UsefulMath.FindDirection(Rotation) * -PhysData.Acceleration;
                }
                else if (movement.Y < 0.0f)
                {
                    Acceleration += UsefulMath.FindDirection(Rotation) * PhysData.Acceleration;
                }
            }


            //Now update movement physics.
            Acceleration /= Mass;
            base.Update(gt);
            Acceleration = V2.Zero;
        }

        public void HurtBy(Jouster other, float stabDamage)
        {
            Health -= PhysicsData.GetDamage(stabDamage, WorldData.ZoomScaleAmount[KarmaWorld.World.CurrentZoom]);
            Velocity += PhysData.BounceEnergyScale * PhysicsData.VelocityFromHit(stabDamage, UsefulMath.FindDirection(other.Rotation));
            if (OnHurtByEnemy != null) OnHurtByEnemy(this, new HurtEventArgs(other, stabDamage));
        }
        public void Hurt(Jouster other, float stabDamage)
        {
            Velocity *= PhysData.BounceEnergyScale * PhysicsData.VelocityDampFromHit(stabDamage, MaxVelocity);
            if (OnHurtEnemy != null) OnHurtEnemy(this, new HurtEventArgs(other, stabDamage));
        }
    }
}
