using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Exam.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace Exam.Controllers
{
    public class HomeController : Controller
    {
        private Context _context;
        // here we can "inject" our context service into the constructor
        public HomeController(Context context)
        {
            _context = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            if(HttpContext.Session.GetInt32("userId") != null)
            {
                HttpContext.Session.Clear();
            }
            return View();
        }

        [HttpPost("register")]
        public IActionResult Register(User user)
        {
            if(ModelState.IsValid)
            {
                // If a User exists with provided email
                if(_context.Users.Any(u => u.Email == user.Email))
                {
                    // Manually add a ModelState error to the Email field, with provided
                    // error message
                    ModelState.AddModelError("Email", "Email already in use!");
                    return View("");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.Password = Hasher.HashPassword(user, user.Password);
                _context.Add(user);
                _context.SaveChanges();
                return Redirect("/");
            }
            else
            {
                return View("Index");
            }
        }
        [HttpGet("login")]
        public IActionResult LogIn()
        {
            if(HttpContext.Session.GetInt32("userId") != null)
            {
                HttpContext.Session.Clear();
            }
            return View("LogIn");
        }

        [HttpPost("signIn")]
        public IActionResult SignIn(LoginUser userSubmission)
        {
            if(ModelState.IsValid)
            {
                // If inital ModelState is valid, query for a user with provided email
                var userInDb = _context.Users.FirstOrDefault(u => u.Email == userSubmission.Email);
                // If no user exists with provided email
                if(userInDb == null)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("LogIn");
                }
                
                var hasher = new PasswordHasher<LoginUser>();

                // verify provided password against hash stored in db
                var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password);
                
                // result can be compared to 0 for failure
                if(result == 0)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("");
                }
                HttpContext.Session.SetInt32("userId", userInDb.UserId);
                return Redirect("home");
            }
            else
            {
                return View("login");
            }
        }

        [HttpGet("home")]
        public IActionResult Home()
        {
            if(HttpContext.Session.GetInt32("userId") is null)
            {
                return Redirect("/");
            }
            User sessionUser = _context.Users
                .Include(u => u.EventsAttending)
                .ThenInclude(rsvp => rsvp.EventOfRSVP)
                .FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("userId").Value);
            List<Event> EventsAttending = new List<Event>();
            foreach(var rsvp in sessionUser.EventsAttending)
            {
                EventsAttending.Add(rsvp.EventOfRSVP);
            }
            IOrderedEnumerable<Event> AllEvents = _context.Events
                .Include(e => e.Participants)
                .Include(e => e.Creator)
                .ToList()
                .OrderByDescending(e => e.StartDate);
            ViewBag.sessionUser = sessionUser;
            ViewBag.EventsAttending = EventsAttending;
            ViewBag.AllEvents = AllEvents;
            return View("Home");
        }
        [HttpGet("event/{eventId}")]
        public IActionResult Event(int eventId)
        {
            if(HttpContext.Session.GetInt32("userId") is null)
            {
                return Redirect("/");
            }
            User sessionUser = _context.Users
                .Include(u => u.EventsAttending)
                .ThenInclude(rsvp => rsvp.EventOfRSVP)
                .FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("userId").Value);
            Event RequestedEvent = _context.Events
                .Include(e => e.Participants)
                .ThenInclude(rsvp => rsvp.Attendee)
                .Include(e => e.Creator)
                .FirstOrDefault(e => e.EventId == eventId);
            List<Event> EventsAttending = new List<Event>();
            foreach(var rsvp in sessionUser.EventsAttending)
            {
                EventsAttending.Add(rsvp.EventOfRSVP);
            }
            List<User> EventParticipants = new List<User>();
            foreach(var rsvp in RequestedEvent.Participants)
            {
                EventParticipants.Add(rsvp.Attendee);
            }
            ViewBag.sessionUser = sessionUser;
            ViewBag.RequestedEvent = RequestedEvent;
            ViewBag.EventsAttending = EventsAttending;
            ViewBag.EventParticipants = EventParticipants;
            return View("Event");
        }
        [HttpGet("new")]
        public IActionResult New()
        {
            if(HttpContext.Session.GetInt32("userId") is null)
            {
                return Redirect("/");
            }
            User sessionUser = _context.Users
                .Include(u => u.EventsAttending)
                .ThenInclude(rsvp => rsvp.EventOfRSVP)
                .FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("userId").Value);
            ViewBag.sessionUser = sessionUser;
            return View("New");
        }
        [HttpPost("create")]
        public IActionResult CreateEvent(Event e)
        {
            if(HttpContext.Session.GetInt32("userId") is null)
            {
                return Redirect("/");
            }
            if(ModelState.IsValid)
            {
                _context.Add(e);
                _context.SaveChanges();
                return Redirect("/home");
            }
            else
            {
                User sessionUser = _context.Users
                    .Include(u => u.EventsAttending)
                    .ThenInclude(rsvp => rsvp.EventOfRSVP)
                    .FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("userId").Value);
                ViewBag.sessionUser = sessionUser;
                return View("New");
            }
        }
        [HttpPost("join")]
        public IActionResult Join(RSVP rsvp)
        {
            if(HttpContext.Session.GetInt32("userId") is null)
            {
                return Redirect("/");
            }
            _context.Add(rsvp);
            _context.SaveChanges();
            return Redirect("/home");
        }
        [HttpPost("leave")]
        public IActionResult Leave(RSVP FormRSVP)
        {
            if(HttpContext.Session.GetInt32("userId") is null)
            {
                return Redirect("/");
            }
            RSVP RetrievedRSVP = _context.RSVPs.SingleOrDefault(rsvp => rsvp.EventId == FormRSVP.EventId && rsvp.UserId == FormRSVP.UserId);
            _context.RSVPs.Remove(RetrievedRSVP);
            _context.SaveChanges();
            return Redirect("/home");
        }
        [HttpPost("delete")]
        public IActionResult Delete(RSVP FormRSVP)
        {
            if(HttpContext.Session.GetInt32("userId") is null)
            {
                return Redirect("/");
            }
            Event RetrievedEvent = _context.Events.SingleOrDefault(e => e.EventId == FormRSVP.EventId);
            _context.Events.Remove(RetrievedEvent);
            _context.SaveChanges();
            return Redirect("/home");
        }
    }
}