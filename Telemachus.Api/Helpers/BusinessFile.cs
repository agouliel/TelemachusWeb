using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Helpers
{
    public class BusinessFile
    {
        public string EventId { get; set; }
        public string ReportId { get; set; }
        public string FieldId { get; set; }
        public string BunkeringId { get; set; }
        public string DocumentTypeId { get; set; }
        private string _rootPath { get; set; }
        public BusinessFile(string rootPath = null)
        {
            _rootPath = rootPath;
            if (string.IsNullOrEmpty(_rootPath))
            {
                _rootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Telemachus");
            }
        }
        public DirectoryInfo GetPath()
        {
            string path = Path.Combine(
                _rootPath,
                "Bunkering",
                BunkeringId,
                "Documents"
            );
            if (!string.IsNullOrEmpty(DocumentTypeId))
            {
                path = Path.Combine(path, DocumentTypeId);
            }
            return new DirectoryInfo(path);
        }
        public void Delete()
        {
            throw new NotImplementedException();
        }
        public FileInfo GetFile()
        {
            var path = GetPath();
            if (!path.Exists)
            {
                return null;
            }
            return path.GetFiles().FirstOrDefault();
        }
        public List<FileInfo> GetFiles()
        {
            var path = GetPath();
            if (!path.Exists)
            {
                return null;
            }
            return path.GetFiles("*", SearchOption.AllDirectories).ToList();
        }
        public FileStream GetStream(string fileName)
        {
            var path = GetPath();
            if (!path.Exists)
            {
                path.Create();
            }
            return File.Create(Path.Combine(path.FullName, fileName));
        }
    }
}
