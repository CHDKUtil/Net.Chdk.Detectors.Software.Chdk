using Net.Chdk.Model.Card;
using Net.Chdk.Model.Software;
using Net.Chdk.Providers.Boot;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Net.Chdk.Detectors.Software.Chdk
{
    public sealed class ChdkProductDetector : IProductDetector
    {
        private const string Name = "CHDK";

        private static readonly Dictionary<string, string> Versions = new Dictionary<string, string>
        {
            ["CCHDK4.CFG"] = "1.4",
            ["OSD__4.CFG"] = "1.4",
            ["CCHDK3.CFG"] = "1.3",
            ["OSD__3.CFG"] = "1.3",
            ["CCHDK2.CFG"] = "1.2",
            ["OSD__2.CFG"] = "1.2",
            ["CCHDK1.CFG"] = "1.1",
            ["OSD__1.CFG"] = "1.1",
            ["CCHDK.CFG"] = "1.0",
        };

        private static readonly Dictionary<string, string> Languages = new Dictionary<string, string>
        {
            ["logo.dat"] = "en",
            ["logo_de.dat"] = "de",
        };

        private IBootProvider BootProvider { get; }

        public ChdkProductDetector(IBootProvider bootProvider)
        {
            BootProvider = bootProvider;
        }

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
            return GetValue(chdkPath, Versions, Version.Parse);
        }

        private static CultureInfo GetLanguage(string chdkPath)
        {
            var dataPath = Path.Combine(chdkPath, "DATA");
            return GetValue(dataPath, Languages, CultureInfo.GetCultureInfo);
        }

        private DateTime GetCreationTime(CardInfo cardInfo)
        {
            var rootPath = cardInfo.GetRootPath();
            var diskbootPath = Path.Combine(rootPath, BootProvider.FileName);
            return File.GetCreationTimeUtc(diskbootPath);
        }

        private static T GetValue<T>(string basePath, IDictionary<string, string> mapping, Func<string, T> getValue)
            where T : class
        {
            foreach (var kvp in mapping)
            {
                var filePath = Path.Combine(basePath, kvp.Key);
                if (File.Exists(filePath))
                    return getValue(kvp.Value);
            }
            return null;
        }
    }
}
