using Backend.Services.Interfaces;

namespace Backend.Services.Implementations
{
    public class FileService : IFileService
    {
        public string ReadFile(string path, string body)
        {
            using (StreamReader reader = new StreamReader(path))
            {
                body = reader.ReadToEnd();
            };

            return body;
        }
    }
}
