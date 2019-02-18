using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebjetMovieApp.Models
{
    /// <summary>
    /// a class to read values from appSettings.json
    /// </summary>
    public class ApiSettings
    {
        public string Servers { get; set; }
        public string ServerUrl { get; set; }
        public string TokenName { get; set; }
        public string TokenValue { get; set; }
    }
}
