using Net.Chdk.Detectors.Software.Binary;
using Net.Chdk.Model.Software;
using Net.Chdk.Providers.Software;
using System;
using System.Globalization;
using System.Linq;

namespace Net.Chdk.Detectors.Software.Chdk
{
    sealed class ChdkSoftwareDetector : InnerBinarySoftwareDetector
    {
        private const string Release = "release";
        private const string Trunk = "trunk";

        private static readonly string[] Prefixes = new[]
        {
            "Version ",
            "Firmware ",
        };

        public ChdkSoftwareDetector(ISourceProvider sourceProvider)
            : base(sourceProvider)
        {
        }

        protected override string ProductName => "CHDK";

        protected override string[] Strings => new[]
        {
            "CHDK ",
        };

        protected override int StringCount => 3;

        public override SoftwareInfo GetSoftware(byte[] buffer, int index)
        {
            var index2 = index;
            var first = GetString(buffer, ref index2);
            return Prefixes
                .Select(p => GetSoftware(buffer, index, first, p))
                .FirstOrDefault(s => s != null);
        }

        protected override Version GetProductVersion(string[] strings)
        {
            var split = strings[0].Trim('\'').Split(' ');
            if (split.Length != 2)
                return null;
            var str = GetVersionString(split[1].Split('-'));
            return GetVersion(str);
        }

        protected override CultureInfo GetLanguage(string[] strings)
        {
            var sourceName = GetSourceName(strings);
            switch (sourceName)
            {
                case "CHDK":
                    return CultureInfo.GetCultureInfo("en");
                case "CHDK_DE":
                    return CultureInfo.GetCultureInfo("de");
                default:
                    return null;
            }
        }

        protected override DateTime? GetCreationDate(string[] strings)
        {
            var str = TrimStart(strings[1], "Build: ");
            return GetCreationDate(str);
        }

        protected override SoftwareCameraInfo GetCamera(string[] strings)
        {
            var str = TrimStart(strings[2], "Camera: ");
            if (str == null)
                return null;
            var split = str.Split(new[] { " - " }, StringSplitOptions.None);
            if (split.Length != 2)
                return null;
            return GetCamera(split[0], split[1]);
        }

        protected override string GetSourceName(string[] strings)
        {
            return strings[0].Trim('\'').Split(' ')[0];
        }

        protected override string GetSourceChannel(string[] strings)
        {
            var version = GetProductVersion(strings);
            if ((version.Major > 1 || (version.Major == 1 && version.Minor >= 4)) && version.MajorRevision > 0)
                return Trunk;
            return Release;
        }

        private static string GetVersionString(string[] versionSplit)
        {
            switch (versionSplit.Length)
            {
                case 0:
                    return null;
                case 1:
                    return versionSplit[0];
                default:
                    return string.Join(".", versionSplit[0], versionSplit[1]);
            }
        }

        private SoftwareInfo GetSoftware(byte[] buffer, int index, string first, string prefix)
        {
            if (!first.StartsWith(prefix))
                return null;
            return base.GetSoftware(buffer, index + prefix.Length);
        }

        private static string TrimStart(string str, string prefix)
        {
            if (!str.StartsWith(prefix))
                return null;
            return str.Substring(prefix.Length);
        }
    }
}
