using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.IO;

using Soriana.PPS.Payments.ProgramaLealtad.Services;
using Soriana.PPS.Payments.ProgramaLealtad.Constants;
using Soriana.PPS.Common.Constants;
using Soriana.PPS.Common.DTO.Common;
using Soriana.PPS.Common.DTO.ProgramaLealtad;

namespace Soriana.PPS.Payments.ProgramaLealtad
{
    public class ProgramaLealtadFunction
    {
        #region PrivateFields
        private readonly ILogger<ProgramaLealtadFunction> _Logger;
        private readonly IProgramaLealtadService _ProgramaLealtadService;
        #endregion

        #region Constructor
        public ProgramaLealtadFunction(ILogger<ProgramaLealtadFunction> logger,
                                        IProgramaLealtadService programaLealtadService)
        {
            _Logger = logger;
            _ProgramaLealtadService = programaLealtadService;
        }
        #endregion

        #region Public Methods
        [FunctionName(ProgramaLealtadConstants.PROGRAMA_LEALTAD_FUNCTION_NAME)]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest request)
        {
            try
            {
                _Logger.LogInformation(string.Format(FunctionAppConstants.FUNCTION_EXECUTING_MESSAGE, ProgramaLealtadConstants.PROGRAMA_LEALTAD_FUNCTION_NAME));
                if (!request.Body.CanSeek)
                    throw new Exception(JsonConvert.SerializeObject(new BusinessResponse() { StatusCode = (int)HttpStatusCode.BadRequest, Description = HttpStatusCode.BadRequest.ToString(), DescriptionDetail = ProgramaLealtadConstants.PROGRAMA_LEALTAD_NO_CONTENT_REQUEST, ContentRequest = null }));
                request.Body.Position = 0;
                string jsonPaymentOrderProcessRequest = await new StreamReader(request.Body).ReadToEndAsync();

                Saldo_Req PPSRequest = JsonConvert.DeserializeObject<Saldo_Req>(jsonPaymentOrderProcessRequest);

                var result = await _ProgramaLealtadService.ProgamaLealtad(PPSRequest);

                return new OkObjectResult(result);
            }
            catch (BusinessException ex)
            {
                _Logger.LogError(ex, ProgramaLealtadConstants.PROGRAMA_LEALTAD_FUNCTION_NAME);
                return new BadRequestObjectResult(ex);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ProgramaLealtadConstants.PROGRAMA_LEALTAD_FUNCTION_NAME);
                return new BadRequestObjectResult(new BusinessResponse()
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Description = string.Concat(HttpStatusCode.InternalServerError.ToString(), CharactersConstants.ESPACE_CHAR, CharactersConstants.HYPHEN_CHAR, CharactersConstants.ESPACE_CHAR, ProgramaLealtadConstants.PROGRAMA_LEALTAD_FUNCTION_NAME),
                    DescriptionDetail = ex
                });
            }
        }
        #endregion
    }
}
