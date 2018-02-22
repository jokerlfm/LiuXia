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
        #endregion

        #region business
        public string GetInputName()
        {
            try
            {
                if (mainKB.Acquire().IsFailure)
                    return "";

                if (mainKB.Poll().IsFailure)
                    return "";

                KeyboardState state = mainKB.GetCurrentState();
                if (Result.Last.IsFailure)
                    return "";

                if (state.PressedKeys.Count == 0)
                {
                    return "";
                }

                return state.PressedKeys[0].ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }
        #endregion
    }
}