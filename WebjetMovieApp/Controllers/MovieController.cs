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
        public IEnumerable<Movie> GetMovies(string page)
        {
            //this must be done as page by page 
            return DataProvider.GetAllMovies(_appSettings);
        }

        [HttpGet]
        [Route("api/movie")]
        public IEnumerable<MoviePrice> GetMovieDetail(string id)
        {
            return DataProvider.GetMovieDetail(_appSettings, id);
        }

        //[HttpGet]
        //[Route("api/movie")]
        //public MovieDetail GetMovieDetail(string id)
        //{
        //    return DataProvider.GetMovieDetail(_appSettings, id);
        //}
    }
}