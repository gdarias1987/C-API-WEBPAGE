using Dapper;
using MELI.Models;
using MELI.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MELI.Services
{
    public class DataServiceMySQL : IDataService
    {
        private readonly IConfiguration _configuration;
        private readonly ILoggerService _loggerService;
        private readonly string _conString;
        private static MySqlConnection _conn;

        public DataServiceMySQL(IConfiguration configuration, ILoggerService loggerService)
        {
            _configuration = configuration;
            _loggerService = loggerService;
            _conString = (configuration["ConnectionString:MySQL"]);
        }


        public async Task<List<Checkpoint>> GetCheckpointAsync()
        {
            string query = "SELECT * FROM EventosDev;";

            try
            {
                using (_conn = new MySqlConnection(_conString))
                {
                    var result = await _conn.QueryAsync<Checkpoint>(query);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                _loggerService.recordLogError(_loggerService.GetLastMethodName(), ex.Message, "GetCheckPointAsync");
            }

            return null;
        }

        public async Task<Checkpoint> GetCheckpointByIDAsync(int idEvento)
        {
            string query = "SELECT * FROM EventosDev WHERE idEvento = @idEvento;";

            try
            {
                using (_conn = new MySqlConnection(_conString))
                {
                    var result = await _conn.QueryFirstOrDefaultAsync<Checkpoint>(query, new { idEvento });
                    return result;
                }
            }
            catch (Exception ex)
            {
                _loggerService.recordLogError(_loggerService.GetLastMethodName(), ex.Message, "GetCheckpointByIDAsync");
            }

            return null;
        }

        public async Task<Checkpoint> CreateCheckpointAsync(int id)
        {
            Checkpoint checkpoint = new Checkpoint(id);

            string query = "INSERT INTO EventosDev (idEvento, estado, subestado, timestamp) VALUES ( @idEvento, @estado, @subestado, @timestamp );";

            try
            {
                using (_conn = new MySqlConnection(_conString))
                {
                    await _conn.QueryAsync(query, new { checkpoint.idEvento, checkpoint.estado, checkpoint.subestado, checkpoint.timestamp });
                }
            }
            catch (MySqlException ex)
            {
                _loggerService.recordLogError(_loggerService.GetLastMethodName(), ex.Message, "CreateCheckpointAsync");
                return null;
            }

            return checkpoint;
        }

        public async Task UpdateCheckpointAsync(Checkpoint item)
        {
            item.timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            string query = "UPDATE EventosDev SET estado = @estado, subestado = @subestado, timestamp = @timestamp WHERE idEvento = @idEvento;";

            try
            {
                using (_conn = new MySqlConnection(_conString))
                {
                    await _conn.QueryAsync(query, new { item.idEvento, item.estado, item.subestado, item.timestamp });
                }
            }
            catch (Exception ex)
            {
                _loggerService.recordLogError(_loggerService.GetLastMethodName(), ex.Message, "UpdateCheckpointAsync");
            }
        }

        public async Task<Usuario> CheckUserLogin(Usuario usuario)
        {
            string query = "SELECT * FROM UsuarioDev WHERE usuario = @usuario AND pass = @pass;";

            try
            {
                using (_conn = new MySqlConnection(_conString))
                {
                    var result = await _conn.QuerySingleOrDefaultAsync<Usuario>(query, new { usuario.usuario, usuario.pass });
                    usuario = result as Usuario;
                }
            }
            catch (Exception ex)
            {
                _loggerService.recordLogError(_loggerService.GetLastMethodName(), ex.Message, "usuario: " + usuario.usuario );
            }

            return usuario;
        }

        public async Task<Usuario> GetUsuarioByIDAsync(int userId)
        {
            Usuario usuario = null;
            string query = "SELECT * FROM UsuarioDev WHERE id = @id;";

            try
            {
                using (_conn = new MySqlConnection(_conString))
                {
                    var result = await _conn.QueryAsync<Usuario>(query, new { userId });
                    usuario = result as Usuario;
                }
            }
            catch (MySqlException ex)
            {
                _loggerService.recordLogError(_loggerService.GetLastMethodName(), ex.Message, "userId: " + userId);
            }

            return usuario;
        }

    }
}
