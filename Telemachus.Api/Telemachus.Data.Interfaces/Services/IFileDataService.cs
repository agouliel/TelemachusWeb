namespace Telemachus.Data.Interfaces.Services
{
    public interface IFileDataService
    {
        void Save(string fileName, string path, byte[] data);
        void Delete(string path);
    }
}
