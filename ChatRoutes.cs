using Orchard.Mvc.Routes;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace JabbR
{
    public class ChatRoutes : IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }
        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            return new[] {
                    new RouteDescriptor {
                        Priority = 10,
                        Route = new Route(
                            "Chat",
                            new RouteValueDictionary {
                                {"area", "JabbR"},
                                {"controller", "Chat"},
                                {"action", "Index"}
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary {
                                {"area", "JabbR"}
                            },
                            new MvcRouteHandler())
                    },
                    new RouteDescriptor {
                        Priority = 10,
                        Route = new Route(
                            "OperatorsAvailable",
                            new RouteValueDictionary {
                                {"area", "JabbR"},
                                {"controller", "Operator"},
                                {"action", "GetOnlineOperators"}
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary {
                                {"area", "JabbR"}
                            },
                            new MvcRouteHandler())
                    },
                    new RouteDescriptor {
                        Priority = 10,
                        Route = new Route(
                            "Operator",
                            new RouteValueDictionary {
                                {"area", "JabbR"},
                                {"controller", "Chat"},
                                {"action", "Operator"}
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary {
                                {"area", "JabbR"}
                            },
                            new MvcRouteHandler())
                    },
                    new RouteDescriptor {
                        Priority = 10,
                        Route = new Route(
                            "Monitor",
                            new RouteValueDictionary {
                                {"area", "JabbR"},
                                {"controller", "Chat"},
                                {"action", "Monitor"}
                            },
                            new RouteValueDictionary(),
                            new RouteValueDictionary {
                                {"area", "JabbR"}
                            },
                            new MvcRouteHandler())
                    }

            };
        }
    }
}
