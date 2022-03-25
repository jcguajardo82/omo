using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

using Soriana.PPS.Payments.Omonel.Services;
using Soriana.PPS.Payments.Omonel.Constants;
using Soriana.PPS.Common.Constants;
using Soriana.PPS.Common.DTO.Common;
using System.IO;
using Soriana.PPS.Common.DTO.ProgramaLealtad;

namespace Soriana.PPS.Payments.Omonel
{
    public class OmonelPaymentFunction
    {
        #region Private Fields
        private readonly ILogger<OmonelPaymentFunction> _Logger;
        private readonly IOmonelService _OmonelService;
        #endregion

        #region Constructor
        public OmonelPaymentFunction(ILogger<OmonelPaymentFunction> logger,
                                    IOmonelService omonelService)
        {
            _Logger = logger;
            _OmonelService = omonelService;
        }
        #endregion

        #region Public Methods
        [FunctionName(OmonelConstants.OMONEL_PAYMENT_FUNCTION_NAME)]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest request)
        {
            try
            {
                _Logger.LogInformation(string.Format(FunctionAppConstants.FUNCTION_EXECUTING_MESSAGE, OmonelConstants.OMONEL_PAYMENT_FUNCTION_NAME));
                if (!request.Body.CanSeek)
                    throw new Exception(JsonConvert.SerializeObject(new BusinessResponse() { StatusCode = (int)HttpStatusCode.BadRequest, Description = HttpStatusCode.BadRequest.ToString(), DescriptionDetail = OmonelConstants.OMONEL_PAYMENT_NO_CONTENT_REQUEST, ContentRequest = null }));
                request.Body.Position = 0;
                string jsonPaymentOrderProcessRequest = await new StreamReader(request.Body).ReadToEndAsync();

                Omonel_Req OmonelRequest = JsonConvert.DeserializeObject<Omonel_Req>(jsonPaymentOrderProcessRequest);

                Saldo_Res result = await _OmonelService.OmonelPayment(OmonelRequest);

                return new OkObjectResult(result);
            }
            catch (BusinessException ex)
            {
                _Logger.LogError(ex, OmonelConstants.OMONEL_PAYMENT_FUNCTION_NAME);
                return new BadRequestObjectResult(ex);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, OmonelConstants.OMONEL_PAYMENT_FUNCTION_NAME);
                return new BadRequestObjectResult(new BusinessResponse()
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Description = string.Concat(HttpStatusCode.InternalServerError.ToString(), CharactersConstants.ESPACE_CHAR, CharactersConstants.HYPHEN_CHAR, CharactersConstants.ESPACE_CHAR, OmonelConstants.OMONEL_PAYMENT_FUNCTION_NAME),
                    DescriptionDetail = ex
                });
            }
        }
        #endregion
    }
}
