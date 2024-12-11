using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using userproj.Data;
using userproj.Models;
using userproj.Models.ViewModels;

namespace userproj.Controllers
{
    public class UserController : Controller
    {   
        //Объект шифрования
        private readonly IDataProtector dataProtector;
        UserContext context = new UserContext();
        public UserController(IDataProtectionProvider protectionProvider){
            //уникальный ключ для защиты
            dataProtector = protectionProvider.CreateProtector("AuthCookieProtector");
        }

        //метод шифрования
        private string ProtectCookieValue(string data){
            return dataProtector.Protect(data);
        }

        //метод расшифровки 
        private string UnprotectCookieValue(string protectedData){
            return dataProtector.Unprotect(protectedData);
        }

        public ActionResult Index()
        {
            return View();
        }

        public IActionResult Register(){
            return View();
        }

        [HttpPost]
        public IActionResult Register(string username, string password, string role = "customer"){
            if(context.Users.Where(x => x.UserName == username).ToList().Count() != 0){
                return Content("Пользователь с таким именем уже существует!");
            }

            User user = new User(){
                UserName = username,
                Password = BCrypt.Net.BCrypt.HashPassword(password),
                Role = role
            };

            //UserRepository.AddUser(user);
            context.Users.Add(user);
            context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Login(){
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password){
            User user = context.Users.FirstOrDefault(x => x.UserName == username);
            
            if(user == null){
                return View();
            }
            
            if(!BCrypt.Net.BCrypt.Verify(password, user.Password)){
                return Content("Введён неправильный пароль!");
            }

            string cookieValue = user.UserName;
            string protectedCookieValue = ProtectCookieValue(cookieValue);

            HttpContext.Response.Cookies.Append("auth", protectedCookieValue);

            return RedirectToAction("Index");
        }

        public IActionResult Download(){
            // если не удаётся найти куки auth
            if(!HttpContext.Request.Cookies.TryGetValue("auth", out var protectedCookieValue)){
                return RedirectToAction("Index");
            }

            try{
                var cookieValue = UnprotectCookieValue(protectedCookieValue);
                return View();
            }catch{
                return RedirectToAction("Login");
            }
        }

        public IActionResult Settings(){
            // если не удаётся найти куки auth

            SettingsViewModel viewModel = new SettingsViewModel();
            if(!HttpContext.Request.Cookies.TryGetValue("auth", out var protectedCookieValue)){
                viewModel.IsAuthentificated = false;
                return RedirectToAction("Index");
            }


            try{
                var cookieValue = UnprotectCookieValue(protectedCookieValue);
                
                viewModel.IsAuthentificated = true;
                viewModel.Name = cookieValue;
            }catch{
                viewModel.IsAuthentificated = false;
            }

            return View(viewModel);
        }
    }
}
