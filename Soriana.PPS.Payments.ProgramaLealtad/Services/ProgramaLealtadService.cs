using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using Soriana.PPS.Common.DTO.ProgramaLealtad;

namespace Soriana.PPS.Payments.ProgramaLealtad.Services
{
    public class ProgramaLealtadService : IProgramaLealtadService
    {
        #region PrivateFields
        //string MBSWebAPI = System.Environment.GetEnvironmentVariable("MBSWebAPI");
        private readonly ILogger<ProgramaLealtadService> _Logger;
        #endregion

        #region Constructor
        public ProgramaLealtadService(ILogger<ProgramaLealtadService> logger)
        {
            _Logger = logger;
        }
        #endregion

        #region Public Methods
        public async Task<Saldo_Res> ProgamaLealtad(Saldo_Req ProgramaLealtadRequest)
        {
            string PPSRequest = JsonConvert.SerializeObject(ProgramaLealtadRequest);
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
