using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.Routing;

namespace projectNew
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "Friends",
                routeTemplate: "api/{namespace}/{controller}/{id}/{modifier}/{secondModifier}",
                defaults: new { id = RouteParameter.Optional, 
                                modifier = RouteParameter.Optional,
                                secondModifier = RouteParameter.Optional}

            );
            
            config.Services.Replace(typeof(IHttpControllerSelector),
                new NamespaceVersioning(config));
        }
    }
}
