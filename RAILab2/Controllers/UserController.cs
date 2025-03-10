using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using RAILab2.Models;

namespace RAILab2.Controllers
{
    public class UserController : Controller
    {
        public static List<User> users = [];

        public static void Init()
        {
            var admin = new User("admin");
            var user1 = new User("ala");
            var user2 = new User("ola");
            var user3 = new User("kasia");
            var user4 = new User("basia");
            var user5 = new User("asia");

            user1.Friends.AddRange([user2, user3, user4]);
            user2.Friends.AddRange([user3, user4, user5]);
            user3.Friends.AddRange([user1, user2]);
            user4.Friends.AddRange([user2, user3]);
            user5.Friends.Add(user3);
            users.AddRange([admin, user1, user2, user3, user4, user5]);
        }

        public ActionResult Login ()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(IFormCollection collection)
        {
            var login = collection["login"];
            var user = users.Find(u => u.Login == login);

            if (user != null)
            {
                HttpContext.Session.SetString("UserLogin",login);
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("Login", "Użytkownik o danym loginie nie istnieje");
            return View();
        }

        public ActionResult Logout()
        {
            HttpContext.Session.Remove("UserLogin");
            return RedirectToAction("Index", "Home");
        }

        public ActionResult List()
        {
            return View(users);
        }

        // GET: UserController/Delete/5
        public ActionResult Delete(string login)
        {
            users.Remove(users.Find(u => u.Login == login));
            return RedirectToAction("List");
        }

        // GET: UserController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            var login = collection["login"];
            var user = users.Find(u => u.Login == login);

            if (user == null)
            {
                users.Add(new User(login));
                return RedirectToAction("List");
            }
            ModelState.AddModelError("Login", "Użytkownik o danym loginie już istnieje");
            return View();
        }
    }
}
