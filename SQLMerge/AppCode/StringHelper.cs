
namespace SqlMerge
{


    internal class StringHelper
    {


        internal static string RepeatNewLine(int iRepeatCount)
        {
            string strResult = "";
            string strNewLine = System.Environment.NewLine;

            for (int iRepeatIndex = 0; iRepeatIndex < iRepeatCount; ++iRepeatIndex)
            {
                strResult += strNewLine;
            } // Next iRepeatIndex

            return strResult;
        } // End Function RepeatNewLine



        internal static int SortFileNames(System.IO.FileInfo f1, System.IO.FileInfo f2)
        {
            int? iName1 = null;
            int? iName2 = null;


            System.Text.RegularExpressions.Match ma = System.Text.RegularExpressions.Regex.Match(f1.Name, @"^\d+");
            if (ma.Success)
            {
                try
                {
                    iName1 = int.Parse(ma.Value);
                }
                catch { }
            }


            ma = System.Text.RegularExpressions.Regex.Match(f2.Name, @"^\d+");
            if (ma.Success)
            {
                try
                {
                    iName2 = int.Parse(ma.Value);
                }
                catch { }
            }

            if (iName1.HasValue && iName2.HasValue)
            {
                int i = iName1.Value.CompareTo(iName2.Value);

                if (i != 0)
                    return i;
            }

            return f1.Name.CompareTo(f2.Name);
        }


    }


}
