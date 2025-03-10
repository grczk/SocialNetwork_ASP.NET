using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static RAILab2.Controllers.UserController;

namespace RAILab2.Controllers
{
    public class FriendsController : Controller
    {
        // GET: FriendsController
        public ActionResult Index()
        {
            var currentUser = users.Find(u => u.Login == HttpContext.Session.GetString("UserLogin"));
            if (currentUser == null)
            {
                return RedirectToAction("Login", "User"); 
            }
            return View(currentUser.Friends);
        }
        [HttpPost]
        [Route("Friends/Add/{login}")]
        public JsonResult Add(string login)
        {
            var currentUser = users.Find(u => u.Login == HttpContext.Session.GetString("UserLogin"));
            if (currentUser == null)
            {
                return Json(false);
            }
            var friendToAdd = users.Find(u => u.Login == login);
            if (friendToAdd==null || currentUser.Friends.Contains(friendToAdd) || currentUser == friendToAdd)
            {
                return Json(false);
            }
            currentUser.Friends.Add(friendToAdd);
            return Json(true);
        }

        [HttpPost]
        [Route("Friends/Del/{login}")]
        public JsonResult Del(string login)
        {
            var currentUser = users.Find(u => u.Login == HttpContext.Session.GetString("UserLogin"));
            if (currentUser == null)
            {
                return Json(false);
            }

            var friendToRemove = currentUser.Friends.Find(f => f.Login == login);
            if (friendToRemove == null)
            {
                return Json(false);
            }

            currentUser.Friends.Remove(friendToRemove);
            return Json(true);
        }

        [Route("Friends/List")]
        public JsonResult List()
        {
            var currentUser = users.Find(u => u.Login == HttpContext.Session.GetString("UserLogin"));
            if (currentUser == null)
            {
                return Json(new List<string>());
            }
            var friendsList = currentUser.Friends.Select(f => f.Login).ToList();

            return Json(friendsList);
        }

        [HttpGet]
        [Route("Friends/Export")]
        public IActionResult Export()
        {
            var currentUser = users.Find(u => u.Login == HttpContext.Session.GetString("UserLogin"));
            if (currentUser == null)
            {
                return Unauthorized();
            }

            var friendsData = string.Join(Environment.NewLine, currentUser.Friends.Select(f => f.Login));
            var fileName = "friends_export.txt";
            var fileBytes = System.Text.Encoding.UTF8.GetBytes(friendsData);
            return File(fileBytes, "text/plain", fileName);
        }

        [HttpPost]
        [Route("Friends/Import")]
        public IActionResult Import(IFormFile file)
        {
            var currentUser = users.Find(u => u.Login == HttpContext.Session.GetString("UserLogin"));
            if (currentUser == null)
            {
                return Unauthorized();
            }

            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Nie przesłano żadnego pliku.";
                return RedirectToAction("List", "Friends");
            }

            try
            {
                using (var streamReader = new StreamReader(file.OpenReadStream()))
                {
                    var content = streamReader.ReadToEnd();
                    var newFriendsLogins = content.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

                    currentUser.Friends.Clear();
                    foreach (var login in newFriendsLogins)
                    {
                        var friend = users.Find(u => u.Login == login);
                        if (friend != null && friend != currentUser)
                        {
                            currentUser.Friends.Add(friend);
                        }
                    }
                }

                return RedirectToAction("List", "Friends");
            }
            catch
            {
                TempData["Error"] = "Wystąpił błąd podczas przetwarzania pliku.";
                return RedirectToAction("List", "Friends");
            }
        }
    }
}
