using OWML.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CameraShake
{
    public static class Log
    {
        static IModConsole Console;
        public static void Initialize(IModConsole console)
        {
            Console = console;
        }

        //--------------------------------------------- Printing for testing, not final version. ---------------------------------------------//
        public static void Print(object message) {
            if (ModMain.isDevelopmentVersion) Console.WriteLine($"{message}");
        }
        public static void Print(string line) {
            if (ModMain.isDevelopmentVersion) Console.WriteLine(line);
        }
        public static void Success(string line, bool showInFinal = true) {
            if (ModMain.isDevelopmentVersion || showInFinal) Console.WriteLine(line, MessageType.Success);
        }
        //--------------------------------------------- Warnings and Errors should show always. ---------------------------------------------//
        public static void Warning(string line) => Console.WriteLine(line, MessageType.Warning);
        public static void Error(string line) => Console.WriteLine(line, MessageType.Error);
        public static bool ErrorIf(bool error, string errorMessage)
        {
            if (error)
            {
                Console.WriteLine(errorMessage, MessageType.Error);
                return true;
            }
            return false;
        }
    }
}