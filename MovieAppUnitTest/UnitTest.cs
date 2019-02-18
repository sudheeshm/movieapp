using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebjetMovieApp.Controllers;
using WebjetMovieApp.Models;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System;

namespace MovieAppUnitTest
{
    [TestClass]
    public class UnitTest
    {
        private ApiSettings invalid_appSetting = new ApiSettings();
        private ApiSettings appSetting = new ApiSettings();
        private List<Movie> m_Loaded_Movies = null;
        MovieController m_MovieController = null;

        public UnitTest()
        {
            appSetting.Servers = "cinemaworld:cw, filmworld:fw";
            appSetting.ServerUrl = "http://webjetapitest.azurewebsites.net/api/";
            appSetting.TokenName = "x-access-token";
            appSetting.TokenValue = "";     //fill the token
        }

        [TestMethod]
        public void InValidAppSettingsTest()
        {
            IOptions<ApiSettings> myOptions = Options.Create(invalid_appSetting);
            var mc = new MovieController(myOptions);
            var movies = mc.GetMovies(0, 10) as List<Movie>;

            Assert.IsTrue(movies == null || movies.Count == 0);
        }

        [TestMethod]
        public void ValidAppSettingsTest()
        {
            initialiseMovieController();
            var movies = m_MovieController.GetMovies(0, 10) as List<Movie>;

            int result = movies != null ? movies.Count : 0;

            Assert.IsFalse(result < 0);
        }

        [TestMethod]
        public void PageTest()
        {
            initialiseMovieController();
            LoadMovies();
 
            var movies = m_MovieController.GetMovies(0, 0) as List<Movie>;
            int result = movies != null ? movies.Count : 0;

            Assert.IsTrue(result <= m_Loaded_Movies.Count);
        }

        [TestMethod]
        public void PageWithRandomPageSizeTest()
        {
            initialiseMovieController();
            LoadMovies();

            Random rnd = new Random();
            int pagesize = rnd.Next(1, m_Loaded_Movies.Count);

            var movies = m_MovieController.GetMovies(1, pagesize) as List<Movie>;
            int result = movies != null ? movies.Count : 0;

            Assert.IsTrue(result <= pagesize);
        }

        [TestMethod]
        public void PageWithInValidValuesTest()
        {
            initialiseMovieController();
            LoadMovies();

            Random rnd = new Random();
            int page = rnd.Next(10000, 1000000);
            int pagesize = rnd.Next(10000, 1000000);

            var movies = m_MovieController.GetMovies(page, pagesize) as List<Movie>;
            int result = movies != null ? movies.Count : 0;

            Assert.IsTrue(result >= 0 && result <= m_Loaded_Movies.Count);
        }

        [TestMethod]
        public void GetMovieWithValidIDTest()
        {
            initialiseMovieController();
            LoadMovies();
            if (m_Loaded_Movies.Count > 0)
            {
                Random rnd = new Random();
                int index = rnd.Next(0, m_Loaded_Movies.Count - 1);              
                var movie = m_MovieController.GetMovieDetail(m_Loaded_Movies[index].ID) as Movie;
                Assert.IsTrue(movie != null && movie.ID == m_Loaded_Movies[index].ID);
            }
        }

        [TestMethod]
        public void GetMovieWithInvalidIDTest()
        {
            initialiseMovieController();
            LoadMovies();
            var movie = m_MovieController.GetMovieDetail("abcdefg") as Movie;
            Assert.IsTrue(movie == null);
        }

        [TestMethod]
        public void GetdMovieWithEmptyIDTest()
        {
            initialiseMovieController();
            LoadMovies();
            var movie = m_MovieController.GetMovieDetail("") as Movie;
            Assert.IsTrue(movie == null);
        }

        #region private functions
        private void LoadMovies()
        {
            initialiseMovieController();
            m_Loaded_Movies = m_MovieController.GetMovies(0, 0) as List<Movie>;
        }

        private void initialiseMovieController()
        {
            if (m_MovieController == null)
            {
                IOptions<ApiSettings> myOptions = Options.Create(appSetting);
                m_MovieController = new MovieController(myOptions);
            }
        }
        #endregion
    }


}
