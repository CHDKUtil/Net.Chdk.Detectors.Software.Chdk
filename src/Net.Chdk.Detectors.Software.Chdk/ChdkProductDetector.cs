using Net.Chdk.Model.Card;
using Net.Chdk.Model.Software;
using System;
using System.Globalization;
using System.IO;

namespace Net.Chdk.Detectors.Software.Chdk
{
    public sealed class ChdkProductDetector : IProductDetector
    {
        private const string Name = "CHDK";

        public ProductInfo GetProduct(CardInfo cardInfo)
        {
            string chdkPath = Path.Combine(cardInfo.DriveLetter, Name);
            if (!Directory.Exists(chdkPath))
                return null;

            return new ProductInfo
            {
                Name = Name,
                Version = GetVersion(chdkPath),
                Created = GetCreationTime(cardInfo),
                Language = GetLanguage(chdkPath),
            };
        }

        private static string GetVersion(string chdkPath)
        {
            var cfg4Path = Path.Combine(chdkPath, "CCHDK4.CFG");
            if (File.Exists(cfg4Path))
                return "1.4";

            var cfg3Path = Path.Combine(chdkPath, "CCHDK3.CFG");
            if (File.Exists(cfg4Path))
                return "1.3";

            var cfg2Path = Path.Combine(chdkPath, "CCHDK2.CFG");
            if (File.Exists(cfg4Path))
                return "1.2";

            var cfg1Path = Path.Combine(chdkPath, "CCHDK1.CFG");
            if (File.Exists(cfg4Path))
                return "1.1";

            var cfgPath = Path.Combine(chdkPath, "CCHDK.CFG");
            if (File.Exists(cfg4Path))
                return "1.0";

            return null;
        }

        private static DateTime GetCreationTime(CardInfo cardInfo)
        {
            var diskbootPath = Path.Combine(cardInfo.DriveLetter, "DISKBOOT.BIN");
            return File.GetCreationTimeUtc(diskbootPath);
        }

        private static CultureInfo GetLanguage(string chdkPath)
        {
            var defaultPath = Path.Combine(chdkPath, "SCRIPTS", "default.lua");
            if (File.Exists(defaultPath))
                return CultureInfo.GetCultureInfo("de");

            return null;
        }
    }
}
