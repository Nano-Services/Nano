﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DynamicExpression.Interfaces;
using Nano.Models;
using Nano.Models.Auth;
using Nano.Models.Interfaces;
using Nano.Web.Api.Requests;
using Nano.Web.Api.Requests.Auth;
using Nano.Web.Api.Requests.User;

namespace Nano.Web.Api
{
    // BUG: Find better way to define Api. SignIn, SignOut, SignUp etc shouldn't be part of all apis / Controllers.

    /// <summary>
    /// Default Api.
    /// </summary>
    public class DefaultApi : BaseApi
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="apiOptions">The <see cref="Api.ApiOptions"/>.</param>
        public DefaultApi(ApiOptions apiOptions)
            : base(apiOptions)
        {
        }

        /// <summary>
        /// Log In Async.
        /// </summary>
        /// <param name="request">The <see cref="LogInRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The <see cref="AccessToken"/>.</returns>
        public virtual async Task<AccessToken> LogInAsync(LogInRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.CustomAsync<LogInRequest, AccessToken>(request, cancellationToken);
        }

        /// <summary>
        /// Log Out Async.
        /// </summary>
        /// <param name="request">The <see cref="LogOutRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>Void..</returns>
        public virtual async Task LogOutAsync(LogOutRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            await this.CustomAsync(request, cancellationToken);
        }
                
        /// <summary>
        /// Sign Up Async.
        /// </summary>
        /// <param name="request">The <see cref="SingUpRequest{TUser}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The instance of <typeparamref name="TUser"/>.</returns>
        public virtual async Task<TUser> SignUpAsync<TUser>(SingUpRequest<TUser> request, CancellationToken cancellationToken = default)
            where TUser : DefaultEntityUser
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.CustomAsync<SingUpRequest<TUser>, TUser>(request, cancellationToken);
        }

        /// <summary>
        /// Get External Schemes Async.
        /// </summary>
        /// <param name="request">The <see cref="ExternalSchemesRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A collection of <see cref="ExternalScheme"/>'s.</returns>
        public virtual async Task<IEnumerable<ExternalScheme>> GetExternalSchemesAsync(ExternalSchemesRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.CustomAsync<ExternalSchemesRequest, IEnumerable<ExternalScheme>>(request, cancellationToken);
        }

        /// <summary>
        /// Index.
        /// Invokes the 'index' endpoint of the api.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="request">The <see cref="IndexRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The matching entities.</returns>
        public virtual async Task<IEnumerable<TEntity>> IndexAsync<TEntity>(IndexRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.Invoke<IndexRequest, IEnumerable<TEntity>>(request, cancellationToken);
        }

        /// <summary>
        /// Details.
        /// Invokes the 'details' endpoint of the api.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="request">The <see cref="DetailsRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The matching entity.</returns>
        public virtual async Task<TEntity> DetailsAsync<TEntity>(DetailsRequest request, CancellationToken cancellationToken = default)
            where TEntity: class
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.Invoke<DetailsRequest, TEntity>(request, cancellationToken);
        }

        /// <summary>
        /// Details Many.
        /// Invokes the 'details' endpoint of the api, with multiple id's.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="request">The <see cref="DetailsManyRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The matching entities.</returns>
        public virtual async Task<IEnumerable<TEntity>> DetailsManyAsync<TEntity>(DetailsManyRequest request, CancellationToken cancellationToken = default)
            where TEntity: class
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.Invoke<DetailsManyRequest, IEnumerable<TEntity>>(request, cancellationToken);
        }

        /// <summary>
        /// Query.
        /// Invokes the 'query' endpoint of the api.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <typeparam name="TCriteria">The criteria type</typeparam>
        /// <param name="request">The <see cref="QueryRequest{TCriteria}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The matching entities.</returns>
        public virtual async Task<IEnumerable<TEntity>> QueryAsync<TEntity, TCriteria>(QueryRequest<TCriteria> request, CancellationToken cancellationToken = default) 
            where TEntity: class
            where TCriteria : IQueryCriteria, new()
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.Invoke<QueryRequest<TCriteria>, IEnumerable<TEntity>>(request, cancellationToken);
        }

        /// <summary>
        /// Query.
        /// Invokes the 'query/first' endpoint of the api.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <typeparam name="TCriteria">The criteira type</typeparam>
        /// <param name="request">The <see cref="QueryFirstRequest{TCriteria}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The first match entity.</returns>
        public virtual async Task<TEntity> QueryFirstAsync<TEntity, TCriteria>(QueryFirstRequest<TCriteria> request, CancellationToken cancellationToken = default) 
            where TEntity: class
            where TCriteria : IQueryCriteria, new()
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.Invoke<QueryFirstRequest<TCriteria>, TEntity>(request, cancellationToken);
        }

        /// <summary>
        /// Query.
        /// Invokes the 'query/first' endpoint of the api.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <typeparam name="TCriteria">The criteria type</typeparam>
        /// <param name="request">The <see cref="QueryCountRequest{TCriteria}"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The count of matching entities.</returns>
        public virtual async Task<int> QueryCountAsync<TEntity, TCriteria>(QueryCountRequest<TCriteria> request, CancellationToken cancellationToken = default) 
            where TEntity: class, IEntity
            where TCriteria : IQueryCriteria, new()
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.Invoke<TEntity, QueryCountRequest<TCriteria>, int>(request, cancellationToken);
        }

        /// <summary>
        /// Create.
        /// Invokes the 'create' endpoint of the api.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="request">The <see cref="CreateRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The created entity.</returns>
        public virtual async Task<TEntity> CreateAsync<TEntity>(CreateRequest request, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityCreatable
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.Invoke<CreateRequest, TEntity>(request, cancellationToken);
        }

        /// <summary>
        /// Create Many.
        /// Invokes the 'create/many' endpoint of the api.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="request">The <see cref="CreateManyRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The created entities.</returns>
        public virtual async Task<IEnumerable<TEntity>> CreateManyAsync<TEntity>(CreateManyRequest request, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityCreatable
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.Invoke<CreateManyRequest, IEnumerable<TEntity>>(request, cancellationToken);
        }

        /// <summary>
        /// Edit.
        /// Invokes the 'edit' endpoint of the api.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="request">The <see cref="EditRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The updated entity.</returns>
        public virtual async Task<TEntity> EditAsync<TEntity>(EditRequest request, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityUpdatable
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.Invoke<EditRequest, TEntity>(request, cancellationToken);
        }

        /// <summary>
        /// Edit Many.
        /// Invokes the 'Edit/many' endpoint of the api.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="request">The <see cref="EditManyRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The updated entities.</returns>
        public virtual async Task<IEnumerable<TEntity>> EditManyAsync<TEntity>(EditManyRequest request, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityUpdatable
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.Invoke<EditManyRequest, IEnumerable<TEntity>>(request, cancellationToken);
        }

        /// <summary>
        /// Edit Many.
        /// Invokes the 'Edit/query' endpoint of the api.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="request">The <see cref="EditManyRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>The updated entities.</returns>
        public virtual async Task<IEnumerable<TEntity>> EditQueryAsync<TEntity>(EditQueryRequest request, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityUpdatable
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return await this.Invoke<EditQueryRequest, IEnumerable<TEntity>>(request, cancellationToken);
        }

        /// <summary>
        /// Delete.
        /// Invokes the 'delete' endpoint of the api.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="request">The <see cref="DeleteRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>Nothing.</returns>
        public virtual async Task DeleteAsync<TEntity>(DeleteRequest request, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityDeletable
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            await this.Invoke<DeleteRequest, TEntity>(request, cancellationToken);
        }

        /// <summary>
        /// Delete Many.
        /// Invokes the 'delete/many' endpoint of the api.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="request">The <see cref="DeleteManyRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>Nothing.</returns>
        public virtual async Task DeleteManyAsync<TEntity>(DeleteManyRequest request, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityDeletable
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            await this.Invoke<DeleteManyRequest, TEntity>(request, cancellationToken);
        }

        /// <summary>
        /// Delete Query.
        /// Invokes the 'delete/query' endpoint of the api.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="request">The <see cref="DeleteManyRequest"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>Nothing.</returns>
        public virtual async Task DeleteQueryAsync<TEntity>(DeleteQueryRequest request, CancellationToken cancellationToken = default)
            where TEntity : class, IEntityDeletable
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            await this.Invoke<DeleteQueryRequest, TEntity>(request, cancellationToken);
        }
    }
}