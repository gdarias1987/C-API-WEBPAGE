using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MELI.Models
{
    public class Checkpoint 
    {
        // EN ESTE LISTADO DE CONTROL VOY A GUARDAR LOS EVENTOS QUE LLEGUEN; CON SU ULTIMO ESTADO
        public static List<Checkpoint> listado = new List<Checkpoint>()
        {
            new Checkpoint(1)
        };

        public Checkpoint() { }

        public Checkpoint(int id) 
        {
            this.idEvento = id;
            this.estado = "Handling";
            this.subestado = null;
            this.timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public Checkpoint(Checkpoint checkpoint)
        {
            this.idEvento = checkpoint.idEvento;
            this.estado = checkpoint.estado;
            this.subestado = checkpoint.subestado;
            this.timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public int idEvento { get; set; } = -1;

        public string estado { get; set; } = "";

        public String subestado { get; set; } = null;

        public string timestamp { get; set; }

        public string UserId { get; set; }
        
        public string Ip { get; set; }
    }
}
