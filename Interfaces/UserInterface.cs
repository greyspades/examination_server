using User.Models;

namespace User.Interface
{
    public interface IUserRepository
    {
        public Task<Settings> GetSettings();
        public Task SaveSettings(Settings payload);
        public Task<dynamic> AdminAuth(AdminDto payload);
    }
}
