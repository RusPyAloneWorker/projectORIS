using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace HTTPResponse
{
    public class FileFinder
    {
        public static byte[] GetFileBytes(string rawPath, string path)
        {
            byte[] result = null;
            string fullPath = path + rawPath;
            Console.WriteLine(fullPath);
            if (Directory.Exists(fullPath))
            {
                fullPath += path;
                if (Directory.Exists(fullPath))
                {
                    result = File.ReadAllBytes(fullPath);
                }
            }
            else if (File.Exists(fullPath))
            {
                result = File.ReadAllBytes(fullPath);
            }
            return result;
        }
        public static string GetFileStrings(string rawPath, string path)
        {
            string result = null;
            string fullPath = path + rawPath;
            Console.WriteLine(fullPath);
            if (Directory.Exists(fullPath))
            {
                fullPath += path;
                if (Directory.Exists(fullPath))
                {
                    result = File.ReadAllText(fullPath);
                }
            }
            else if (File.Exists(fullPath))
            {
                result = File.ReadAllText(fullPath);
            }
            return result;
        }
    }
}
