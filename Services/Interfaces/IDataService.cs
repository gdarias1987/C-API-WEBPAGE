using MELI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MELI.Services
{
    public interface IDataService
    {
        Task<List<Checkpoint>> GetCheckpointAsync();
        
        Task<Checkpoint> GetCheckpointByIDAsync(int id);

        Task<Checkpoint> CreateCheckpointAsync(int id);

        Task UpdateCheckpointAsync(Checkpoint item);

        Task ClearDDBB();



        // USER LOGIN && JWT
        Task<Usuario> CheckUserLogin(Usuario usuario);

        Task<Usuario> GetUsuarioByIDAsync(int userId);
    }
}
