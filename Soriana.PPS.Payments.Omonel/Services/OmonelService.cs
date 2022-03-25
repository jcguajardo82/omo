using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using Soriana.PPS.Common.DTO.ProgramaLealtad;

namespace Soriana.PPS.Payments.Omonel.Services
{
    public class OmonelService : IOmonelService
    {
        #region Private Fields
        private readonly ILogger<OmonelService> _Logger;
        #endregion

        #region Constructor
        public OmonelService(ILogger<OmonelService> logger)
        {
            _Logger = logger;
        }
        #endregion

        #region Public Method
        public async Task<Saldo_Res> OmonelPayment(Omonel_Req OmonelRequest)
        {
            string PPSRequest = JsonConvert.SerializeObject(OmonelRequest);
            string MBSWebAPI = System.Environment.GetEnvironmentVariable("MBSWebAPI");

            #region HTTP 
            FmkTools.RestResponse responseApi = FmkTools.RestClient.RequestRest_1(FmkTools.HttpVerb.POST, MBSWebAPI, null, PPSRequest);
            string jsonResponse = responseApi.message;

            Saldo_Res PPSResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<Saldo_Res>(jsonResponse);

            return PPSResponse;
            #endregion
        }
        #endregion
    }
}
