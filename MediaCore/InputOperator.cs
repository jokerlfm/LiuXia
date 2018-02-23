using SlimDX;
using SlimDX.Direct3D9;
using SlimDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace MediaCore
{
    public class InputOperator
    {
        public InputOperator(System.Windows.Forms.Control pmTargetControl)
        {
            DirectInput dinput = new DirectInput();
            this.mainKB = new Keyboard(dinput);
            CooperativeLevel cl = CooperativeLevel.Exclusive;
            cl |= CooperativeLevel.Foreground;
            cl |= CooperativeLevel.NoWinKey;
            this.mainKB.SetCooperativeLevel(pmTargetControl, cl);
            mainKB.Properties.BufferSize = 8;
            mainKB.Acquire();
        }

        #region declaratoin        
        Keyboard mainKB;
        Key pressingKey = Key.Z;
        Key releasedKey = Key.Z;
        #endregion

        #region business
        public bool GetKeyName(out string pmPressingKey, out string pmReleasedKey)
        {
            pmPressingKey = "";
            pmReleasedKey = "";
            try
            {
                if (mainKB.Acquire().IsFailure)
                    return false;

                if (mainKB.Poll().IsFailure)
                    return false;

                KeyboardState state = mainKB.GetCurrentState();
                if (Result.Last.IsFailure)
                    return false;

                if (state.PressedKeys.Count == 0)
                {
                    releasedKey = pressingKey;
                    pressingKey = Key.Z;
                }
                else
                {
                    releasedKey = Key.Z;
                    pressingKey = state.PressedKeys[0];
                }

                pmReleasedKey = releasedKey.ToString();
                pmPressingKey = pressingKey.ToString();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion
    }
}