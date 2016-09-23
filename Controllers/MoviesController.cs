using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ODataMovies.Models;
using ODataMovies.Business;
using System.Web.OData;
using System.Web.Http;
using System.Net;
using System.Diagnostics;

namespace ODataMovies.Controllers
{
    public class MoviesController : ODataController
    {
        //[EnableQuery]
        //public IList<Movie> Get()
        //{
        //return m_service.Movies;
        //}

        [EnableQuery]
        public IList<Movie> Get()
        {
            return ig_service.GetMovies();
        }

        public Movie Get(int key)
        {
            IEnumerable<Movie> movie = m_service.Movies.Where(m => m.Id == key);
            if (movie.Count() == 0)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            else
                return movie.FirstOrDefault();
		}

		/// <summary>
		/// Creates a new movie. 
		/// Use the POST http verb.
		/// Set Content-Type:Application/Json
		/// Set body as: { "Id":1,"Title":"Transformers - 4","ReleaseDate":"2015-10-25T00:00:00+05:30","Rating":"FiveStar","Director":{ "FirstName":"Not","LastName":"Sure" } }
		/// </summary>
		/// <param name="movie"></param>
		/// <returns></returns>
		public IHttpActionResult Post([FromBody] Movie movie)
        {
            try
            {
                return Ok<Movie>(m_service.Add(movie));
            }
            catch(ArgumentNullException e)
            {
                Debugger.Log(1, "Error", e.Message);
				return BadRequest();
            }
            catch(ArgumentException e)
            {
                Debugger.Log(1, "Error", e.Message);
				return BadRequest();
			}
            catch(InvalidOperationException e)
            {
                Debugger.Log(1, "Error", e.Message);
				return Conflict();
            }
        }

        /* This works just as well
        public Movie Put(int key, Movie movie)
        {
            if (movie == null) throw new HttpResponseException(HttpStatusCode.BadRequest);

            Movie movieInstance = m_service.Movies.Where(m => m.Id == key).FirstOrDefault();

            if (movieInstance == null) throw new HttpResponseException(HttpStatusCode.NotFound);

            return movieInstance.CopyFrom(movie);
        }*/

        /// <summary>
        /// Saves the entire Movie object to the object specified by key (id). Is supposed to overwrite all properties
        /// Use the PUT http verb
        /// Set Content-Type:Application/Json
        /// Set body as: { "Id":0,"Title":"StarWars - The Force Awakens","ReleaseDate":"2015-10-25T00:00:00+05:30","Rating":"FourStar" }
        /// </summary>
        /// <param name="key"></param>
        /// <param name="movie"></param>
        /// <returns></returns>
        public IHttpActionResult Put(int key, Movie movie)
        {
            try
            {
                movie.Id = key;
                return Ok<Movie>(m_service.Save(movie));
            }
            catch(ArgumentNullException)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            catch(ArgumentException)
            {
				return NotFound();
            }
        }

        /// <summary>
        /// Use the DELETE http verb
        /// Request for odata/Movies(1)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IHttpActionResult Delete(int key)
        {
            if (m_service.Remove(key))
                return Ok();
            else
                return NotFound();            
        }

        /// <summary>
        /// Use the PATCH http Verb
        /// Set Content-Type:Application/Json
        /// Call this using following in request body: { "Rating":"ThreeStar" }        
        /// </summary>
        /// <param name="key"></param>
        /// <param name="moviePatch"></param>
        /// <returns></returns>
        public IHttpActionResult Patch(int key, Delta<Movie> moviePatch)
        {
            Movie movie = m_service.Find(key);
            if (movie == null) return NotFound();
            moviePatch.CopyChangedValues(movie);
            return Ok<Movie>(m_service.Save(movie));
        }

        private DataService m_service = new DataService();
        private IgniteService ig_service = new IgniteService();
    }
}