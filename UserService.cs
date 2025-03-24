using SIMS_App.Models;
using SIMS_App.Services;  // Cần có namespace này để nhận diện IUserService
using System;
using System.Collections.Generic;
using System.Linq;

namespace SIMS.Services
{
    public class UserService : IUserService // 👈 Thêm kế thừa IUserService
    {
        private static List<User> users = new List<User>
        {
            new User { Id = 1, Name = "Nguyen Bao", Email = "user3@example.com", Phone = "5555555", DateOfBirth = new DateTime(1983, 7, 20) },
            new User { Id = 2, Name = "Nguyen Phong", Email = "user4@example.com", Phone = "4444444", DateOfBirth = new DateTime(1992, 11, 10) },
            new User { Id = 3, Name = "Nguyen Nam", Email = "user5@example.com", Phone = "7777777", DateOfBirth = new DateTime(1985, 4, 30) }
        };

        public List<User> GetAllUsers()
        {
            return users;
        }

        public User GetUserById(int id)
        {
            return users.FirstOrDefault(u => u.Id == id);
        }

        public void AddUser(User user)
        {
            user.Id = users.Max(u => u.Id) + 1;
            users.Add(user);
        }

        public void UpdateUser(User user)
        {
            var existingUser = users.FirstOrDefault(u => u.Id == user.Id);
            if (existingUser != null)
            {
                existingUser.Name = user.Name;
                existingUser.Email = user.Email;
                existingUser.Phone = user.Phone;
                existingUser.DateOfBirth = user.DateOfBirth;
            }
        }

        public void DeleteUser(int id)
        {
            var user = users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                users.Remove(user);
            }
        }
    }
}



