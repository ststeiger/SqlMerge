
namespace SQLMerge
{


    class EncodingDetector
    {


        /// <summary>
        /// return the detected encoding and the contents of the file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="contents"></param>
        /// <returns></returns>
        public static System.Text.Encoding DetectEncodingWithStreamReader(string fileName, out string contents)
        {
            // open the file with the stream-reader:
            using (System.IO.StreamReader reader = new System.IO.StreamReader(fileName, true))
            {
                // read the contents of the file into a string
                contents = reader.ReadToEnd();

                // return the encoding.
                return reader.CurrentEncoding;
            }
        }


        public static System.Text.Encoding BomInfo(string srcFile)
        {
            return BomInfo(srcFile, false);
        } // End Function BomInfo 


        public static System.Text.Encoding BomInfo(string srcFile, bool thorough)
        {
            byte[] b = new byte[5];

            using (System.IO.FileStream file = new System.IO.FileStream(srcFile, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
            {
                int numRead = file.Read(b, 0, 5);
                if (numRead < 5)
                    System.Array.Resize(ref b, numRead);

                file.Close();
            } // End Using file 

            if (b.Length >= 4 && b[0] == 0x00 && b[1] == 0x00 && b[2] == 0xFE && b[3] == 0xFF)
                return System.Text.Encoding.GetEncoding("utf-32BE"); // UTF-32, big-endian 
            else if (b.Length >= 4 && b[0] == 0xFF && b[1] == 0xFE && b[2] == 0x00 && b[3] == 0x00)
                return System.Text.Encoding.UTF32; // UTF-32, little-endian
            else if (b.Length >= 2 && b[0] == 0xFE && b[1] == 0xFF)
                return System.Text.Encoding.BigEndianUnicode; // UTF-16, big-endian
            else if (b.Length >= 2 && b[0] == 0xFF && b[1] == 0xFE)
                return System.Text.Encoding.Unicode; // UTF-16, little-endian
            else if (b.Length >= 3 && b[0] == 0xEF && b[1] == 0xBB && b[2] == 0xBF)
                return System.Text.Encoding.UTF8;  // UTF-8
            else if (b.Length >= 3 && b[0] == 0x2b && b[1] == 0x2f && b[2] == 0x76)
                return System.Text.Encoding.UTF7;  // UTF-7


            // Maybe there is a future encoding ...
            // PS: The above yields more than this - this doesn't find UTF7 ...
            if (thorough)
            {
                foreach (System.Text.EncodingInfo ei in System.Text.Encoding.GetEncodings())
                {
                    System.Text.Encoding enc = ei.GetEncoding();

                    byte[] preamble = enc.GetPreamble();
                    if (preamble.Length == 0)
                        continue;

                    if (preamble.Length > b.Length)
                        continue;

                    for (int i = 0; i < preamble.Length; ++i)
                    {
                        if (b[i] != preamble[i])
                        {
                            goto NextEncoding;
                        }
                    } // Next i 

                    return enc;
                NextEncoding:
                    continue;
                } // Next ei
            } // End if (thorough)

            return null;
        } // End Function BomInfo 


        public static System.Text.Encoding DetectOrGuessEncoding(string fileName)
        {
            return DetectOrGuessEncoding(fileName, false);
        }


        public static System.Text.Encoding DetectOrGuessEncoding(string fileName, bool withOutput)
        {
            if (!System.IO.File.Exists(fileName))
                return null;


            System.ConsoleColor origBack = System.ConsoleColor.Black;
            System.ConsoleColor origFore = System.ConsoleColor.White;
            

            if (withOutput)
            {
                origBack = System.Console.BackgroundColor;
                origFore = System.Console.ForegroundColor;
            }

            // System.Text.Encoding systemEncoding = System.Text.Encoding.Default; // Returns hard-coded UTF8 on .NET Core ... 
            System.Text.Encoding systemEncoding = GetSystemEncoding();
            System.Text.Encoding utf8Encoding = System.Text.Encoding.UTF8;

            System.Text.Encoding enc = BomInfo(fileName);
            if (enc != null)
            {
                if (withOutput)
                {
                    System.Console.BackgroundColor = System.ConsoleColor.Green;
                    System.Console.ForegroundColor = System.ConsoleColor.White;
                    System.Console.WriteLine(fileName);
                    System.Console.WriteLine(enc);
                    System.Console.BackgroundColor = origBack;
                    System.Console.ForegroundColor = origFore;
                }

                return enc;
            }

            using (System.IO.Stream strm = System.IO.File.OpenRead(fileName))
            {
                UtfUnknown.DetectionResult detect = UtfUnknown.CharsetDetector.DetectFromStream(strm);

                if (detect != null && detect.Details != null && detect.Details.Count > 0 && detect.Details[0].Confidence < 1)
                {
                    if (withOutput)
                    {
                        System.Console.BackgroundColor = System.ConsoleColor.Red;
                        System.Console.ForegroundColor = System.ConsoleColor.White;
                        System.Console.WriteLine(fileName);
                        System.Console.WriteLine(detect);
                        System.Console.BackgroundColor = origBack;
                        System.Console.ForegroundColor = origFore;
                    }

                    foreach (UtfUnknown.DetectionDetail detail in detect.Details)
                    {
                        if (detail.Encoding == utf8Encoding || detail.Encoding == systemEncoding)
                            return detail.Encoding;
                    }

                    return detect.Details[0].Encoding;
                }
                else if (detect != null && detect.Details != null && detect.Details.Count > 0)
                {
                    if (withOutput)
                    {
                        System.Console.BackgroundColor = System.ConsoleColor.Green;
                        System.Console.ForegroundColor = System.ConsoleColor.White;
                        System.Console.WriteLine(fileName);
                        System.Console.WriteLine(detect);
                        System.Console.BackgroundColor = origBack;
                        System.Console.ForegroundColor = origFore;
                    }

                    return detect.Details[0].Encoding;
                }

                enc = GetSystemEncoding();

                if (withOutput)
                {
                    System.Console.BackgroundColor = System.ConsoleColor.DarkRed;
                    System.Console.ForegroundColor = System.ConsoleColor.Yellow;
                    System.Console.WriteLine(fileName);
                    System.Console.Write("Assuming ");
                    System.Console.Write(enc.WebName);
                    System.Console.WriteLine("...");
                    System.Console.BackgroundColor = origBack;
                    System.Console.ForegroundColor = origFore;
                }

                return systemEncoding;
            } // End Using strm 

        } // End Function DetectOrGuessEncoding 


        public static System.Text.Encoding GetSystemEncoding()
        {
            // The OEM code page for use by legacy console applications
            // int oem = System.Globalization.CultureInfo.CurrentCulture.TextInfo.OEMCodePage;

            // The ANSI code page for use by legacy GUI applications
            // int ansi = System.Globalization.CultureInfo.InstalledUICulture.TextInfo.ANSICodePage; // Machine 
            int ansi = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ANSICodePage; // User 

            try
            {
                // https://stackoverflow.com/questions/38476796/how-to-set-net-core-in-if-statement-for-compilation
#if ( NETSTANDARD && !NETSTANDARD1_0 )  || NETCORE || NETCOREAPP3_0 || NETCOREAPP3_1 
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
#endif

                System.Text.Encoding enc = System.Text.Encoding.GetEncoding(ansi);
                return enc;
            }
            catch (System.Exception)
            { }


            try
            {

                foreach (System.Text.EncodingInfo ei in System.Text.Encoding.GetEncodings())
                {
                    System.Text.Encoding e = ei.GetEncoding();

                    // 20'127: US-ASCII 
                    if (e.WindowsCodePage == ansi && e.CodePage != 20127)
                    {
                        return e;
                    }

                }
            }
            catch (System.Exception)
            { }

            // return System.Text.Encoding.GetEncoding("iso-8859-1");
            return System.Text.Encoding.UTF8;
        } // End Function GetSystemEncoding 


    } // End Class 


}
