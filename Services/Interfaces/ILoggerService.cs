using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MELI.Services.Interfaces
{
    public interface ILoggerService
    {
        void recordLogError(string modulo, string err, string user);

        string GetLastMethodName();

        List<string> getLogFiles();
        
        Task<List<ErrLog>> getLogData(string dia);
        Task recordLogMsj(string modulo, string msj, string user);
    }
}
