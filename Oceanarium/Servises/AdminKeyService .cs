using Oceanarium.Servises.Interfaces;

namespace Oceanarium.Servises
{
    public class AdminKeyService : IAdminKeyService
    {
        private readonly HashSet<string> _keys;

        public AdminKeyService(IWebHostEnvironment env)
        {
            var path = Path.Combine(env.ContentRootPath, "Data", "AdminKeys.txt");
            if (File.Exists(path))
            {
                _keys = File.ReadAllLines(path).ToHashSet(StringComparer.OrdinalIgnoreCase);
            }
            else
            {
                _keys = new HashSet<string>();
            }
        }

        public bool IsValidKey(string key)
        {
            return !string.IsNullOrWhiteSpace(key) && _keys.Contains(key.Trim());
        }
    }
}
