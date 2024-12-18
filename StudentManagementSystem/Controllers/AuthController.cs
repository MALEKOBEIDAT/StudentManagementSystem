using System;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using BCrypt.Net;
using StudentManagementSystem.Models;

public class AuthController : Controller
{
    private readonly StudentManagementDBEntities _context = new StudentManagementDBEntities();

    // GET: Register Page
    [HttpGet]
    public ActionResult Register()
    {
        return View();
    }

    // POST: Register User
    [HttpPost]
    public JsonResult Register(User user)
    {
        try
        {
            if (ModelState.IsValid)
            {
                // Hash the password before storing it in the database
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

                using (var context = _context)
                {
                    user.PasswordHash = hashedPassword;
                    context.Users.Add(user);
                    context.SaveChanges();
                }
             
                // Call the stored procedure to register the user
                //_context.Database.ExecuteSqlCommand(
                //    "EXEC SP_RegisterUser @Username, @PasswordHash, @Email, @FullName",
                //    new SqlParameter("@Username", user.Username),
                //    new SqlParameter("@PasswordHash", hashedPassword),
                //    new SqlParameter("@Email", user.Email),
                //    new SqlParameter("@FullName", user.FullName)
                //);

                return Json(new { success = true, message = "User registered successfully!" });
            }
            else
            {
                return Json(new { success = false, message = "Invalid input data." });
            }
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Error: " + ex.Message });
        }
    }

    // GET: Login Page
    [HttpGet]
    public ActionResult Login()
    {
        return View();
    }

    // **POST: Login User**
    [HttpPost]
    public JsonResult Login(User user)
    {
        try
        {
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.PasswordHash))
            {
                return Json(new { success = false, message = "Username and Password are required." });
            }

            // Call the stored procedure to verify user login
            var result = _context.Users
                .Where(u => u.Username == user.Username)
                .Select(u => new { u.PasswordHash, u.Username })
                .FirstOrDefault();

            if (result == null)
            {
                return Json(new { success = false, message = "Invalid username or password." });
            }

            // Check if the password matches
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(user.PasswordHash, result.PasswordHash);

            if (!isPasswordValid)
            {
                return Json(new { success = false, message = "Invalid username or password." });
            }

            // If successful, create a session or authentication cookie
            Session["Username"] = result.Username;

            return Json(new { success = true, message = "Login successful!" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Error: " + ex.Message });
        }
    }
}
