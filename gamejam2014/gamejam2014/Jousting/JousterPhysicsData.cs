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

        public float FrictionScale
        {
            get
            {
                return frictionScale;
            }
            set { frictionScale = value; }
        }
        private float frictionScale;

        public float EdgeBounceEnergyScale
        {
            get
            {
                return edgeBounceEnergyScale;
            }
        }
        private float edgeBounceEnergyScale;


        public JousterPhysicsData(ZoomLevels zoom, float maxSpeed, float turnSpd, float accel, float frictionScale, float edgeBounceEnergyScale)
        {
            ZoomLevel = zoom;

            this.maxSpeed = maxSpeed;
            acceleration = accel;
            this.frictionScale = frictionScale;
            turnSpeed = turnSpd;
            this.edgeBounceEnergyScale = edgeBounceEnergyScale;
        }
    }
}
