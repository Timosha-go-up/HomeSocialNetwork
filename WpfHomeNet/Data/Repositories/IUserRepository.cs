using HomeSocialNetwork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfHomeNet.Data.Repositories
{
    public interface IUserRepository
    {
        // Создание (Create)
        Task<UserEntity> CreateAsync(UserEntity user);

        // Чтение (Read)
        Task<List<UserEntity>> GetAllAsync();
        Task<UserEntity?> GetByIdAsync(int id);
        Task<UserEntity?> GetByEmailAsync(string email);

      
        Task UpdateAsync(UserEntity user);

        // Удаление (Delete)
        Task DeleteAsync(int id);
    }
}
