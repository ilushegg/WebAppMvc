using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppMvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Text;


namespace WebAppMvc.Controllers
{
    
    public class HomeController : Controller
    {
        ApplicationContext db;
        EmailService emailService;
        public HomeController(ApplicationContext context)
        {
            db = context;
            emailService = new EmailService("Email");
            
        }
        public async Task<IActionResult> Index()
        {
            
            IQueryable<Person>? persons = db.Persons;
            ViewData["BirthSort"] = SortState.BirthAsc;
            int now = DateTime.Now.DayOfYear;
            IQueryable<Person>? personRes = persons.Where(p => p.Birthday.DayOfYear - now >= 0);
            personRes = personRes.OrderBy(s => s.Birthday.DayOfYear);
            
            if (System.IO.File.Exists("Email"))
            {
                using (FileStream fstream = System.IO.File.OpenRead("Email"))
                {
                    byte[] buffer = new byte[fstream.Length];
                    await fstream.ReadAsync(buffer, 0, buffer.Length);
                    string str = Encoding.Default.GetString(buffer);
                    string[] temp = str.Split("\t");
                    emailService.email = temp[0];
                    string[] hourMinute = temp[2].Split(":");
                    emailService.hour = Convert.ToInt32(hourMinute[0]);
                    emailService.minute = Convert.ToInt32(hourMinute[1]);
                    if (temp[1] == "on")
                    {
                        emailService.db = db.Persons.ToList();
                        emailService.Init();
                    }
                    emailService.sent = temp[1];
                }
            }
            
                return View(await personRes.AsNoTracking().ToListAsync());


        }
        public async Task<IActionResult> Persons(SortState sortOrder = SortState.BirthDesc)
        {
            IQueryable<Person>? persons = db.Persons;
            
            ViewData["NameSort"] = sortOrder == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            ViewData["BirthSort"] = sortOrder == SortState.BirthAsc ? SortState.BirthDesc : SortState.BirthAsc;
            

            persons = sortOrder switch
            {
                
                SortState.BirthAsc => persons.OrderBy(s => s.Birthday),
                SortState.NameAsc => persons.OrderBy(s => s.Name),
                SortState.NameDesc => persons.OrderByDescending(s => s.Name),
                
                
                _ => persons.OrderBy(s => s.Name),
            };
            emailService.db = db.Persons.ToList();
            return View(await persons.AsNoTracking().ToListAsync());

            
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(PersonViewModel pvm)
        {
            Person person = new Person { Name = pvm.Name, Birthday = pvm.Birthday };
            byte[] imageData = null;
            if (pvm.Image != null)
            {
               
                using (var binaryReader = new BinaryReader(pvm.Image.OpenReadStream()))
                {
                   imageData = binaryReader.ReadBytes((int)pvm.Image.Length);
                }
                
            }
            else
            {
                imageData = System.IO.File.ReadAllBytes("no_image.jpg");

            }
            person.Image = imageData;
            db.Persons.Add(person);
            db.SaveChanges();
            emailService.db = db.Persons.ToList();
            return RedirectToAction("Persons");


        }
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                Person person = new Person { Id = id.Value };
                db.Entry(person).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction("Persons");
            }
            emailService.db = db.Persons.ToList();
            return NotFound();
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id != null)
            {
                Person? person = await db.Persons.FirstOrDefaultAsync(p => p.Id == id);
                if (person != null) return View(person);
            }
            emailService.db = db.Persons.ToList();
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Person person, NameBirthdayModel nbm/*Person person, string Name, DateTime Birthday*/)
        {       
                var newsPost = db.Persons.Find(person.Id);
                db.Entry(newsPost).CurrentValues.SetValues(nbm);
                
    
            await db.SaveChangesAsync();
            emailService.db = db.Persons.ToList();
            return RedirectToAction("Persons"); 
        }

        [HttpPost]
        public async Task<IActionResult> EditImage(Person person, PersonViewModel pvm)
        {
            byte[] imageData = null;
            if (pvm.Image != null)
            {

                using (var binaryReader = new BinaryReader(pvm.Image.OpenReadStream()))
                {
                    imageData = binaryReader.ReadBytes((int)pvm.Image.Length);
                }

                
            }
            else
            {
                imageData = System.IO.File.ReadAllBytes("no_image.jpg");

            }
            person.Image = imageData;
            

            db.Update(person);
            db.SaveChanges();
            emailService.db = db.Persons.ToList();
            return RedirectToAction("Persons");
        }
        public async Task<IActionResult> EditImage(int? id)
        {
            if (id != null)
            {
                Person? person = await db.Persons.FirstOrDefaultAsync(p => p.Id == id);
                if (person != null) return View(person);
            }
            emailService.db = db.Persons.ToList();
            return NotFound();
        }

        public IActionResult Settings()
        {
            emailService.db = db.Persons.ToList();
            return View();
        }

      
        [HttpPost]
        public async Task<IActionResult> Settings(string email, string sendtrue, string sendfalse, string time)
        {
           
            
            emailService.email = email;
            emailService.send = false;
            
            string send = "";
            if (sendtrue == "on")
            {
                send = "on";
            }
            else if(sendfalse == "on")
            {
                send = "onue";
            }
            else if(sendtrue == null && sendfalse == null)
            {
                send = emailService.sent;
            }
            using (FileStream fstream = new FileStream("Email", FileMode.Create))
            {
                byte[] buffer = Encoding.Default.GetBytes(emailService.email+"\t"+send + "\t"+ time);
                await fstream.WriteAsync(buffer, 0, buffer.Length);
            }
            emailService.sent = send;
            emailService.db = db.Persons.ToList();
            emailService.Init();
            return RedirectToAction("Index");
        }


    }
}
