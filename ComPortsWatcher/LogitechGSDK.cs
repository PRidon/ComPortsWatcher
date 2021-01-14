﻿using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Specialized;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ComPortsWatcher
{
    public class LogitechGSDK
    {
        //LCD	SDK
        public const int LOGI_LCD_COLOR_BUTTON_LEFT = (0x00000100);
        public const int LOGI_LCD_COLOR_BUTTON_RIGHT = (0x00000200);
        public const int LOGI_LCD_COLOR_BUTTON_OK = (0x00000400);
        public const int LOGI_LCD_COLOR_BUTTON_CANCEL = (0x00000800);
        public const int LOGI_LCD_COLOR_BUTTON_UP = (0x00001000);
        public const int LOGI_LCD_COLOR_BUTTON_DOWN = (0x00002000);
        public const int LOGI_LCD_COLOR_BUTTON_MENU = (0x00004000);
        public const int LOGI_LCD_MONO_BUTTON_0 = (0x00000001);
        public const int LOGI_LCD_MONO_BUTTON_1 = (0x00000002);
        public const int LOGI_LCD_MONO_BUTTON_2 = (0x00000004);
        public const int LOGI_LCD_MONO_BUTTON_3 = (0x00000008);
        public const int LOGI_LCD_MONO_WIDTH = 160;
        public const int LOGI_LCD_MONO_HEIGHT = 43;
        public const int LOGI_LCD_COLOR_WIDTH = 320;
        public const int LOGI_LCD_COLOR_HEIGHT = 240;
        public const int LOGI_LCD_TYPE_MONO = (0x00000001);
        public const int LOGI_LCD_TYPE_COLOR = (0x00000002);

        private const int RIGHT_SHIFT_INDEX = (20);
        private const int NUMBER_OF_STRING = (4);

        string[] LcdBaseStrings;
        string[] LcdShowStrings;

        int ShiftDown;
        int ShiftRight;

        LogitechLcdButton Button0;
        LogitechLcdButton Button1;
        LogitechLcdButton Button2;
        LogitechLcdButton Button3;

        public LogitechGSDK(Action btn0PrsEvent, Action btn1PrsEvent, Action btn2PrsEvent, Action btn3PrsEvent)
        {
            ShiftDown = 0;
            ShiftRight = 0;
            LogiLcdInit("Com Ports viewer", LOGI_LCD_TYPE_MONO);
            Button0 = new LogitechLcdButton(LogiLcdIsButtonPressed, LOGI_LCD_MONO_BUTTON_0, ShiftToDown);
            Button1 = new LogitechLcdButton(LogiLcdIsButtonPressed, LOGI_LCD_MONO_BUTTON_1, ShiftToUp);
            Button2 = new LogitechLcdButton(LogiLcdIsButtonPressed, LOGI_LCD_MONO_BUTTON_2, btn2PrsEvent);
            Button3 = new LogitechLcdButton(LogiLcdIsButtonPressed, LOGI_LCD_MONO_BUTTON_3, btn3PrsEvent);
            LcdBaseStrings = new string[1];
            LcdShowStrings = new string[1];
        }

        public static bool LcdExist()
        {
            return LogiLcdIsConnected(LOGI_LCD_TYPE_MONO);
        }

        public void Polling()
        {
            Button0.CheckButton();
            Button1.CheckButton();
            Button2.CheckButton();
            Button3.CheckButton();
            LcdShow();
        }

        public void LcdUpdate(List<string> lcdStrings)
        {
            if (!Enumerable.SequenceEqual(LcdBaseStrings, lcdStrings))
            {
                LcdBaseStrings = (string[])lcdStrings.ToArray().Clone();
                LcdShowStrings = (string[])LcdBaseStrings.Clone();
            }
        }

        private void LcdShow()
        {
            LcdShowStrings = (string[])LcdBaseStrings.Clone();
            if (LcdBaseStrings.Length < NUMBER_OF_STRING)
                ShiftDown = 0;
            else if (ShiftDown > LcdBaseStrings.Length - NUMBER_OF_STRING)
                ShiftDown = LcdBaseStrings.Length - NUMBER_OF_STRING;
            List<string> tt = LcdShowStrings.ToList();
            tt.RemoveRange(0, ShiftDown);
            for (int i = 0; (i < NUMBER_OF_STRING) && (i < tt.Count); i++)
            {
                LogiLcdMonoSetText(i, tt[i]);
            }
            LogiLcdUpdate();
        }

        private void ShiftToDown()
        {
            if (LcdBaseStrings.Length < NUMBER_OF_STRING)
                ShiftDown = 0;
            else
            {
                ShiftDown++;
                if (ShiftDown > LcdBaseStrings.Length - NUMBER_OF_STRING)
                    ShiftDown = LcdBaseStrings.Length - NUMBER_OF_STRING;
            }
        }

        private void ShiftToUp()
        {
            ShiftDown--;
            if (ShiftDown < 0)
                ShiftDown = 0;
        }

        private void ShiftToRight()
        {
            for (int i = 0; i < LcdBaseStrings.Length; i++)
            {
                if (LcdBaseStrings[i].Length > (RIGHT_SHIFT_INDEX * (ShiftRight + 1)))
                    LcdShowStrings[i] = LcdBaseStrings[i].Substring(RIGHT_SHIFT_INDEX * ShiftRight);
            }
        }

        [DllImport("LogitechLcdEnginesWrapper", CharSet = CharSet.Unicode,
        CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLcdInit(String friendlyName, int lcdType);

        [DllImport("LogitechLcdEnginesWrapper", CharSet = CharSet.Unicode,
        CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLcdIsConnected(int lcdType);

        [DllImport("LogitechLcdEnginesWrapper", CharSet = CharSet.Unicode,
        CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLcdIsButtonPressed(int button);

        [DllImport("LogitechLcdEnginesWrapper", CharSet = CharSet.Unicode,
        CallingConvention = CallingConvention.Cdecl)]
        public static extern void LogiLcdUpdate();

        [DllImport("LogitechLcdEnginesWrapper", CharSet = CharSet.Unicode,
        CallingConvention = CallingConvention.Cdecl)]
        public static extern void LogiLcdShutdown();

        //	Monochrome	LCD	functions
        [DllImport("LogitechLcdEnginesWrapper", CharSet = CharSet.Unicode,
        CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLcdMonoSetBackground(byte[] monoBitmap);

        [DllImport("LogitechLcdEnginesWrapper", CharSet = CharSet.Unicode,
        CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLcdMonoSetText(int lineNumber, String text);

        //	Color	LCD	functions
        [DllImport("LogitechLcdEnginesWrapper", CharSet = CharSet.Unicode,
        CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLcdColorSetBackground(byte[] colorBitmap);

        [DllImport("LogitechLcdEnginesWrapper", CharSet = CharSet.Unicode,
        CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLcdColorSetTitle(String text, int red, int green,
        int blue);

        [DllImport("LogitechLcdEnginesWrapper", CharSet = CharSet.Unicode,
        CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LogiLcdColorSetText(int lineNumber, String text, int red,
        int green, int blue);
    }
}