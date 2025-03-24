using System;
using System.IO;
using System.Text;

namespace SQLMerge.Core.Utilities
{
    public static class EncodingDetector
    {
        public static Encoding DetectOrGuessEncoding(string filePath)
        {
            // Read the first 4096 bytes of the file
            byte[] buffer = new byte[4096];
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                fs.Read(buffer, 0, buffer.Length);
            }

            // Check for BOM
            if (buffer[0] == 0xEF && buffer[1] == 0xBB && buffer[2] == 0xBF)
                return Encoding.UTF8;
            if (buffer[0] == 0xFF && buffer[1] == 0xFE)
                return Encoding.Unicode;
            if (buffer[0] == 0xFE && buffer[1] == 0xFF)
                return Encoding.BigEndianUnicode;

            // Try to detect encoding based on content
            bool hasNullBytes = false;
            bool hasHighBytes = false;
            bool hasLowBytes = false;

            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] == 0)
                    hasNullBytes = true;
                if (buffer[i] > 127)
                    hasHighBytes = true;
                if (buffer[i] < 32 && buffer[i] != 9 && buffer[i] != 10 && buffer[i] != 13)
                    hasLowBytes = true;
            }

            if (hasNullBytes)
                return Encoding.Unicode;
            if (hasHighBytes && !hasLowBytes)
                return Encoding.UTF8;
            if (hasHighBytes && hasLowBytes)
                return Encoding.GetEncoding(1252); // Windows-1252

            return Encoding.UTF8; // Default to UTF-8
        }
    }
} 