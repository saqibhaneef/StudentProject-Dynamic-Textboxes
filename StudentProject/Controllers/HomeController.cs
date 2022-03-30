using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StudentProject.Models;
using System.Diagnostics;

namespace StudentProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _Context;

        public HomeController(ILogger<HomeController> logger,AppDbContext context)
        {
            _logger = logger;
            _Context = context;
        }

        public IActionResult Index()
        {
            var list=_Context.Students.ToList();
            return View(list);
        }
        public IActionResult Detail(int Id)
        {
            var model = _Context.Students.Include(x=>x.addresses).Where(x=>x.Id==Id).FirstOrDefault();
            return View(model);
        }
        [HttpPost]
        public IActionResult Add(string addressList,string detail)
        {
            try
            {
                var student = new Student();
                student = JsonConvert.DeserializeObject<Student>(detail);

                student.addresses = JsonConvert.DeserializeObject<List<Address>>(addressList);

                _Context.Add(student);
                _Context.SaveChanges();
                return Json("ok");
            }
            catch (Exception)
            {

                throw;
            }
            return Json("error");
        }
        [HttpGet]
        public IActionResult Edit(int Id)
        {
            //var model= _Context.Students.Where(x => x.Id == Id).FirstOrDefault();            
            var model = _Context.Students.Include(x => x.addresses).Where(x => x.Id == Id).FirstOrDefault();           
            return View(model);
        }
        [HttpPost]
        public IActionResult Edit(string addressList, string detail)
        {
            
                var student = new Student();
                student = JsonConvert.DeserializeObject<Student>(detail);

                student.addresses = JsonConvert.DeserializeObject<List<Address>>(addressList);

                var record = _Context.Students.Where(p => p.Id == student.Id).Include(p => p.addresses).SingleOrDefault();
                _Context.Entry(record).CurrentValues.SetValues(student);
                
                foreach (var childModel in student.addresses)
                {
                    var existingAddress = record.addresses
                        .Where(c => c.Id == childModel.Id)
                        .SingleOrDefault();
                        _Context.Entry(existingAddress).CurrentValues.SetValues(childModel);
                    
                }
                _Context.SaveChanges();
                return Json("ok");                        
        }
        
        public IActionResult Delete(int Id)
        {
            var student=_Context.Students.Include(x=>x.addresses).Where(_ => _.Id == Id).FirstOrDefault();
            foreach (var item in student.addresses)
            {
                _Context.Remove(item);
            }
            _Context.Remove(student);
            _Context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}