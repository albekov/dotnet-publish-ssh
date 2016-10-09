using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Renci.SshNet;

namespace DotnetPublishSsh
{
    internal sealed class Uploader
    {
        public char DirectorySeparator { get; set; } = '/';

        private readonly ConnectionInfo _connectionInfo;
        private readonly HashSet<string> _existingDirectories = new HashSet<string>();

        public Uploader(PublishSshOptions publishSshOptions)
        {
            _connectionInfo = CreateConnectionInfo(publishSshOptions);
        }

        private static ConnectionInfo CreateConnectionInfo(PublishSshOptions options)
        {
            var authenticationMethods = new List<AuthenticationMethod>();

            if (options.Password != null)
                authenticationMethods.Add(
                    new PasswordAuthenticationMethod(options.User, options.Password));

            if (options.KeyFile != null)
                authenticationMethods.Add(
                    new PrivateKeyAuthenticationMethod(options.User, new PrivateKeyFile(options.KeyFile)));

            var connectionInfo = new ConnectionInfo(
                options.Host,
                options.Port,
                options.User,
                authenticationMethods.ToArray());

            return connectionInfo;
        }

        public void UploadFiles(string path, ICollection<LocalFile> localFiles)
        {
            //using (var client = new SshClient(_connectionInfo))
            using (var ftp = new SftpClient(_connectionInfo))
            {
                //client.Connect();
                ftp.Connect();

                foreach (var localFile in localFiles)
                {
                    UploadFile(localFile, ftp, path);
                }
            }
            Console.WriteLine($"Uploaded {localFiles.Count} files.");
        }

        private void UploadFile(LocalFile localFile, SftpClient ftp, string path)
        {
            Console.WriteLine($"Uploading {localFile.RelativeName}");
            using (var stream = File.OpenRead(localFile.FileName))
            {
                var filePath = localFile.RelativeName.Replace(Path.DirectorySeparatorChar, DirectorySeparator);

                var fullPath = path + filePath;

                EnsureDirExists(ftp, fullPath);

                ftp.UploadFile(stream, fullPath, true);
            }
        }

        private void EnsureDirExists(SftpClient ftp, string path)
        {
            var parts = path.Split(new[] {DirectorySeparator}, StringSplitOptions.RemoveEmptyEntries)
                .Where(p => !string.IsNullOrEmpty(p))
                .ToList();

            if (!path.EndsWith(DirectorySeparator.ToString()))
                parts = parts.Take(parts.Count - 1).ToList();

            CreateDir(ftp, parts);
        }

        private void CreateDir(SftpClient ftp, ICollection<string> parts, bool noCheck = false)
        {
            if (parts.Any())
            {
                var path = Combine(parts);
                var parent = parts.Take(parts.Count - 1).ToList();

                if (noCheck || ftp.Exists(path))
                {
                    CreateDir(ftp, parent, true);
                }
                else
                {
                    CreateDir(ftp, parent);
                    ftp.CreateDirectory(path);
                }

                _existingDirectories.Add(path);
            }
        }

        private string Combine(ICollection<string> parts)
        {
            var path = DirectorySeparator +
                       string.Join(DirectorySeparator.ToString(), parts) +
                       (parts.Any() ? DirectorySeparator.ToString() : "");
            return path;
        }
    }
}