using Microsoft.Extensions.DependencyInjection;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using System.Text.Json;
using System.Net.Http.Json;
using System.Configuration;

namespace kpworkersbotmicro
{
    internal static class Sender
    {
        private static readonly HttpClient httpClient;
        public delegate Task Erorrs(string errors);
        public static Erorrs error;
        private static string adress = ConfigurationManager.AppSettings.Get("ServerAdress");

        static Sender()
        {
            var services = new ServiceCollection();
            services.AddHttpClient();
            var serviceProvider = services.BuildServiceProvider();
            var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
            httpClient = httpClientFactory.CreateClient();
        }
        public static async Task<string> CheckWorkerStatus(long id, string? name=null)
        {
            try
            {
                var httpResponseMessage = await httpClient.GetAsync($"{adress}/check/{id}/{name}");
                await Console.Out.WriteLineAsync($"{adress}/check/{id}/{name}");
                var status = httpResponseMessage.StatusCode.ToString();

                await Console.Out.WriteLineAsync("Status " + status);
                await Console.Out.WriteLineAsync(status.GetType().ToString());
                return status.ToLower();
            }
            catch(Exception ex)
            {
                var lastrow= ex.StackTrace.Split("\n").Last();
                error?.Invoke("Ошипка в методе CheckWorkerStatus \ntype\n" + ex.GetType()
                    + "\n" + ex.Message+"\n"+lastrow);
            }
            return null;
            
            
        }
        public static async Task SendPartOfWorkAsync(WorkRezult workRezult)
        {
            await Console.Out.WriteLineAsync("Start send");
           
            var workRezultJson = new StringContent(
                    JsonSerializer.Serialize(workRezult),
                    Encoding.UTF8,
                    Application.Json);
            try
            {
                var httpResponseMessage = await httpClient.PostAsync($"{adress}/newwork", workRezultJson);



                //await Console.Out.WriteLineAsync(status.ToString());
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var contentStream =
                    await httpResponseMessage.Content.ReadAsStringAsync();
                    await Console.Out.WriteLineAsync("Yes");
                    await Console.Out.WriteLineAsync(contentStream);


                    return;

                }
            }
            catch (Exception ex)
            {
                var lastrow = ex.StackTrace.Split("\n").Last();
                error?.Invoke("Ошипка в методе SendPartOfWorkAsync \ntype\n" + ex.GetType()
                    + "\n" + ex.Message + "\n" + lastrow);

            }

        }
        public static async Task<WorkRezult> SendWorkerIDAsync(long id)
        {
                       
            var idItemJson = new StringContent(
                    JsonSerializer.Serialize(id),
                    Encoding.UTF8,
                    Application.Json);
            
            
            try
            {
                var httpResponseMessage = await httpClient.PostAsync($"{adress}/update", idItemJson);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var workRezult = await httpResponseMessage.Content.ReadFromJsonAsync<WorkRezult>();

                    await Console.Out.WriteLineAsync("Yes i get rezulting report");
                    return workRezult;

                }
                return new WorkRezult();
            }
            catch (Exception ex)
            {
                var lastrow = ex.StackTrace.Split("\n").Last();
                error?.Invoke("Ошипка в методе SendWorkerIDAsync \ntype\n" + ex.GetType()
                    + "\n" + ex.Message + "\n" + lastrow);
                return new WorkRezult();
            }
            
        }

        public static async Task<List<WorkerSalary>> GetFastSelect()
        {
            try
            {
                var httpResponseMessage = await httpClient.GetAsync($"{adress}/fast");
                await Console.Out.WriteLineAsync(httpResponseMessage.ToString());
                var list = await httpResponseMessage.Content.ReadFromJsonAsync<List<WorkerSalary>>();
                return list;
            }
            catch (Exception ex)
            {
                var lastrow = ex.StackTrace.Split("\n").Last();
                error?.Invoke("Ошипка в методе GetFastSelect \ntype\n" + ex.GetType()
                    + "\n" + ex.Message + "\n" + lastrow);

            }
            return null;
        }
        public static async Task<List<string>> GetProjects()
        {
            try
            {
                var httpResponseMessage = await httpClient.GetAsync($"{adress}/projects");
                var list = await httpResponseMessage.Content.ReadFromJsonAsync<List<string>>();
                return list;
            }
            catch (Exception ex)
            {
                var lastrow = ex.StackTrace.Split("\n").Last();
                error?.Invoke("Ошипка в методе GetProjects \ntype\n" + ex.GetType()
                    + "\n" + ex.Message + "\n" + lastrow);
            }
            return null;
        }
        public static async Task<List<WorkerSalary>> SelectBetweenTwoDatesAsync(string twoDates)
        {
            var twoDatesJson = new StringContent(
                    JsonSerializer.Serialize(twoDates),
                    Encoding.UTF8,
                    Application.Json);
            try
            {
                var httpResponseMessage = await httpClient.PostAsync($"{adress}/twodates", twoDatesJson);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var list =
                    await httpResponseMessage.Content.ReadFromJsonAsync<List<WorkerSalary>>();
                    return list;
                }
            }
            catch (Exception ex)
            {
                var lastrow = ex.StackTrace.Split("\n").Last();
                error?.Invoke("Ошипка в методе SelectBetweenTwoDatesAsync \ntype\n" + ex.GetType()
                    + "\n" + ex.Message + "\n" + lastrow);
            }
            return null;
        }

    }
}
