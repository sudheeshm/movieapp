using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebjetMovieApp.Models;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebjetMovieApp.DataAccess
{
    public static class DataProvider
    {
        private static Dictionary<string, ServerData> m_ServerDictionary = new Dictionary<string, ServerData>();
        private static List<Movie> m_MovieCollection = new List<Movie>();
        private static ApiSettings m_ApiSettings = null;

        #region  public functions

        /// <summary>
        /// Get the list of movies from all backend servers and return the unique list of movies.
        /// This list is only fetched once in a day as the assumption is that the movie database in the servers
        /// are only updated once in a day. Currently this routine hit those servers on every 24 hours.
        /// This value of 24 hours can be very well configured in the AppSettings to read from there and lso to reload on a fixed time.
        /// Such details are not implemented for simplicity
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Movie> GetAllMovies(ApiSettings settings)
        {
            m_ApiSettings = settings;
            bool bMoviesUpdated = false;

            //create a dictionary of servers (this gives more freedom to add more servers later)
            initialiseServerLoadTime();

            //go thru each api-server details configured in the settings file and fetch data if needed.
            for (int i = 0; i < m_ServerDictionary.Count; i++)
            {
                var server = m_ServerDictionary.ElementAt(i);
                if (server.Value.LastLoadTime.AddHours(24) <= DateTime.Now)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(server.Key) && m_ApiSettings != null)
                        {
                            string apiURL = m_ApiSettings.ServerUrl + server.Key + "/movies";

                            var taskitem = System.Threading.Tasks.Task.Run(() => getServerData(apiURL));
                            taskitem.Wait();

                            var response = taskitem.Result;
                            var list = JsonConvert.DeserializeObject<API_Movies>(response);
                            server.Value.MovieList = list.Movies;
                            server.Value.LastLoadTime = DateTime.Now;
                            bMoviesUpdated = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        //log exception
                    }
                }
            }

            //if we fetched the latest collection from the server, refresh our collection as well
            if (bMoviesUpdated)
            {
                var list = new List<Movie>();
                foreach (var server in m_ServerDictionary)
                {
                    list = list.Union(server.Value.MovieList).Distinct(new EqualityComparer()).ToList();
                }
                m_MovieCollection.Clear();
                m_MovieCollection.AddRange(list);
            }
            return m_MovieCollection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        ///
        //public static MovieDetail GetMovieDetail(ApiSettings settings, string id)
        public static List<MoviePrice> GetMovieDetail(ApiSettings settings, string id)
        {
            MovieDetail movieToReturn = null;
            var mPrices = new List<MoviePrice>();

            //make sure we have this movie in the collection
            var match = m_MovieCollection.Where(m => m.ID == id).FirstOrDefault();

            if (match != null)
            {
                for (int i = 0; i < m_ServerDictionary.Count; i++)
                {
                    var server = m_ServerDictionary.ElementAt(i);
                    try
                    {
                        if (!string.IsNullOrEmpty(server.Key) && m_ApiSettings != null)
                        {
                            string apiURL = m_ApiSettings.ServerUrl + server.Key + "/movie/" + server.Value.IdPrefix + id;

                            var taskitem = System.Threading.Tasks.Task.Run(() => getServerData(apiURL));
                            taskitem.Wait();

                            var response = taskitem.Result;
                            var movieDetail = JsonConvert.DeserializeObject<MovieDetail>(response);
                            var mprice = new MoviePrice(server.Key);

                            if (movieToReturn == null && movieDetail != null)
                            {
                                movieToReturn = movieDetail;
                                movieToReturn.PriceDetail = new List<MoviePrice>();
                                movieToReturn.Price = "";
                            }
                            if(movieToReturn  != null)
                            {
                                if (movieDetail != null)
                                    mprice.Price = !string.IsNullOrEmpty(movieDetail.Price) ? movieDetail.Price : "Unavailable";
                                movieToReturn.PriceDetail.Add(mprice);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //log exception
                    }
                }
            }
            //return movieToReturn;
            return movieToReturn != null ? movieToReturn.PriceDetail : null;
        }
        #endregion

        #region  private functions

        /// <summary>
        /// asyncronous routine to fetch data from server
        /// </summary>
        /// <param name="apiUrl"></param>
        /// <returns></returns>
        private static async Task<string> getServerData(string apiUrl)
        {
            string retJson = "";
            if (string.IsNullOrEmpty(apiUrl) || m_ApiSettings == null)
                return retJson;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                client.DefaultRequestHeaders.Add(m_ApiSettings.TokenName, m_ApiSettings.TokenValue);
                var response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var mediaType = response.Content.Headers.ContentType.MediaType;
                    if (mediaType == "application/json")
                    {
                        retJson = await response.Content.ReadAsStringAsync();
                    }
                }
            }
            return retJson;
        }

        /// <summary>
        /// Initialise the cache collection
        /// </summary>
        private static void initialiseServerLoadTime()
        {
            if(m_ApiSettings != null)
            {
                var servers = m_ApiSettings.Servers.Split(',').Select(p => p.Trim()).ToList();
                foreach (var server in servers)
                {
                    var details = server.Split(':').ToList();
                    if (!m_ServerDictionary.ContainsKey(details[0]))
                    {
                        m_ServerDictionary.Add(details[0], new ServerData(details[0], details[1]));
                    }
                }
            }
        }
        #endregion
    }
}
