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
                            if (list != null)
                            {
                                server.Value.MovieList = list.Movies;
                                server.Value.LastLoadTime = DateTime.Now;
                                bMoviesUpdated = true;
                            }
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
        public static Movie GetMovieDetail(ApiSettings settings, string id)
        {
            //make sure we have this movie in the collection
            var movieInCache = m_MovieCollection.Where(m => m.ID == id).FirstOrDefault();

            if (movieInCache != null)
            {
                //if we have outdated record - updated it now from the db server
                if (movieInCache.Detail == null || (movieInCache.Detail != null && movieInCache.Detail.LastUpdated.AddHours(24) > DateTime.Now))
                {
                    List<MovieDetail> mDetails = new List<MovieDetail>();

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
                                mDetails.Add(getMovieDetail(response, server.Key));
                            }
                        }
                        catch (Exception ex)
                        {
                            //log exception
                        }
                    }

                    //update the cached version to the latest version with price and other details
                    updateMovieDetail(movieInCache, mDetails);
                }
            }
            return movieInCache;
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
        /// <summary>
        /// Construct the MovieDetail Object from the string
        /// </summary>
        /// <param name="response"></param>
        /// <param name="server"></param>
        /// <returns></returns>
        private static MovieDetail getMovieDetail(string response, string server)
        {
            try
            {
                var movieDetail = JsonConvert.DeserializeObject<MovieDetail>(response);
                if (movieDetail != null)
                {
                    movieDetail.LastUpdated = DateTime.Now;
                    movieDetail.PriceDetail = new List<MoviePrice>();
                    MoviePrice price = new MoviePrice(server);
                    price.Price = string.IsNullOrEmpty(movieDetail.Price) ? movieDetail.Price : "No Price Detail" ;
                    movieDetail.PriceDetail.Add(price);
                }
                return movieDetail;
            }
            catch(Exception ex)
            { }
            return null;
        }

        private static void updateMovieDetail(Movie movie, List<MovieDetail> mDetails)
        {
            if (movie == null)
                return;

            //let us set the detail Object if we need to
            if (movie.Detail == null)
            {
                MovieDetail md = new MovieDetail();
                md.PriceDetail = new List<MoviePrice>();
                foreach(var server in m_ServerDictionary )
                {
                    MoviePrice mp = new MoviePrice(server.Key);
                    mp.Price = "Not Available";
                    md.PriceDetail.Add(mp);
                }
                movie.Detail = md;
            }

            //update the Cached version
            if (mDetails.Count > 0)
            {
                var pList = movie.Detail.PriceDetail;
                bool bOnlyPrice = false;
                for(int i = 0; i < mDetails.Count; i++)
                {
                    var md = mDetails[i];
                    if (md != null)
                    {
                        movie.Detail.UpdateValues(md, bOnlyPrice, m_ServerDictionary.ElementAt(i).Key);
                        bOnlyPrice = true;
                    }
                }
            }

            //make sure we have entries for all the servers in the PriceList
            if (m_ServerDictionary.Count != movie.Detail.PriceDetail.Count)
            {
                foreach (var server in m_ServerDictionary)
                {
                    if (movie.Detail.PriceDetail.Where(x => x.Provider == server.Key).FirstOrDefault() == null)
                    {
                        MoviePrice mp = new MoviePrice(server.Key);
                        mp.Price = "Not Available";
                        movie.Detail.PriceDetail.Add(mp);
                    }
                }

            }
        }
        #endregion
    }
}
