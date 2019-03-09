using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DynamicExpression.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nano.Eventing.Interfaces;
using Nano.Models;
using Nano.Repository.Interfaces;
using Nano.Security;
using Nano.Security.Models;
using Nano.Web.Hosting;

namespace Nano.Web.Controllers
{
    /// <inheritdoc />
    public class DefaultUserController<TEntity, TCriteria> : DefaultControllerUpdatable<TEntity, TCriteria>
        where TEntity : DefaultEntityUser
        where TCriteria : class, IQueryCriteria, new()
    {
        /// <summary>
        /// Security Manager.
        /// </summary>
        protected virtual SecurityManager SecurityManager { get; }

        /// <inheritdoc />
        protected DefaultUserController(ILogger logger, IRepository repository, IEventing eventing, SecurityManager securityManager) 
            : base(logger, repository, eventing)
        {
            this.SecurityManager = securityManager ?? throw new ArgumentNullException(nameof(securityManager));
        }

        /// <summary>
        /// Signup.
        /// Registers a new user.
        /// </summary>
        /// <param name="signUp">The signuo request.</param>
        /// <param name="cancellationToken">The token used when request is cancelled.</param>
        /// <returns>The created user.</returns>
        /// <response code="201">Created.</response>
        /// <response code="400">Bad Request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="500">Error occured.</response>
        [HttpPost]
        [Route("signup")]
        [AllowAnonymous]
        [Consumes(HttpContentType.JSON, HttpContentType.XML)]
        [Produces(HttpContentType.JSON, HttpContentType.XML)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(Error), (int)HttpStatusCode.InternalServerError)]
        public virtual async Task<IActionResult> SignUp([FromBody][Required]SignUp<TEntity> signUp, CancellationToken cancellationToken = default)
        {
            var identityUser = await this.SecurityManager
                .SignUpAsync(signUp, cancellationToken);

            var entity = signUp.User;

            // BUG: See if it can be mapped as Foreign key, so we dont need the extra property IdentiyUserId, as well as these lines of code.
            entity.IdentityUserId = identityUser.Id;
            entity.Id = Guid.Parse(entity.IdentityUserId); 

            var result = await this.Repository
                .AddAsync(entity, cancellationToken);

            return this.Created("signup", result);
        }
    }
}