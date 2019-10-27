﻿// RandUtilities.cs modified by Iron Wolf for Pawnmorph on 08/27/2019 7:03 PM
// last updated 08/27/2019  7:03 PM

using Multiplayer.API;
using Verse;

namespace Pawnmorph.Utilities
{
    public static class RandUtilities
    {
        private static int _lastTick;
        private static int _lastSeed;

        public static int MPSafeSeed
        {
            get
            {
                var ticks = Find.TickManager.TicksAbs;
                if (ticks != _lastTick)
                {
                    _lastTick = ticks;
                    _lastSeed = ticks;
                    return _lastTick;
                }

                _lastSeed = ZorShift(_lastSeed);
                return _lastSeed;
            }
        }

        /// <summary>
        /// If the game is in multiplayer pushes a deterministic seed to Rand. <br />
        /// If not in multiplayer this call does nothing .
        /// </summary>
        public static void PushState()
        {
            if (MP.IsInMultiplayer)
            {
                Rand.PushState(MPSafeSeed);

            }
        }

        /// <summary>
        /// Pops the Rand state if the game is in multiplayer. <br />
        /// This does nothing if the game is not in multiplayer.
        /// </summary>
        public static void PopState()
        {
            if (MP.IsInMultiplayer)
            {
                Rand.PopState();
            }
        }


        /// <summary> Multiplayer save version of Rand.MTBEventOccurs  </summary>
        /// <param name="mtb"> The MTB. </param>
        /// <param name="mtbUnit"> The MTB unit. </param>
        /// <param name="checkDuration"> Duration of the check. </param>
        public static bool MPSafeMTBEventOccurs(float mtb, float mtbUnit, float checkDuration)
        {
            PushState();

            var res = Rand.MTBEventOccurs(mtb, mtbUnit, checkDuration);

            PopState();

            return res;
        }

        /// <summary> Preform a zorShift on the given int value. </summary>
        /// <param name="val"> The value. </param>
        static int ZorShift(int val)
        {
            uint uVal = unchecked((uint) val); // Just copy the bit pattern. 
            uVal ^= uVal << 13;
            uVal ^= uVal >> 17;
            uVal ^= uVal << 5;
            return unchecked((int) uVal); // Return the shuffled bit pattern.
        }
    }
}