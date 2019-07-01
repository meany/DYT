﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace dm.DYT.Common
{
    public static class Util
    {
        public static string ConvertToCompoundDuration(int Seconds, bool onlyOneUnit = true)
        {
            bool neg = false;
            if (Seconds < 0)
            {
                Seconds = Math.Abs(Seconds);
                neg = true;
            }
            if (Seconds == 0)
            {
                return "0 seconds";
            }

            var span = TimeSpan.FromSeconds(Seconds);
            int[] parts = { span.Days / 7, span.Days % 7, span.Hours, span.Minutes, span.Seconds };
            string[] units = { " week", " day", " hour", " minute", " second" };

            string r = string.Join(", ",
                from index in Enumerable.Range(0, units.Length)
                where parts[index] > 0
                select parts[index] + units[index] + ((parts[index] > 1) ? "s" : string.Empty));
            r = (onlyOneUnit) ? r.Split(',')[0] : r;
            return (neg) ? $"-{r}" : r;
        }

        public static string GetVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string fullV = fvi.FileVersion;
            string patchV = fullV.Substring(0, fullV.LastIndexOf('.')).TrimEnd(".0");
            string buildV = fullV.Substring(fullV.LastIndexOf('.') + 1);
            return $"{patchV} (build {buildV})";
        }

        public static bool StartDotNetProcess(string directory, string fileName, string[] args)
        {
            var proc = new Process();
            proc.StartInfo.WorkingDirectory = directory;
            proc.StartInfo.FileName = "dotnet";
            proc.StartInfo.Arguments = $"{fileName} {string.Join(" ", args)}";
            return proc.Start();
        }
    }
}
