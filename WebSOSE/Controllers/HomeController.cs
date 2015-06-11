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
            if(string.IsNullOrEmpty(query))
            {
                return View("Search",null);
            }
            else
            {
                Searcher.QueryProcessor queryP = new QueryProcessor();
                var result = queryP.ProcessQuery(query);
                if (result.Count > 100)
                    result = result.GetRange(0, 100);

                return View(result);
            }
        }
    }
}
