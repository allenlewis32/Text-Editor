using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Text_Editor.Models;

namespace Text_Editor.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account/Register
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Account/Register
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // check if email is already registered
                    SqlCommand cmd = new($"select count(*) from users where email='{model.Email}'", Connection.conn);
                    int? res = (int?)cmd.ExecuteScalar();
                    if (res == null || res! == 0) // if null or zero, not registered
                    {
                        cmd.CommandText = $"addUser";
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@email", model.Email);
                        cmd.Parameters.AddWithValue("@password", model.Password);
                        cmd.Parameters.AddWithValue("@name", model.Name);
						cmd.ExecuteNonQuery();
						HttpContext.Session.SetString("uEmail", model.Email);
						HttpContext.Session.SetString("uName", model.Name);
						return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Email already registered");
                    }
                }
                catch
                {
                    ModelState.AddModelError("", "Unable to register user");
                }
            }
            return View(model);
        }

        // GET: Account/Login
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // POST: Account/Register
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // check if email is registered
                    SqlCommand cmd = new($"select name from users where email='{model.Email}'", Connection.conn);
                    using var reader = cmd.ExecuteReader();
                    if (reader.Read()) // email is registered
                    {
                        HttpContext.Session.SetString("uEmail", model.Email);
                        HttpContext.Session.SetString("uName", reader.GetString(0));
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Email not registered");
                    }
                }
                catch
                {
                    ModelState.AddModelError("", "Unable to login");
                }
            }
            return View(model);
        }

        // GET: Account/Logout
        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
