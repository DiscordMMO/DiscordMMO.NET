﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace DiscordMMO
{
    public static class Logger
    {

        public static ConsoleColor defaultForegroundColor { get; private set; }

        public static void Init()
        {
            defaultForegroundColor = Console.ForegroundColor;
        }

        public static void Log(string msg, LogSeverity severity = LogSeverity.Info)
        {
            // Set log color
            switch (severity)
            {
                // Errors
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                // Info
                case LogSeverity.Debug:
                case LogSeverity.Info:
                case LogSeverity.Verbose:
                    Console.ForegroundColor = defaultForegroundColor;
                    break;
                // Warnings
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                default:
                    Log($"Something went wrong while trying to log a message: Invalid message severity: {severity}", LogSeverity.Error);
                    Log($"Message attempted to log: {msg}", LogSeverity.Error);
                    return;
            }

            

            Console.WriteLine(msg);

            // Reset the color
            Console.ForegroundColor = defaultForegroundColor;

        }

        public static void Log(string msg, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(msg);
            Console.ForegroundColor = defaultForegroundColor;
        }

    }
}
