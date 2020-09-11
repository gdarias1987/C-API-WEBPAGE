using MELI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MELI.Services
{
    public class DataServiceDynamic : IDataService
    {
        public async Task<List<Checkpoint>> GetCheckpointAsync()
        {
            await Task.Delay(10);

            return Checkpoint.listado;
        }

        public async Task<Checkpoint> GetCheckpointByIDAsync(int id)
        {
            await Task.Delay(10);

            return Checkpoint.listado.Where(I=>I.idEvento == id).FirstOrDefault();
        }

        public async Task<Checkpoint> CreateCheckpointAsync(int id)
        {
            await Task.Delay(10);

            Checkpoint nuevo = new Checkpoint(id);

            Checkpoint.listado.Add(nuevo);

            return nuevo;
        }

        public async Task UpdateCheckpointAsync(Checkpoint item)
        {
            for (int i = 0; i < Checkpoint.listado.Count; i++)
            {
                if (Checkpoint.listado[i].idEvento == item.idEvento)
                {
                    Checkpoint.listado[i] = new Checkpoint(item);
                }
            }

            await Task.Delay(10);
        }

        public async Task<Usuario> CheckUserLogin(Usuario usuario)
        {
            await Task.Delay(10);

            if(usuario.usuario == "usuario" && usuario.pass == "pass")
            {
                usuario.idUsuario = 1;
                return usuario;
            }
            else
            {
                return null;
            }
        }

        public async Task<Usuario> GetUsuarioByIDAsync(int userId)
        {
            await Task.Delay(10);

            Usuario user = new Usuario();
            user.idUsuario = userId;
            user.usuario = "usuario";
            return user;
        }
    }
}
