using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;
using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Linq;
using ODataMovies.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ODataMovies.Business
{
    public class IgniteService
    {

        public IgniteService()
        {

        }

        public List<Movie> GetMovies()
        {
            var cfg = new IgniteConfiguration
            {
                BinaryConfiguration = new Apache.Ignite.Core.Binary.BinaryConfiguration(typeof(Movie), typeof(StarRating), typeof(Person)),
                JvmOptions = new List<string> { "-Xms512m", "-Xmx1024m" },
                JvmClasspath = Directory.GetFiles(HttpContext.Current.Server.MapPath(@"~\bin\libs")).Aggregate((x, y) => x + ";" + y)
            };
            Ignition.ClientMode = true;
            var ignite = Ignition.Start(cfg);
            var cache = ignite.GetOrCreateCache<int, Movie>(new CacheConfiguration
            {
                Name = "myMusicCache",
                QueryEntities = new[]
            {
                        new QueryEntity(typeof(int), typeof(Movie))

                    }
            });
            List<Movie> movies = new List<Movie>();
            IQueryable<ICacheEntry<int, Movie>> qry =
                cache.AsCacheQueryable().Where(m => m.Key == 1);

            foreach (ICacheEntry<int, Movie> entry in qry)
            {
                Console.WriteLine("Movie: " + entry.Value);
                movies.Add(entry.Value);
            }


            return movies;
        }
    }
}

