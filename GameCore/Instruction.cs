using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameCore
{
    public class Instruction
    {
        public Instruction(int pmID)
        {
            instructionID = pmID;
            instructionParametersArray = new int[10];
        }

        #region declaration
        public int instructionID = 0;
        public InstructionType type = InstructionType.InstructionType_None;

        public int[] instructionParametersArray;
        #endregion

        #region business

        #endregion
    }

    public enum InstructionType
    {
        InstructionType_None,
        InstructionType_ScreenGettingDark,
        InstructionType_ScreenGettingBright
    }
}