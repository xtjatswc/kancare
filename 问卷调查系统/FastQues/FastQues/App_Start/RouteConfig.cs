using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace FastQues
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "QuestionRecords_Home",
                url: "{controller}/{action}/{id}/{ProvinceCode}",
                defaults: new { controller = "QuestionRecords", action = "Home", id = UrlParameter.Optional, ProvinceCode = UrlParameter.Optional }
            );

            routes.MapRoute(
            name: "QuestionRecords_Index2",
            url: "{controller}/{action}/{id}/{Province}/{City}",
            defaults: new { controller = "QuestionRecords", action = "Index2", id = UrlParameter.Optional, Province= UrlParameter.Optional, City = UrlParameter.Optional }
        );

        }
    }
}
