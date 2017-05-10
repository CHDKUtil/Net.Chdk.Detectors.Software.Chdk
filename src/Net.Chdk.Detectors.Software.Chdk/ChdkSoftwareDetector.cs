using Net.Chdk.Detectors.Software.Binary;
using Net.Chdk.Model.Software;
using System;
using System.Globalization;

namespace Net.Chdk.Detectors.Software.Chdk
{
    sealed class ChdkSoftwareDetector : InnerBinarySoftwareDetector
    {
        protected override string Name => "CHDK";

        protected override string[] Strings => new[]
        {
            "CHDK Version ",
            "CHDK Firmware ",
        };

        protected override int StringCount => 3;

        protected override Version GetVersion(string[] strings)
        {
            var split = strings[0].Trim('\'').Split(' ');
            if (split.Length != 2)
                return null;
            var str = GetVersionString(split[1].Split('-'));
            return GetVersion(str);
        }

        protected override CultureInfo GetLanguage(string[] strings)
        {
            var split = strings[0].Trim('\'').Split(' ');
            var nameStr = split[0];
            switch (nameStr)
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

        private static string TrimStart(string str, string prefix)
        {
            if (!str.StartsWith(prefix))
                return null;
            return str.Substring(prefix.Length);
        }
    }
}
