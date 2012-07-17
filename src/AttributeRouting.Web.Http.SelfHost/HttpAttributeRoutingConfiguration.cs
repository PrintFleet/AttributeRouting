﻿using System;
using System.Net.Http;
using System.Threading;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;
using AttributeRouting.Framework.Factories;
using AttributeRouting.Web.Http.SelfHost.Framework.Factories;

namespace AttributeRouting.Web.Http.SelfHost
{
    public class HttpAttributeRoutingConfiguration : AttributeRoutingConfigurationBase
    {
        private readonly IAttributeRouteFactory _attributeFactory;
        private readonly IRouteConstraintFactory _routeConstraintFactory;
        private readonly IParameterFactory _parameterFactory;

        public HttpAttributeRoutingConfiguration()
        {
            _attributeFactory = new HttpAttributeRouteFactory(this);
            _routeConstraintFactory = new HttpRouteConstraintFactory(this);
            _parameterFactory = new HttpRouteParameterFactory();

            CurrentUICultureResolver = (ctx, data) => Thread.CurrentThread.CurrentUICulture.Name;

            RegisterDefaultInlineRouteConstraints<IHttpRouteConstraint>(typeof(RegexRouteConstraintAttribute).Assembly);
        }

        public override Type FrameworkControllerType
        {
            get { return typeof(IHttpController); }
        }

        /// <summary>
        /// Attribute factory
        /// </summary>
        public override IAttributeRouteFactory AttributeFactory
        {
            get { return _attributeFactory; }
        }

        /// <summary>
        /// Constraint factory
        /// </summary>
        public override IRouteConstraintFactory RouteConstraintFactory
        {
            get { return _routeConstraintFactory; }
        }

        /// <summary>
        /// Parameter factory
        /// </summary>
        public override IParameterFactory ParameterFactory
        {
            get { return _parameterFactory; }
        }

        /// <summary>
        /// this delegate returns the current UI culture name.
        /// This value is used when constraining inbound routes by culture.
        /// The default delegate returns the CurrentUICulture name of the current thread.
        /// </summary>
        public Func<HttpRequestMessage, IHttpRouteData, string> CurrentUICultureResolver { get; set; }

        /// <summary>
        /// Scans the assembly of the specified controller for routes to register.
        /// </summary>
        /// <typeparam name="T">The type of the controller used to specify the assembly</typeparam>
        public void ScanAssemblyOf<T>() where T : IHttpController
        {
            ScanAssembly(typeof(T).Assembly);
        }

        /// <summary>
        /// Adds all the routes for the specified controller type to the end of the route collection.
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        public void AddRoutesFromController<T>() where T : IHttpController
        {
            AddRoutesFromController(typeof(T));
        }

        /// <summary>
        /// Adds all the routes for all the controllers that derive from the specified controller
        /// to the end of the route collection.
        /// </summary>
        /// <typeparam name="T">The base controller type</typeparam>
        public void AddRoutesFromControllersOfType<T>() where T : IHttpController
        {
            AddRoutesFromControllersOfType(typeof(T));
        }

        /// <summary>
        /// Automatically applies the specified constaint against url parameters
        /// with names that match the given regular expression.
        /// </summary>
        /// <param name="keyRegex">The regex used to match url parameter names</param>
        /// <param name="constraint">The constraint to apply to matched parameters</param>
        public void AddDefaultRouteConstraint(string keyRegex, IHttpRouteConstraint constraint)
        {
            base.AddDefaultRouteConstraint(keyRegex, constraint);
        }
    }
}