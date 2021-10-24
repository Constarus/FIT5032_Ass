using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FIT5032_Ass.Models;

namespace FIT5032_Ass.Controllers
{
    public class BookingsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        //The global temp booking
        public static Booking temp = new Booking();

        // GET: Bookings
        public ActionResult Index()
        {
            var bookings = db.Bookings.Include(b => b.Room);
            return View(bookings.ToList());
        }

        // GET: Bookings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);

        }

        //Rate
        //Only user and admin can create, edit and delete the booking.
        [Authorize(Roles = "User,Admin")]
        public ActionResult Rate(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            //make the golbal temp here
            temp = booking;
            if (booking == null)
            {
                return HttpNotFound();
            }
            //make the selection list from one star to five stars
            List<SelectListItem> selection = new List<SelectListItem>();
            selection.Add(new SelectListItem() { Text = "★☆☆☆☆", Value = "1", Selected = false });
            selection.Add(new SelectListItem() { Text = "★★☆☆☆", Value = "2", Selected = false });
            selection.Add(new SelectListItem() { Text = "★★★☆☆", Value = "3", Selected = false });
            selection.Add(new SelectListItem() { Text = "★★★★☆", Value = "4", Selected = false });
            selection.Add(new SelectListItem() { Text = "★★★★★", Value = "5", Selected = true });

            ViewBag.Select = selection;

            return View(booking);
            
        }

        //Rating method
        [HttpPost]
        public ActionResult Rate(FormCollection form)
        {
            //get current rate
            string rate = form["Select"];
            int rateScore = 0;
            int.TryParse(rate, out rateScore);
            double dou = double.Parse(temp.Score);
            //caculate the generated score
            double finalScore = (dou * temp.Score_num + rateScore) / (temp.Score_num + 1);
            //add the score number by 1
            temp.Score = String.Format("{0:F}", finalScore);
            temp.Score_num++;


            if (ModelState.IsValid)
            {
                db.Entry(temp).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }


            return View(temp);
        }

        // GET: Bookings/Create
        [Authorize(Roles = "User,Admin")]
        public ActionResult Create()
        {
            ViewBag.RoomId = new SelectList(db.Rooms, "Id", "Name");
            return View();
        }

        // POST: Bookings/Create
        
        [ValidateInput(true)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Description,Date,Register,RoomId,Score,Score_num")] Booking booking)
        {

            int result = 0;
            foreach(Booking book in db.Bookings.ToList())
            {
                if(book.Date == booking.Date && book.RoomId==booking.RoomId)
                {
                    result=1;
                }
            }
            if (result == 0)
            {
                if (ModelState.IsValid)
                {
                    db.Bookings.Add(booking);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.RoomId = new SelectList(db.Rooms, "Id", "Name", booking.RoomId);
                return View(booking);

            }
            else
            {
                return RedirectToAction("Warning");
            }
           

        }

        // GET: Bookings/Edit/5
        [Authorize(Roles = "User,Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            ViewBag.RoomId = new SelectList(db.Rooms, "Id", "Name", booking.RoomId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        
        //XSS attack
        [ValidateInput(true)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,Date,Register,RoomId,Score,Score_num")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                db.Entry(booking).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RoomId = new SelectList(db.Rooms, "Id", "Name", booking.RoomId);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        [Authorize(Roles = "User,Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.Bookings.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Booking booking = db.Bookings.Find(id);
            db.Bookings.Remove(booking);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult Warning()
        {
            return View();
        }
        public ActionResult Chart()
        {
            return View();
        }
    }
}
