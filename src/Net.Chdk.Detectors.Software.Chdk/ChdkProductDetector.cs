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

        public SoftwareProductInfo GetProduct(CardInfo cardInfo)
        {
            var rootPath = cardInfo.GetRootPath();
            var chdkPath = Path.Combine(rootPath, Name);
            if (!Directory.Exists(chdkPath))
                return null;

            return new SoftwareProductInfo
            {
                Name = Name,
                Version = GetVersion(chdkPath),
                Created = GetCreationTime(cardInfo),
                Language = GetLanguage(chdkPath),
            };
        }

        private static Version GetVersion(string chdkPath)
        {
            var cfg4Path = Path.Combine(chdkPath, "CCHDK4.CFG");
            if (File.Exists(cfg4Path))
                return new Version("1.4");

            var cfg3Path = Path.Combine(chdkPath, "CCHDK3.CFG");
            if (File.Exists(cfg3Path))
                return new Version("1.3");

            var cfg2Path = Path.Combine(chdkPath, "CCHDK2.CFG");
            if (File.Exists(cfg2Path))
                return new Version("1.2");

            var cfg1Path = Path.Combine(chdkPath, "CCHDK1.CFG");
            if (File.Exists(cfg1Path))
                return new Version("1.1");

            var cfgPath = Path.Combine(chdkPath, "CCHDK.CFG");
            if (File.Exists(cfgPath))
                return new Version("1.0");

            return null;
        }

        private static DateTime GetCreationTime(CardInfo cardInfo)
        {
            var diskbootPath = cardInfo.GetDiskbootPath();
            return File.GetCreationTimeUtc(diskbootPath);
        }

        private static CultureInfo GetLanguage(string chdkPath)
        {
            var dataPath = Path.Combine(chdkPath, "DATA");

            var logoPath = Path.Combine(dataPath, "logo.dat");
            if (File.Exists(logoPath))
                return new CultureInfo("en");

            var logoDePath = Path.Combine(dataPath, "logo_de.dat");
            if (File.Exists(logoDePath))
                return new CultureInfo("de");

            return null;
        }
    }
}
