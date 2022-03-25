using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

using Soriana.PPS.Common.Configuration;
using Soriana.PPS.Common.Extensions;
using Soriana.PPS.Common.Mapping.AutoMapper.Converters;
using Soriana.PPS.Common.Constants;
using Soriana.PPS.Payments.Omonel.Services;

[assembly: FunctionsStartup(typeof(Soriana.PPS.Payments.Omonel.Startup))]
namespace Soriana.PPS.Payments.Omonel
{
    public class Startup: FunctionsStartup
    {
        #region Constructors
        public Startup() { }
        #endregion

        #region Overrides Methods
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            base.ConfigureAppConfiguration(builder);
        }
        public override void Configure(IFunctionsHostBuilder builder)
        {
            //Formatter Injection
            builder.Services.AddMvcCore().AddNewtonsoftJson(options => options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore);
            
            //HttpClientOptions
            builder.Services.AddOptions<HttpClientListOptions>().Configure<IConfiguration>((settings, configuration) =>
            {
                string sectionPath = string.Concat(HttpClientListOptions.HTTP_CLIENT_LIST_OPTIONS, CharactersConstants.COLON_CHAR, HttpClientOptions.HTTP_CLIENT_OPTIONS);
                foreach (HttpClientOptions options in configuration.GetSection(sectionPath).Get<HttpClientOptions[]>())
                    settings.HttpClientOptions.Add(options);
            });
            builder.Services.AddOptions<MerchantDefinedDataOptions>().Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection(MerchantDefinedDataOptions.MERCHANT_DEFINED_DATA_OPTIONS_CONFIGURATION).Bind(settings);
            });

            //Configuration Injection
            IConfiguration configuration = builder.GetContext().Configuration;
            builder.Services.Configure<IConfiguration>(configuration);

            //SeriLog Injection
            builder.Services.AddSeriLogConfiguration(configuration);

            //Business Service Injection -- Service
            builder.Services.AddScoped<IOmonelService, OmonelService>();
        }
        #endregion

    }
}
