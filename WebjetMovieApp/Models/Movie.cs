﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.ComponentModel;

namespace WebjetMovieApp.Models
{
    public class Movie
    {
        private string m_id;
        public string Title { get; set; }
        public string Year { get; set; }
        public string ID { get { return m_id; }
            set {
                //substract the server initials from the ID - assume that all server will appenfd 2 char wide server initial to ID string
                m_id = value.Length > 2 ? value.Substring(2, value.Length - 2) : value;
            }
        }
        public string Type { get; set; }
        public string Poster { get; set; }
    }

    public class MovieDetail : Movie
    {
        public string Rated { get; set; }
        public string Released { get; set; }
        public string Runtime { get; set; }
        public string Genre { get; set; }
        public string Director { get; set; }
        public string Writer { get; set; }
        public string Actors { get; set; }
        public string Plot { get; set; }
        public string Language { get; set; }
        public string Country { get; set; }
        public string Awards { get; set; }
        public string Metascore { get; set; }
        public string Rating { get; set; }
        public string Votes { get; set; }
        public string Price { get; set; }

        [JsonIgnore]
        public List<MoviePrice> PriceDetail { get; set; }
    }

    public class API_Movies
    {
        public List<Movie> Movies { get; set; }
    }

    public class MoviePrice
    {
        public string Provider { get; set; }

        [DefaultValue("Not Available")]
        public string Price { get; set; }

        public MoviePrice(string provider)
        {
            Provider = provider;
        }
    }

    public class EqualityComparer : IEqualityComparer<Movie>
    {

        public bool Equals(Movie m1, Movie m2)
        {
            return m1.ID == m2.ID;
        }

        public int GetHashCode(Movie m)
        {;
            int result = 0;
            Int32.TryParse(m.ID, out result);
            return result;
        }
    }
}
