using Final8Net.Data;
using Final8Net.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Final8Net.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly ApplicationDbContext _db;
        public AccountController(ILogger<AccountController> logger, ApplicationDbContext db)
        {
            _db = db;
            _logger = logger;
        }
        public IActionResult Login(string returnUrl)
        {
            TempData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
		{

            var student = await _db.student.FirstOrDefaultAsync(s => s.email == model.Email);
            if (student != null && VerifyPassword(model.Password, student.password))
            {
                // Authentication successful, redirect to the specified return URL or home page
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction(nameof(HomeController.Index), "Home");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password");
                return View(model);
            }
        }
		//public IActionResult Login()
		//{
		//	return View();
		//}
		//[HttpPost]
		//[ValidateAntiForgeryToken]
		//public async Task<IActionResult> Login(LoginViewModel model)
		//{
		//	if (ModelState.IsValid)
		//	{
		//		try
		//		{
		//			var student = await _db.student.FirstOrDefaultAsync(s => s.email == model.Email);

		//			if (student != null && VerifyPassword(model.Password, student.password))
		//			{
		//				// Authentication successful, redirect to home page
		//				return RedirectToAction("Index", "Home");
		//			}
		//			else
		//			{
		//				ModelState.AddModelError(string.Empty, "Invalid email or password");
		//				return View(model);
		//			}
		//		}
		//		catch (Exception ex)
		//		{
		//			_logger.LogError(ex, "An error occurred while processing the login request.");
		//			ModelState.AddModelError(string.Empty, "An error occurred while processing your request. Please try again later.");
		//			return View(model);
		//		}
		//	}
		//	else
		//	{
		//		return View(model);
		//	}
		//}
		public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _db.student.FirstOrDefaultAsync(s => s.email == model.Email);
                if (existingUser != null)
                {
                    // User already exists
                    ModelState.AddModelError(string.Empty, "An account with this email already exists.");
                    return View(model);
                }
                // Hash the password
                string hashedPassword = HashPassword(model.Password);

                // Create new student record
                var student = new Student
                {
                    email = model.Email,
                    first_name = model.FirstName,
                    last_name = model.LastName,
                    password = hashedPassword
                };
                
                _db.student.Add(student);
                await _db.SaveChangesAsync();
                
                // Redirect to a confirmation page or log the user in, depending on your flow
                return RedirectToAction("Index", "Home"); // Adjust as needed
            }
            else
            {
                // Return view with validation summary or specific error messages
                return View(model);
            }
        }
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt());
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
