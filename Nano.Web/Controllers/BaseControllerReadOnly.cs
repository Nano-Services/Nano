using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DynamicExpression.Entities;
using DynamicExpression.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.Eventing.Interfaces;
using Nano.Models;
using Nano.Models.Interfaces;
using Nano.Services.Interfaces;
using Nano.Web.Controllers.Extensions;

namespace Nano.Web.Controllers
{
    /// <inheritdoc />
    public abstract class BaseControllerReadOnly<TService, TEntity, TIdentity, TCriteria> : BaseController<TService>
        where TService : IService
        where TEntity : class, IEntity, IEntityIdentity<TIdentity>, IEntityWritable
        where TCriteria : class, IQueryCriteria, new()
    {
        /// <inheritdoc />
        protected BaseControllerReadOnly(ILogger logger, TService service, IEventing eventing)
            : base(logger, service, eventing)
        {

        }

        /// <summary>
        /// Gets all models. 
        /// Filtered by query model parameters (pagination and ordering).
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="cancellationToken">The cancellationToken.</param>
        /// <returns>A collection of models.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">The request model is invalid.</response>
        /// <response code="500">An error occured when processing the request.</response>
        [HttpGet]
        [HttpPost]
        [Route("index")]
        [Produces(HttpContentType.JSON, HttpContentType.XML, HttpContentType.HTML)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> Index([FromQuery][FromBody]Query query, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await this.Service
                .GetAllAsync<TEntity>(query ?? new Query(), cancellationToken);

            if (result == null)
                return this.NotFound();

            if (this.Request.IsContentTypeHtml())
                return this.View("Index", result);

            return this.Ok(result);
        }

        /// <summary>
        /// Gets the model, uniquely identified by the supplied identifier.
        /// </summary>
        /// <param name="id">The identifier, that uniquely identifies the model.</param>
        /// <param name="cancellationToken">The cancellationToken.</param>
        /// <returns>Details about the model.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">The request model is invalid.</response>
        /// <response code="404">No results found.</response>
        /// <response code="500">An error occured when processing the request.</response>
        [HttpGet]
        [Route("details/{id}")]
        [Produces(HttpContentType.JSON, HttpContentType.XML, HttpContentType.HTML)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> Details([FromRoute][Required]TIdentity id, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await this.Service
                .GetAsync<TEntity, TIdentity>(id, cancellationToken);

            if (result == null)
                return this.NotFound();

            if (this.HttpContext.Request.IsContentTypeHtml())
                return this.View("Details", result);

            return this.Ok(result);
        }

        /// <summary>
        /// Gets the models, uniquely identified by the supplied array of identifiers.
        /// </summary>
        /// <param name="ids">The identifier, that uniquely identifies the model.</param>
        /// <param name="cancellationToken">The cancellationToken.</param>
        /// <returns>The models.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">The request model is invalid.</response>
        /// <response code="500">An error occured when processing the request.</response>>
        [HttpPost]
        [Route("details")]
        [Produces(HttpContentType.JSON, HttpContentType.XML, HttpContentType.HTML)]
        [ProducesResponseType(typeof(object[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> Details([FromBody][Required]TIdentity[] ids, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await this.Service
                .GetManyAsync<TEntity, TIdentity>(ids, cancellationToken);

            if (result == null)
                return this.NotFound();

            if (this.Request.IsContentTypeHtml())
                return this.View("Index", result);

            return this.Ok(result);
        }

        /// <summary>
        /// Gets the models, matching the query criteria, pagination and ordering.
        /// </summary>
        /// <param name="criteria">The criteria model, containing filters used in the query.</param>
        /// <param name="cancellationToken">The cancellationToken.</param>
        /// <returns>The models.</returns>
        /// <response code="200">Success.</response>
        /// <response code="400">The request model is invalid.</response>
        /// <response code="500">An error occured when processing the request.</response>>
        [HttpGet]
        [HttpPost]
        [Route("query")]
        [Produces(HttpContentType.JSON, HttpContentType.XML, HttpContentType.HTML)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> Query([FromQuery][FromBody][Required]Query<TCriteria> criteria, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await this.Service
                .GetManyAsync<TEntity, TCriteria>(criteria, cancellationToken);

            if (result == null)
                return this.NotFound();

            if (this.Request.IsContentTypeHtml())
                return this.View("Index", result);

            return this.Ok(result);
        }
    }
}