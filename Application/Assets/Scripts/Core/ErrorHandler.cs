using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

namespace BMSCore
{
    public class ErrorHandler
    {
        private static string errorPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\RhythmTracersData\\error\\";

        public static void LogError(string errorStr)
        {
            string filepath = errorPath + "error" + DateTime.Today.ToString("yyyy-MM-dd") + ".txt";
            if (!File.Exists(filepath))
            {
                Directory.CreateDirectory(errorPath);
                File.Create(filepath).Close();
            }

            FileStream fs = File.Open(filepath, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(DateTime.Now.ToString() + "] " + errorStr);

            sw.Close();
            fs.Close();
        }
    }
}