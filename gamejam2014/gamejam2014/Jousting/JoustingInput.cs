using System;
using System.Collections.Generic;
using Utilities.Input;
using Utilities.Input.Buttons;
using KeyboardKeys = Microsoft.Xna.Framework.Input.Keys;

namespace gamejam2014.Jousting
{
    public enum Jousters
    {
        Harmony,
        Dischord,
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
            Input.AddInput(PlayerToString[Jousters.Harmony] + "Movement", new FourArrowButton(KeyboardKeys.W, KeyboardKeys.S, KeyboardKeys.A, KeyboardKeys.D));
            Input.AddInput(PlayerToString[Jousters.Dischord] + "Movement", new FourArrowButton(KeyboardKeys.Up, KeyboardKeys.Down, KeyboardKeys.Left, KeyboardKeys.Right));

            Input.AddInput(PlayerToString[Jousters.Harmony] + "Special", new KeyboardButton(KeyboardKeys.End));
            Input.AddInput(PlayerToString[Jousters.Dischord] + "Special", new KeyboardButton(KeyboardKeys.Q));

            Input.AddInput(PlayerToString[Jousters.Harmony] + "Hold Special", new KeyboardButton(KeyboardKeys.LeftShift));
            Input.AddInput(PlayerToString[Jousters.Dischord] + "Hold Special", new KeyboardButton(KeyboardKeys.RightShift));
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
