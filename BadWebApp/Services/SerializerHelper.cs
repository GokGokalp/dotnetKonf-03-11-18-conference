using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BadWebApp.Services
{
    public class SerializerHelper
    {
        public Task<string> SerializeIntoJson(object content)
        {
            // Some bad sample for memory allocations.
            string jsonString = JsonConvert.SerializeObject(content, Formatting.Indented, new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.All
            });

            return Task.FromResult(jsonString);
        }
    }
}