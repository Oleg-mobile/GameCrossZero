﻿using Microsoft.AspNetCore.Mvc;

namespace GameApp.Mvc.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
