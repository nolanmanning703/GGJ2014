using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gamejam2014.Jousting
{
    /// <summary>
    /// Physics data for jousters.
    /// </summary>
    public class JousterPhysicsData
    {
        public ZoomLevels ZoomLevel;


        public float MaxSpeed
        {
            get
            {
                return WorldData.ZoomScaleAmount[ZoomLevel] * maxSpeed;
            }
            set { maxSpeed = value; }
        }
        private float maxSpeed;

        public float Acceleration
        {
            get
            {
                return WorldData.ZoomScaleAmount[ZoomLevel] * acceleration;
            }
            set { acceleration = value; }
        }
        private float acceleration;

        public float TurnSpeed
        {
            get
            {
                return turnSpeed;
            }
            set { turnSpeed = value; }
        }
        private float turnSpeed;

        public float Friction
        {
            get
            {
                return WorldData.ZoomScaleAmount[ZoomLevel] * friction;
            }
            set { friction = value; }
        }
        private float friction;

        public float BounceEnergyScale
        {
            get
            {
                return bounceEnergyScale;
            }
        }
        private float bounceEnergyScale;


        public JousterPhysicsData(ZoomLevels zoom, float maxSpeed, float turnSpd, float accel, float friction, float bounceEnergyScale)
        {
            ZoomLevel = zoom;

            this.maxSpeed = maxSpeed;
            acceleration = accel;
            this.friction = friction;
            turnSpeed = turnSpd;
            this.bounceEnergyScale = bounceEnergyScale;
        }
    }
}
