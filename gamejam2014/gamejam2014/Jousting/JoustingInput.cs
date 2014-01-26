using System;
using System.Collections.Generic;
using Utilities.Input;
using Utilities.Input.Buttons;
using V2 = Microsoft.Xna.Framework.Vector2;
using KeyboardKeys = Microsoft.Xna.Framework.Input.Keys;

namespace gamejam2014.Jousting
{
    public enum Jousters
    {
        Harmony,
        Dischord,
    }

    public class VectorSumButton : Button<V2>
    {
        public List<Button<V2>> ToSum;

        public VectorSumButton(params Button<V2>[] butts)
            : base(V2.Zero)
        {
            ToSum = new List<Button<V2>>(butts);
        }
        public override void Update(Microsoft.Xna.Framework.Input.KeyboardState ks, Microsoft.Xna.Framework.Input.MouseState ms, Microsoft.Xna.Framework.Input.GamePadState gps, Microsoft.Xna.Framework.GameTime gt)
        {
            Value = V2.Zero;
            foreach (Button<V2> butt in ToSum)
            {
                butt.Update(ks, ms, gps, gt);
                Value += butt.Value;
            }
        }
    }
    public class FourBoolButtonButton : Button<V2>
    {
        public Button<bool> Left, Right, Top, Bottom;
        public FourBoolButtonButton(Button<bool> left, Button<bool> right, Button<bool> top, Button<bool> bottom)
            : base(V2.Zero)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }
        public override void Update(Microsoft.Xna.Framework.Input.KeyboardState ks, Microsoft.Xna.Framework.Input.MouseState ms, Microsoft.Xna.Framework.Input.GamePadState gps, Microsoft.Xna.Framework.GameTime gt)
        {
            Value = V2.Zero;
            Left.Update(ks, ms, gps, gt);
            Right.Update(ks, ms, gps, gt);
            Top.Update(ks, ms, gps, gt);
            Bottom.Update(ks, ms, gps, gt);
            if (Left.Value) Value += new V2(-1.0f, 0.0f);
            if (Right.Value) Value += new V2(1.0f, 0.0f);
            if (Top.Value) Value += new V2(0.0f, 1.0f);
            if (Bottom.Value) Value += new V2(0.0f, -1.0f);
        }
    }

    /// <summary>
    /// Sets up and reads input for both jousters.
    /// </summary>
    public class JoustingInput
    {
        private static ButtonInputManager Input { get { return KarmaWorld.World.Input; } }

        private static Dictionary<Jousters, string> PlayerToString = new Dictionary<Jousters,string>()
        {
            { Jousters.Harmony, "Harmony " },
            { Jousters.Dischord, "Chaos " },
        };


        public static void InitializeInput()
        {
            Input.AddInput(PlayerToString[Jousters.Harmony] + "Movement",
                           new VectorSumButton(new FourArrowButton(KeyboardKeys.S, KeyboardKeys.W, KeyboardKeys.A, KeyboardKeys.D),
                                               new FourBoolButtonButton(new GamePadButton(Microsoft.Xna.Framework.Input.Buttons.DPadUp, false),
                                                                        new GamePadButton(Microsoft.Xna.Framework.Input.Buttons.DPadDown, false),
                                                                        new GamePadButton(Microsoft.Xna.Framework.Input.Buttons.DPadRight, false),
                                                                        new GamePadButton(Microsoft.Xna.Framework.Input.Buttons.DPadLeft, false))));
            Input.AddInput(PlayerToString[Jousters.Dischord] + "Movement",
                           new VectorSumButton(new FourArrowButton(KeyboardKeys.Down, KeyboardKeys.Up, KeyboardKeys.Left, KeyboardKeys.Right),
                                               new FourBoolButtonButton(new GamePadButton(Microsoft.Xna.Framework.Input.Buttons.Y, false),
                                                                        new GamePadButton(Microsoft.Xna.Framework.Input.Buttons.A, false),
                                                                        new GamePadButton(Microsoft.Xna.Framework.Input.Buttons.X, false),
                                                                        new GamePadButton(Microsoft.Xna.Framework.Input.Buttons.B, false))));

            Input.AddInput(PlayerToString[Jousters.Harmony] + "Special",
                           new MultiButton(new KeyboardButton(KeyboardKeys.End),
                                           new GamePadButton(Microsoft.Xna.Framework.Input.Buttons.RightShoulder, false)));
            Input.AddInput(PlayerToString[Jousters.Dischord] + "Special",
                           new MultiButton(new KeyboardButton(KeyboardKeys.Q),
                                           new GamePadButton(Microsoft.Xna.Framework.Input.Buttons.LeftShoulder, false)));

            Input.AddInput(PlayerToString[Jousters.Harmony] + "Hold Special",
                           new MultiButton(new KeyboardButton(KeyboardKeys.LeftShift),
                                           new GamePadButton(Microsoft.Xna.Framework.Input.Buttons.LeftTrigger, false)));
            Input.AddInput(PlayerToString[Jousters.Dischord] + "Hold Special",
                           new MultiButton(new KeyboardButton(KeyboardKeys.RightShift),
                                           new GamePadButton(Microsoft.Xna.Framework.Input.Buttons.RightTrigger, false)));
        }

        public static Microsoft.Xna.Framework.Vector2 GetMovement(Jousters player)
        {
            return Input.GetVectorInput(PlayerToString[player] + "Movement").Value;
        }
        public static bool IsPressingSpecial(Jousters player)
        {
            return Input.GetBoolInput(PlayerToString[player] + "Special").Value;
        }
        public static bool IsPressingHoldSpecial(Jousters player)
        {
            return Input.GetBoolInput(PlayerToString[player] + "Hold Special").Value;
        }
    }
}
