using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebjetMovieApp.Controllers;
using WebjetMovieApp.Models;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace MovieAppUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        private ApiSettings appSetting = new ApiSettings();
        [TestMethod]
        public void InValidAppSettings()
        {
            IOptions<ApiSettings> myOptions = Options.Create(appSetting);

            MovieController mc = new MovieController(myOptions);
            var movies = mc.GetMovies("1") as List<Movie>;

            Assert.IsFalse(movies != null);
        }
    }


}
