using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Searcher;

namespace WebSOSE.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Search(string query)
        {
            Searcher.QueryProcessor queryP = new QueryProcessor();
            var result = queryP.ProcessQuery(query);

            return View(result.OrderByDescending(pair=>pair.Value).Select(pair=>pair.Key));
        }
    }
}
