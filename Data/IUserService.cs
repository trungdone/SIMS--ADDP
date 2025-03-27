using System.Collections.Generic;
using SIMS_App.Models;

namespace SIMS_App.Services
{
    public interface IUserService
    {
        List<User> GetAllUsers(); // Sửa từ GetUsers() -> GetAllUsers()
        User GetUserById(int id);
        void AddUser(User user);
        void UpdateUser(User user);
        void DeleteUser(int id);
    }

}

