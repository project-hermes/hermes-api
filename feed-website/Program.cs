using GoogleMaps.LocationServices;
using hermes_api.DAL;
using hermes_api.DTO;
using hermes_api.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Text;

namespace feed_website
{
    public class Program
    {
        static public HermesDbContext Context;
        static public IConfiguration Config;

        public static async Task Main(string[] args)
        {
            Console.WriteLine("Start - feed_website");

            Config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var serviceProvider = new ServiceCollection()
               .AddDbContext<HermesDbContext>(options =>
                   options.UseSqlServer(Config.GetConnectionString("Entities")))
               .BuildServiceProvider();

            var scope = serviceProvider.CreateScope();

            Context = scope.ServiceProvider.GetRequiredService<HermesDbContext>();

            var dives = Context.Remora.Where(r => r.RemoraId > 335).ToList();

            foreach(var dive in dives)
            {
                await PushToPublicWebsiteAsync(dive);
            }

            Console.WriteLine("Stop - feed_website");
            Console.ReadLine();
        }

        private static async Task PushToPublicWebsiteAsync(RemoraDALModel diveDAL)
        {
            var longitude = diveDAL.startLng == 0 ? diveDAL.endLng : diveDAL.startLng;
            var latitude = diveDAL.startLng == 0 ? diveDAL.endLat : diveDAL.startLat;
            var diveDTO = new DiveDTOModel
            {
                deviceId = diveDAL.deviceId,
                diveId = diveDAL.diveId,
                mode = diveDAL.mode,
                start = DateConversion.UnixTimeStampToDateTime(diveDAL.startTime),
                longitude = longitude,
                latitude = latitude,
                //locality = GetLocality(longitude, latitude)
            };

            if (Context.RemoraRecord.Where(r => r.RemoraId == diveDAL.RemoraId).Count() > 0)
            {
                var diveRecordOneMeterDAL = Context.RemoraRecord.Where(r => r.RemoraId == diveDAL.RemoraId && r.depth > 1).Take(1).OrderBy(r => r.depth).FirstOrDefault();

                if (diveRecordOneMeterDAL != null)
                    diveDTO.degreeOneMeter = diveRecordOneMeterDAL.degrees;

                var depthMaxMeter = Context.RemoraRecord.Where(r => r.RemoraId == diveDAL.RemoraId).Max(r => r.depth);
                var diveRecordMaxMeterDAL = Context.RemoraRecord.Where(r => r.RemoraId == diveDAL.RemoraId && r.depth == depthMaxMeter).FirstOrDefault();
                diveDTO.deepMax = diveRecordMaxMeterDAL.depth;
                diveDTO.degreeMax = diveRecordMaxMeterDAL.degrees;
            }

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Config.GetSection("PublicWebsite:Token").Value);

            var diveJson = JsonConvert.SerializeObject(diveDTO);
            var content = new StringContent(diveJson, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(Config.GetSection("PublicWebsite:Url").Value, content);

            Thread.Sleep(3000);

            Console.WriteLine("Insert "+ diveDAL.RemoraId + " - IsSuccessStatusCode: " + response.IsSuccessStatusCode);
        }

        private static string GetLocality(double longitude, double latitude)
        {
            var location = new GoogleLocationService(Config.GetConnectionString("GoogleApiKey"));
            try
            {
                var locality = location.GetAddressFromLatLang(latitude, longitude);

                return locality.City + ", " + locality.State + ", " + locality.Country; ;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return "n.c.";

        }
    }
}