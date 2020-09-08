using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MELI.Models;
using MELI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MELI.Pages
{
    //[Authorize(Roles = "ADMINISTRADOR")]
    [Route("api/[controller]")]
    [ApiController]
    public class checkpoint : ControllerBase
    {
        private readonly IDataService _dataService;

        public checkpoint(IDataService dataService)
        {
            _dataService = dataService;
        }

        [HttpPut]
        public async Task<ActionResult> handleEnvio([FromBody] List<Checkpoint> envio)
        {
            if (!ModelState.IsValid)
            {
                return handleErr();
            }

            // POR CADA CHECKPOINT DEL ENVIO; VOY A ACTUALIZAR EL LISTADO DE CONTROL
            foreach (Checkpoint item in envio)
            {
                handleCheckpoint(item);
            }

            List<Checkpoint> checkpoints = await _dataService.GetCheckpointAsync();

            return new JsonResult(checkpoints.OrderByDescending(C => C.idEvento).ToList());
        }

        [HttpPost("{id}")]
        public async Task<ActionResult> createCheckpoint([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return handleErr();
            }

            Checkpoint control = await _dataService.GetCheckpointByIDAsync(id);

            if (control != null)
            {
                return Conflict();
            }
            else
            {
                Checkpoint nuevo = await _dataService.CreateCheckpointAsync(id);

                return new JsonResult(nuevo);
            }
        }


        [HttpGet]
        public async Task<ActionResult> getAllCheckpoints()
        {
            List<Checkpoint> checkpoints = await _dataService.GetCheckpointAsync();

            return new JsonResult(checkpoints.OrderByDescending(C => C.idEvento).ToList());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> getCheckpointByID([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return handleErr();
            }

            Checkpoint control = await _dataService.GetCheckpointByIDAsync(id);

            if ( control == null)
            {
                return new NotFoundResult();
            }
            else
            {
                return new JsonResult(control);
            }
        }

        private void handleCheckpoint(Checkpoint item)
        {
            // TRAIGO EL EVENTO RELACIONADO AL CHECKPOINT RECIBIDO CON EL ULTIMO ESTADO
            Checkpoint control = Checkpoint.listado.Where(C => C.idEvento == item.idEvento ).FirstOrDefault();

            if ( control != null )
            {
                int actualEstado = getValorEstado(control);
                int nuevoEstado = getValorEstado(item);

                if (nuevoEstado > actualEstado)
                {
                    _dataService.UpdateCheckpointAsync(item);
                }
            }

            #region INFORMACION
            /*

            LE DI UN PESO ESPECIFICO SEGUN LA GERARQUIA PARA PODER ORDER EL ESTADO MAS ALTO

            ● Handling 
            ○ Null  -> 1
            ○ Manufacturing  -> 2
            ● Ready To Ship 
            ○ Ready To Print -> 3
            ○ Printed -> 4
            ● Shipped
            ○ Null -> 5
            ○ Soon Deliver -> 6
            ○ Waiting For Withdrawal -> 7
            ● Delivered
            ○ Null -> 8
            ● Not Delivered
            ○ Lost -> 9
            ○ Stolen -> 10

            */
            #endregion

        }

        private int getValorEstado(Checkpoint item)
        {
            switch (item.estado)
            {
                case "Handling":
                    return item.subestado == null ? 1 : 2;
                case "Ready To Ship":
                    return item.subestado == "Ready To Print" ? 3 : 4 /* Printed */;
                case "Shipped":
                    return item.subestado == null ? 5 : ( item.subestado == "Soon Deliver" ? 6 : 7 /* Waiting For Withdrawal */) ;
                case "Delivered":
                    return 8;
                case "Not Delivered":
                    return item.subestado == "Lost" ? 9 : 10 /* Stolen */;
                default:
                    return -1;
            }
        }

        public JsonResult handleErr()
        {
            List<ModelErrorCollection> errors = ModelState.Select(x => x.Value.Errors)
           .Where(y => y.Count > 0)
           .ToList();

            foreach (var error in errors)
            {
                //_log.recordLogError(_log.GetLastMethodName(), error[0].ErrorMessage, Usuario.userConnected.email);
            }

            var errores = new
            {
                error = errors
            };

            return new JsonResult(errores);
        }
    }

}
