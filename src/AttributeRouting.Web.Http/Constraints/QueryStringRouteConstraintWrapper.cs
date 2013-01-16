﻿using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Routing;
using AttributeRouting.Constraints;

namespace AttributeRouting.Web.Http.Constraints
{
    public class QueryStringRouteConstraintWrapper : IQueryStringRouteConstraintWrapper, IHttpRouteConstraint
    {
        private readonly IHttpRouteConstraint _constraint;

        public QueryStringRouteConstraintWrapper(IHttpRouteConstraint constraint)
        {
            _constraint = constraint;
        }

        public object Constraint
        {
            get { return _constraint; }
        }

        public bool Match(HttpRequestMessage request, IHttpRoute route, string parameterName, IDictionary<string, object> values, HttpRouteDirection routeDirection)
        {
            // If the query param does not exist, then fail.
            var queryString = HttpUtility.ParseQueryString(request.RequestUri.Query);
            if (!queryString.AllKeys.Contains(parameterName))
                return false;

            // Process the constraint.
            var queryRouteValues = new Dictionary<string, object>
            {
                { parameterName, queryString[parameterName] }
            };

            return _constraint == null // ie: Simply ensure that the query param exists.
                   || _constraint.Match(request, route, parameterName, queryRouteValues, routeDirection);
        }
    }
}