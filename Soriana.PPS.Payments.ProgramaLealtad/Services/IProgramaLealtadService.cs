using Soriana.PPS.Common.DTO.ProgramaLealtad;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Soriana.PPS.Payments.ProgramaLealtad.Services
{
    public interface IProgramaLealtadService
    {
        Task<Saldo_Res> ProgamaLealtad(Saldo_Req ProgramaLealtadRequest);
    }
}
