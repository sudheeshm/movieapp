using NUnit.Framework;
using WebjetMovieApp.Controllers;
using WebjetMovieApp.Models;
using System.Collections.Generic;
using Microsoft.Extensions.;

namespace Tests
{
    public class MovieControllerTests
    {

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestGetMovies()
        {
            ApiSettings apisettings = new ApiSettings();
            MovieController ctrl = new MovieController(<IObject>apisettings);
            List<Movie> result = ctrl.GetMovies() as List<Movie>;
            Assert.Pass();
        }
    }
}