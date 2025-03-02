using System;
using System.Text.Json.Serialization;

/// <summary>
/// Overall, This Code Base Is The Student Model
/// This Code Has Fields That Will Serialized By JSON With Corresponding Property Name
/// </summary>

namespace SMIS.models
{
    public enum Sex { MALE, FEMALE }
    public enum Strand { STEM, HUMSS, ABM, GAS, ICT, EIM, SMAW, HE, SPORTS }
    public class Student
    {
        [JsonPropertyName("id")]
        public required int id { get; set; }

        [JsonPropertyName("name")]
        public required string name { get; set; } = string.Empty;
        
        [JsonPropertyName("sex")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required Sex sex { get; set; }

        [JsonPropertyName("age")]
        public required int age { get; set; }

        [JsonPropertyName("address")]
        public required string address { get; set; } = string.Empty;
        
        [JsonPropertyName("strand")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required Strand strand { get; set; }

        /// <remarks>
        /// NOTE ------------------
        /// THIS WILL SOON BE IMPLEMENTED :)
        /// STAY TUNED :)
        /// </remarks>
        // [JsonPropertyName("grade")]
        // public required int grade { get; set; }
    }
}