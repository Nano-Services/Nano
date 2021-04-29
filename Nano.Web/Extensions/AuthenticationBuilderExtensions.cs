using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Nano.Security;

namespace Nano.Web.Extensions
{
    /// <summary>
    /// Service Collection Extensions.
    /// </summary>
    public static class AuthenticationBuilderExtensions
    {
        /// <summary>
        /// Adds external logins to the <see cref="AuthenticationBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <param name="options">The <see cref="SecurityOptions"/>.</param>
        /// <returns>The <see cref="AuthenticationBuilder"/>.</returns>
        internal static AuthenticationBuilder AddExternalLogins(this AuthenticationBuilder builder, SecurityOptions options)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            if (options == null) 
                throw new ArgumentNullException(nameof(options));

            foreach (var externalLogin in options.ExternalLogins)
            {
                switch (externalLogin.Name)
                {
                    case "Google":
                    {
                        builder
                            .AddGoogle(x =>
                            {
                                x.ClientId = externalLogin.Id;
                                x.ClientSecret = externalLogin.Secret ?? "N/A";

                                foreach (var scope in externalLogin.Scopes)
                                {
                                    x.Scope
                                        .Add(scope);
                                }
                            });
                        break;
                    }

                    case "Facebook":
                    {
                        builder
                            .AddFacebook(x =>
                            {
                                x.AppId = externalLogin.Id;
                                x.AppSecret = externalLogin.Secret;

                                foreach (var scope in externalLogin.Scopes)
                                {
                                    x.Scope
                                        .Add(scope);
                                }
                            });
                        break;
                    }

                    case "Microsoft":
                        builder
                            .AddMicrosoftAccount(x =>
                            {
                                x.ClientId = externalLogin.Id;
                                x.ClientSecret = externalLogin.Secret;

                                foreach (var scope in externalLogin.Scopes)
                                {
                                    x.Scope
                                        .Add(scope);
                                }
                            });
                        break;

                    default: 
                        throw new NotSupportedException($"External login: {externalLogin.Name} not supported.");
                }
            }

            return builder;
        }
    }
}