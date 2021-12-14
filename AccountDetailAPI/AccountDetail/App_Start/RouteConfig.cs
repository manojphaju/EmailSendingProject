﻿using Swashbuckle.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace AccountDetail
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);
            //to set swagger as default home page
            routes.MapHttpRoute(
       name: "swagger_root",
       routeTemplate: "",
       defaults: null,
       constraints: null,
       handler: new RedirectHandler((message => message.RequestUri.ToString()), "swagger"));

        }
    }
    
}
