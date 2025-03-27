using SIMS_App.Models;
using System.Text.Json;

namespace SIMS_App.Services
{
    public class UserService : IUserService
    {
        private readonly string filePath = "Resources/Student.json";

        public UserService()
        {
            Console.WriteLine($"🔍 UserService initialized. File path: {filePath}");
        }


        public List<User> GetUsers()
        {
            if (!File.Exists(filePath))
                return new List<User>();

            var jsonData = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<User>>(jsonData) ?? new List<User>();
        }

        public List<User> GetAllUsers()  // 🔹 Thêm phương thức này để tránh lỗi CS0535
        {
            return GetUsers();
        }

        public User? GetUserById(int id)
        {
            return GetUsers().FirstOrDefault(u => u.Id == id);
        }

        public void AddUser(User user)
        {
            var users = GetUsers();
            user.Id = users.Any() ? users.Max(u => u.Id) + 1 : 1;
            users.Add(user);
            SaveUsers(users);
        }

        public void UpdateUser(User user)
        {
            var users = GetUsers();
            var existingUser = users.FirstOrDefault(u => u.Id == user.Id);

            if (existingUser != null)
            {
                existingUser.Name = user.Name;
                existingUser.Email = user.Email;
                existingUser.Phone = user.Phone;
                existingUser.DateOfBirth = user.DateOfBirth;
                SaveUsers(users);
            }
        }

        public void DeleteUser(int id)
        {
            var users = GetUsers().Where(u => u.Id != id).ToList();
            SaveUsers(users);
        }

        private void SaveUsers(List<User> users)
        {
            var jsonData = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonData);
        }
    }
}
