using System;
using System.Collections.Generic;

namespace WebjetMovieApp.Models
{
    /// <summary>
    /// a class to cache the backend API server details and it's data. this can be directly save in a file or DB
    /// </summary>
    public class ServerData
    {
        public string ServerName { get; set; }
        public string IdPrefix { get; set; }
        public DateTime LastLoadTime { get; set; }
        public List<Movie> MovieList { get; set; }

        public ServerData(string name, string prefix)
        {
            ServerName = name;
            IdPrefix = prefix;
            LastLoadTime = DateTime.MinValue;
            MovieList = new List<Movie>();
        }
    }
}
