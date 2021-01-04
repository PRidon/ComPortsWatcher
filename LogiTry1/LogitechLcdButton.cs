using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogiTry1
{
    public class LogitechLcdButton
    {
        private bool IsPresedFlag;
        Func<int, bool> CheckPress;
        Action PressAction;
        int ButtonIndex;

        public LogitechLcdButton(Func<int, bool> checkPress, int btnIndex, Action pressAction)
        {
            CheckPress = checkPress;
            ButtonIndex = btnIndex;
            IsPresedFlag = CheckPress(ButtonIndex);
            PressAction = pressAction;
        }

        public void CheckButton()
        {
            if (CheckPress(ButtonIndex))
            {
                if (IsPresedFlag == false)
                {
                    IsPresedFlag = true;
                    PressAction?.Invoke();
                }
            }
            else
            {
                IsPresedFlag = false;
            }
        }
    }
}
