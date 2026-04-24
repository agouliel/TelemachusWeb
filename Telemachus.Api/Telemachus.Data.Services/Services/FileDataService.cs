using System.IO;
using Telemachus.Data.Interfaces.Services;

namespace Telemachus.Data.Services.Services
{
    public class FileDataService : IFileDataService
    {
        public void Save(string fileName, string path, byte[] image)
        {
            var dbPath = Path.Combine(Directory.GetCurrentDirectory(), path, fileName);

            using (var stream = new FileStream(dbPath, FileMode.Create))
            {
                stream.Write(image, 0, image.Length);
            }
        }

        public void Delete(string path)
        {
            var dbPath = Path.Combine(Directory.GetCurrentDirectory(), path);
            File.Delete(dbPath);
        }
    }
}
