using System;
using System.Diagnostics;
using System.Web.Mvc;

namespace Estoque.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                Trace.TraceError("Home/Index error: " + ex);
                return View("Error");
            }
        }

        public ActionResult About()
        {
            try
            {
                ViewBag.Message = "Your application description page.";
                return View();
            }
            catch (Exception ex)
            {
                Trace.TraceError("Home/About error: " + ex);
                return View("Error");
            }
        }

        public ActionResult Contact()
        {
            try
            {
                ViewBag.Message = "Your contact page.";
                return View();
            }
            catch (Exception ex)
            {
                Trace.TraceError("Home/Contact error: " + ex);
                return View("Error");
            }
        }
    }
}
