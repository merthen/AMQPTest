using System.Collections.Generic;
using System.Threading.Tasks;
using Test.API.Models;

namespace Test.API.Data
{
    public interface IUserRepository
    {
        Task<User> CreateUser(User user);
        void DeleteUser(User user);
        void UpdateUser(User user);
        Task<User> GetUser(int id);
        Task<IEnumerable<User>> GetUsers();
        Task<bool> UserExists(string name);
        Task<bool> UserExistsById(int id);
    }
}