using System.Threading.Tasks;

using Soriana.PPS.Common.DTO.ProgramaLealtad;

namespace Soriana.PPS.Payments.Omonel.Services
{
    public interface IOmonelService
    {
        Task<Saldo_Res> OmonelPayment(Omonel_Req OmonelRequest);
    }
}
