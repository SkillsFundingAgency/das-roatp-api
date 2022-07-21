using Newtonsoft.Json;

namespace SFA.DAS.Roatp.Api.Models
{
    public class PatchOperation
    {
        [JsonProperty("value")]
        public object Value { get; set; }
        [JsonProperty("path")]
        public string Path { get; set; }
        [JsonProperty("op")]
        public string Op { get; set; } 
    }

}
