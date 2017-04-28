﻿// ———————————————————————————————
// <copyright file="Bootstrap.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Bootstraps Autofac.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot
{
    using System;
    using System.Linq;
    using System.Web.Configuration;
    using Autofac;
    using Autofac.Extras.AttributeMetadata;
    using Autofac.Integration.Mvc;
    using Autofac.Integration.WebApi;
    using Dialogs;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;

    /// <summary>
    /// Bootstraps Autofac.
    /// </summary>
    public static class Bootstrap
    {
        /// <summary>
        /// Builds a <see cref="IContainer"/>.
        /// </summary>
        /// <returns>A <see cref="IContainer"/>.</returns>
        public static IContainer Build()
        {
            var builder = new ContainerBuilder();

            builder
                .RegisterModule<AttributedMetadataModule>();

            // Using a Telemetry Client per request, so user context, etc is unique per request.
            builder
                .RegisterType<TelemetryClient>()
                .InstancePerRequest();

            builder
                .RegisterType<MicrosoftAppCredentials>()
                .WithParameter("appId", WebConfigurationManager.AppSettings["MicrosoftAppId"])
                .WithParameter("password", WebConfigurationManager.AppSettings["MicrosoftAppPassword"]);

            builder
                .RegisterType<StateClient>();


            builder
                .RegisterType<BotState>()
                .AsImplementedInterfaces();

            builder
                .RegisterControllers(typeof(Bootstrap).Assembly)
                .Except<AuthorizeController>();

            builder
                .RegisterType<AuthorizeController>()
                .WithParameter("appSecret", WebConfigurationManager.AppSettings["AppSecret"])
                .WithParameter("authorizeUrl", new Uri(WebConfigurationManager.AppSettings["AuthorizeUrl"]));

            builder
                .RegisterApiControllers(typeof(Bootstrap).Assembly);

            builder
                .RegisterAssemblyTypes(typeof(Bootstrap).Assembly)
                .Where(t => t.GetInterfaces().Any(i => i.IsAssignableFrom(typeof(IDialog<object>))))
                .Except<AccountConnectDialog>()
                .Except<RootDialog>()
                .AsImplementedInterfaces();

            builder
                .RegisterType<AccountConnectDialog>()
                .WithParameter("appId", WebConfigurationManager.AppSettings["AppId"])
                .WithParameter("authorizeUrl", new Uri(WebConfigurationManager.AppSettings["AuthorizeUrl"]))
                .AsImplementedInterfaces();

            builder
                .RegisterType<RootDialog>()
                .AsSelf();

            return builder.Build();
        }
    }
}