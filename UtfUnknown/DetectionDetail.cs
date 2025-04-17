
namespace UtfUnknown
{


    /// <summary>
    /// Detailed result of a detection
    /// </summary>
    public class DetectionDetail
    {
        /// <summary>
        /// A dictionary for replace unsupported codepage name in .NET to the nearly identical version.
        /// </summary>
        private static readonly System.Collections.Generic.Dictionary<string, string> FixedToSupportCodepageName =
            new System.Collections.Generic.Dictionary<string, string>
            {
                // CP949 is superset of ks_c_5601-1987 (see https://github.com/CharsetDetector/UTF-unknown/pull/74#issuecomment-550362133)
                {UtfUnknown.Core.CodepageName.CP949, UtfUnknown.Core.CodepageName.KS_C_5601_1987},
                {UtfUnknown.Core.CodepageName.ISO_2022_CN, UtfUnknown.Core.CodepageName.X_CP50227},
            };

        /// <summary>
        /// New result
        /// </summary>
        public DetectionDetail(string encodingShortName, float confidence, UtfUnknown.Core.Probers.CharsetProber prober = null,
            System.TimeSpan? time = null, string statusLog = null)
        {
            EncodingName = encodingShortName;
            Confidence = confidence;
            Encoding = GetEncoding(encodingShortName);
            Prober = prober;
            Time = time;
            StatusLog = statusLog;
        }

        /// <summary>
        /// New Result
        /// </summary>
        public DetectionDetail(UtfUnknown.Core.Probers.CharsetProber prober, System.TimeSpan? time = null)
            : this(prober.GetCharsetName(), prober.GetConfidence(), prober, time, prober.DumpStatus())
        {
        }

        /// <summary>
        /// The (short) name of the detected encoding. For full details, check <see cref="Encoding"/>
        /// </summary>
        public string EncodingName { get; }

        /// <summary>
        /// The detected encoding. 
        /// </summary>
        public System.Text.Encoding Encoding { get; set; }

        /// <summary>
        /// The confidence of the found encoding. Between 0 and 1.
        /// </summary>
        public float Confidence { get; set; }

        /// <summary>
        /// The used prober for detection
        /// </summary>
        public UtfUnknown.Core.Probers.CharsetProber Prober { get; set; }

        /// <summary>
        /// The time spend
        /// </summary>
        public System.TimeSpan? Time { get; set; }

        public string StatusLog { get; set; }

        public override string ToString()
        {
            return $"Detected {EncodingName} with confidence of {Confidence}";
        }

        internal static System.Text.Encoding GetEncoding(string encodingShortName)
        {
            string encodingName = FixedToSupportCodepageName.TryGetValue(encodingShortName, out string supportCodepageName)
                ? supportCodepageName
                : encodingShortName;
            try
            {
                return System.Text.Encoding.GetEncoding(encodingName);
            }
            catch (System.ArgumentException) // unsupported name
            {
#if NETSTANDARD && !NETSTANDARD1_0 || NETCOREAPP3_0
                return CodePagesEncodingProvider.Instance.GetEncoding(encodingName);
#else
                return null;
#endif
            }
        }
    }
}