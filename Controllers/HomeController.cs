using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using weddingPlanner.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace weddingPlanner.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private MyContext _context;

        public HomeController(ILogger<HomeController> logger, MyContext context)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("users/add")]
        public IActionResult AddUser(User newUser)
        {
            if(ModelState.IsValid)
            {
                if(_context.Users.Any(a => a.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use.");
                    return View("Index");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                _context.Users.Add(newUser);
                _context.SaveChanges();
                HttpContext.Session.SetInt32("UserId", newUser.UserId);
                return RedirectToAction("Dashboard");
            } else {
                return View("Index");
            }
        }

        [HttpPost("users/login")]
        public IActionResult LogUser(LoginUser loggedIn)
        {
            if(ModelState.IsValid)
            {
                User userInDB = _context.Users.FirstOrDefault(a => a.Email == loggedIn.LoginEmail);
                if(userInDB == null)
                {
                    ModelState.AddModelError("LoginEmail", "Email or Password incorrect");
                    return View("Index");
                }
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(loggedIn, userInDB.Password, loggedIn.LoginPassword);
                if(result == 0)
                {
                    ModelState.AddModelError("LoginEmail", "Email or Password incorrect");
                    return View("Index");
                }
                HttpContext.Session.SetInt32("UserId", userInDB.UserId);
                return RedirectToAction("Dashboard");
            } else {
                return View("Index");
            }
        }

        [HttpGet("Dashboard")]
        public IActionResult Dashboard()
        {
            if(HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.AllWeddings = _context.Weddings.Include(d => d.Wedder).Include(w => w.WeddingGuests).ThenInclude(u => u.Attendee).ToList();
            ViewBag.LoggedIn = _context.Users.FirstOrDefault(a => a.UserId == HttpContext.Session.GetInt32("UserId"));
            return View();
        }

        [HttpGet("user/RSVP/{wedId}")]
        public IActionResult RSVP(int wedId)
        {
            Guest AddGuest = new Guest();
            AddGuest.WeddingId = wedId;
            AddGuest.UserId = (int)HttpContext.Session.GetInt32("UserId");
            _context.Guests.Add(AddGuest);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet("user/unRSVP/{wedId}")]
        public IActionResult UnRSVP(int wedId)
        {
            Guest AddGuest = new Guest();
            AddGuest.WeddingId = wedId;
            AddGuest.UserId = (int)HttpContext.Session.GetInt32("UserId");
            _context.Guests.Remove(AddGuest);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet("delete/wedding/{wedId}")]
        public IActionResult DeleteWedding(int wedId)
        {
            Wedding RemoveWedding = _context.Weddings.FirstOrDefault(a => a.WeddingId == wedId);
            _context.Weddings.Remove(RemoveWedding);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }


        [HttpGet("wedding/add")]
        public IActionResult AddWedding()
        {
            if(HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.LoggedIn = _context.Users.FirstOrDefault(a => a.UserId == HttpContext.Session.GetInt32("UserId"));
            return View("AddWedding");
        }

        [HttpPost("weddings/new")]
        public IActionResult NewWedding(Wedding newWedding)
        {
            if(ModelState.IsValid)
            {
                _context.Weddings.Add(newWedding);
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            } else {
                return View("AddWedding");
            }
        }

        [HttpGet("wedding/{wedId}")]
        public IActionResult ViewWedding(int wedId)
        {
            if(HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index");
            }
            // Going into wedding table grabbing Wedder, also grabbing the list of guests, then including the attendees attached to that list 
            ViewBag.OneWedding = _context.Weddings.Include(z => z.Wedder).Include(w => w.WeddingGuests).ThenInclude(u => u.Attendee).FirstOrDefault(a => a.WeddingId == wedId);
            return View("ViewWedding");
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
