using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace borrdome
{
    // This is a safety module of sorts... also could be useful to have for cross-platform purposes.
    
    public static class Console
    {
        public static ConsoleColor ForegroundColor
        {
            get { return System.Console.ForegroundColor; }
            set { System.Console.ForegroundColor = value; }
        }

        public static ConsoleColor BackgroundColor
        {
            get { return System.Console.BackgroundColor; }
            set { System.Console.BackgroundColor = value; }
        }
        
        public static void SetCursorPosition(int left, int top)
        {
            System.Console.SetCursorPosition(left, top);
        }

        public static void Clear()
        {
            System.Console.Clear();
        }

        public static void Write(string text)
        {
            System.Console.Write(text);
        }

        public static ConsoleKeyInfo ReadKey(bool check)
        {
            return System.Console.ReadKey(check);
        }

        public static void Initialize()
        {
            Type t = Type.GetType("Mono.Runtime");
            
            System.Console.SetWindowSize(80, 25);
            if (t == null) System.Console.SetBufferSize(80, 25); // This doesn't work in mono
            System.Console.CursorVisible = false;
            System.Console.Title = "Borr-Dome";
        }
        
    }
}
