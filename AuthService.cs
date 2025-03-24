using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using SIMS_App.Models;

namespace SIMS_App.Services
{
    public class AuthService
    {
        private readonly string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Users.json");

        public User ValidateUser(string username, string password)
        {
            var users = LoadUsers();
            return users.Find(u => u.Username == username && u.Password == password);
        }

        public bool RegisterUser(RegisterModel model)
        {
            var users = LoadUsers();

            if (users.Exists(u => u.Username == model.Username))
            {
                return false;
            }

            users.Add(new User
            {
                Username = model.Username,
                Password = model.Password,
                Role = model.Role
            });

            SaveUsers(users);
            return true;
        }

        private List<User> LoadUsers()
        {
            if (!File.Exists(_filePath))
            {
                Console.WriteLine("Users.json not found, returning empty list.");
                return new List<User>();
            }

            var json = File.ReadAllText(_filePath).Trim();
            Console.WriteLine($"JSON content read from file: {json}"); // In nội dung JSON ra console

            if (string.IsNullOrEmpty(json) || json == "null")
            {
                Console.WriteLine("JSON file is empty or contains null, resetting to empty list.");
                return new List<User>();
            }

            try
            {
                var users = JsonConvert.DeserializeObject<List<User>>(json);
                if (users == null)
                {
                    Console.WriteLine("Deserialization returned null, resetting to empty list.");
                    return new List<User>();
                }
                return users;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error parsing JSON: {ex.Message}. Resetting file.");
                ResetUsersFile(); // Reset file nếu JSON sai định dạng
                return new List<User>();
            }
        }

        private void ResetUsersFile()
        {
            var defaultUsers = new List<User>
    {
        new User { Username = "admin", Password = "123", Role = "Admin" },
        new User { Username = "user1", Password = "password123", Role = "User" }
    };

            SaveUsers(defaultUsers);
            Console.WriteLine("Users.json was reset to default users.");
        }




        private void SaveUsers(List<User> users)
        {
            try
            {
                var json = JsonConvert.SerializeObject(users, Formatting.Indented);
                File.WriteAllText(_filePath, json);
                Console.WriteLine("User data saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving user data: {ex.Message}");
            }
        }


    }
}





