using System;

namespace DotnetPublishSsh
{
    internal sealed class LocalFile
    {
        public LocalFile(string localPath, string fileName)
        {
            FileName = fileName;
            RelativeName = new Uri(localPath).MakeRelativeUri(new Uri(fileName)).OriginalString;
        }

        public string FileName { get; set; }

        public string RelativeName { get; set; }
    }
}