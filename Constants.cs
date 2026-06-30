using System;

namespace VotRite
{
    class Constants
    {
        public const string SLASH = "\\";
        public static string APP_PATH = "";
        public static string CONFIG_FILE = "";

        public const int COM_PORT_BAUD_RATE = 9600;
        public const int COM_PORT_DATA_BITS = 8;

        public const int SCREEN_V_SPACE = 20;
        public const int BORDER_PATH_LEN = 5;
        public const int CONTEST_DATA_V_SPACE = 20;

        public const int SESSION_CHECK_STATE_INTERVAL = 10000;
        public const int SESSION_EXPIRE_INTERVAL = 10; //300;
        public const int SESSION_TOTAL_TICKS = 10;
        public const int SESSION_TICKS_INTERVAL = 1000;
        public const int SECONDS_IN_MIN = 60;

        public const int CAST_TIMEOUT = 5000;

        public enum PinPadKey
        {
            KEY_0=48,
            KEY_1=49,
            KEY_2=50,
            KEY_3=51,
            KEY_4=52,
            KEY_5=53,
            KEY_6=54,
            KEY_7=55,
            KEY_8=56,
            KEY_9=57,
            KEY_DOT=190,
            KEY_UP=38,
            KEY_DOWN=40,
            KEY_LEFT=37,
            KEY_RIGHT=39,
            KEY_SPACE=32,
            KEY_BKSP=8,
            KEY_ENTER=13
        }

        public enum PinPadState { OK, ERROR }

        public enum SessionState { INACTIVE, ACTIVE, SUSPENDED, EXPIRED }

        public enum BallotTextSize { SMALL = 12, NORMAL = 18, LARGE = 24 }

        public enum ScreenObjectType
        {
            IMAGE,
            BUTTON,
            SELECTION,
            LABEL,
            CONTAINER,
            SCROLL
        }

        public enum ScreenObjectAction
        {
            NONE,
            SET_LOCALE,
            GET_SCREEN,
            GET_CONTEST,
            GET_CONTEST_BY_ID,
            SET_SELECTION,
            SET_WRITEIN,
            SET_TEXT,
            CANCEL,
            ACCEPT,
            CONTINUE,
            GO_BACK,
            SPEAK_TO_POLL_WORKER,
            PRINT_RECORD,
            CONFIRM,
            CANCEL_CONFIRM,
            CAST_BALLOT,
            SCROLL_CONTEST,
            SET_VOLUME,
            SET_TEXT_SIZE,
            SAVE_PREFERENCES
        }

        public enum ScreenObjectState { INACTIVE, ACTIVE }

        public enum SelectionState { DESELECTED, SELECTED }

        public enum ContestDirection { FORWARD, BACK }

        public enum ScrollDirection { DOWN, UP }
        public const int DOWN_SCROLL_TOP = 850;

        public enum PrintDoc { RECORD, REPORT, VOID }

        public enum keyAction { Unlock, none}
    }
}
