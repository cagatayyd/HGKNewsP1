using Microsoft.AspNetCore.Mvc;
using HGKNews.Models;
using System.Diagnostics;

namespace HGKNews.Controllers
{
    public class HomeController : Controller
    { 
        public IActionResult Index()
        {
            return View();
        }
    }
}