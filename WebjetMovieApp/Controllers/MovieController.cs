using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using WebjetMovieApp.DataAccess;
using WebjetMovieApp.Models;
using Microsoft.Extensions.Options;

namespace WebjetMovieApp.Controllers
{
    [Produces("application/json")]
    public class MovieController : Controller
    {
        private readonly ApiSettings _appSettings;

        public MovieController(IOptions<ApiSettings> appsettings)
        {
            _appSettings = appsettings.Value;
        }
        [HttpGet]
        [Route("api/movies")]
        public IEnumerable<Movie> GetMovies(int page, int pagesize)
        {
            //this must be done as page by page 
            var movies = DataProvider.GetAllMovies(_appSettings) as List<Movie>;

            if (movies != null && movies.Count > 0 && page > 0)
            {
                var startIndex = (page - 1) * pagesize;
                startIndex = (startIndex < 0) ? 0 : startIndex;

                if (startIndex <= movies.Count)
                {
                    if (startIndex + pagesize > movies.Count)
                        pagesize = movies.Count - (startIndex + 1);

                    return movies.GetRange(startIndex, pagesize);
                }
                else
                    return new List<Movie>();
            }
            return movies;
        }

        [HttpGet]
        [Route("api/movie")]
        public Movie GetMovieDetail(string id)
        {
            return DataProvider.GetMovieDetail(_appSettings, id);
        }
    }
}