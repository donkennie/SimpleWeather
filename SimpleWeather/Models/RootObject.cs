using Newtonsoft.Json;

namespace SimpleWeather.Models
{
    public class RootObject
    {
        /*  public Coord coord { get; set; }
          public Weather[] weather { get; set; }
          public string _base { get; set; }
          public Main main { get; set; }
          public int visibility { get; set; }
          public Wind wind { get; set; }
          public Clouds clouds { get; set; }
          public long dt { get; set; }
          public Sys sys { get; set; }
          public int timezone { get; set; }
          public int id { get; set; }
          public string name { get; set; }
          public int cod { get; set; }*/
        public Location location { get; set; }
        public Current current { get; set; }
    }

    public class Location
    {
        [JsonProperty("name")]
        public string name { get; set; }
        public string region { get; set; }
        public string country { get; set; }
        public decimal lat { get; set; }
        public decimal lon { get; set; }
        public string tz_id { get; set; }
        public long localtime_epoch { get; set; }
        public DateTime localtime { get; set; }

    }

    public class Current
    {
        public long last_updated_epoch { get; set; }
        public DateTime last_updated { get; set; }
        public decimal temp_c { get; set; }
        public decimal temp_f { get; set; }
        public int is_day { get; set; }
        public Condition condition { get; set; }
        public int wind_mph { get; set; }
        public int wind_kph { get; set; }
        public int wind_degree { get; set; }
        public string wind_dir { get; set; }
        public decimal pressure_mb { get; set; }
        public decimal pressure_in { get; set; }
        public decimal precip_mm { get; set; }
        public decimal precip_in { get; set; }
        public int humidity { get; set; }
        public int cloud { get; set; }
        public decimal feelslike_c { get; set; }
        public decimal feelslike_f { get; set; }
        public decimal vis_km { get; set; }
        public decimal vis_miles { get; set; }
        public int uv { get; set; }
        public decimal gust_mph { get; set; }
        public decimal gust_kph { get; set; }
    }

    public class Condition
    {
        public string text { get; set; }
        public string icon { get; set; }
        public int code { get; set; }

    }
}
