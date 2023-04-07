using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NetModelsLibrary.Models;
using ServerClasses;
using System.Security.Claims;
using System.Text;
using WebServer.Hubs;
using WebServer.Models;

namespace WebServer.Controllers
{
    
    public class Test
    {
        public static IHubContext<ChatHub>? Context { get; set; } = null;
    }
    public class HomeController : Controller
    {
        public IActionResult Chat()
        {
            if (HttpContext.User.Identity?.IsAuthenticated != null && HttpContext.User.Identity.IsAuthenticated)
                return View();
            else
                return RedirectToAction("Authorization");
        }
        private readonly IHubContext<ChatHub> _hubContext;

        public HomeController(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Authorization()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Registration(UserCreationModel model)
        {
            var factory = new ClientFactory();
            factory.Listener = new RequestListener();
            factory.Handler = new RequestHandler();
            factory.Client = new WebClientObject();
            factory.Notifyer = new ClientsNotifyer();
            factory.Respondent = new WebTaskResulter();
            factory.Network = null;
            var client = factory.MakeClient();
            var res = (client as WebClientObject).GetActionResult<ResultModel, UserCreationModel>(client.Listener.OnRegistration, model);
            if (res.Success)
            {
                client.Network = new WebNetwork((IHubContext)_hubContext, client.User.Id.ToString());

                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, client.User.Id.ToString()),
                };
                ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id)).Wait();
                return View("Index");
            }
            else
            {
                return View("Authorization");
            }

        }
        [HttpPost]
        public IActionResult Authorization(AuthModel model)
        {
            var factory = new ClientFactory();
            factory.Listener = new RequestListener();
            factory.Handler = new RequestHandler();
            factory.Client = new WebClientObject();
            factory.Notifyer = new ClientsNotifyer();
            factory.Respondent = new WebTaskResulter();
            factory.Network = null;
            var client = factory.MakeClient();
            model.PasswordMD5 = CreateMD5(model.PasswordMD5);
            var res = (client as WebClientObject).GetActionResult<ResultModel, AuthModel>(client.Listener.OnAuth, model);
            if (res.Success)
            {
                client.Network = new WebNetwork((IHubContext)_hubContext, client.User.Id.ToString());

                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, client.User.Id.ToString()),
                };
                ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id)).Wait();
                return View("Index");
            }
            else
            {
                return View("Authorization");
            }
        }

        private static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}
