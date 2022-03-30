using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StudentProject.Models;

namespace StudentProject.Controllers
{
    public class StudentController : Controller
    {
        private readonly AppDbContext _context;

        public StudentController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var model = _context.Students.ToList();
            return View(model);
        }
        public IActionResult _StudentsList()
        {
            var model=_context.Students.ToList();
            return PartialView(model);
        }
        public IActionResult _Create()
        {            
            return PartialView();
        }
        [HttpPost]
        public IActionResult Create(string addressList, string detail)
        {
            try
            {
                var student = new Student();
                student = JsonConvert.DeserializeObject<Student>(detail);

                student.addresses = JsonConvert.DeserializeObject<List<Address>>(addressList);

                _context.Add(student);
                _context.SaveChanges();
                return Json("ok");
            }
            catch (Exception)
            {

                throw;
            }
            return Json("error");
        }
        public IActionResult _Details(int id)
        {
            var model = _context.Students.Include(x => x.addresses).Where(x => x.Id == id).FirstOrDefault();
            return PartialView(model);
        }
        public IActionResult _Edit(int id)
        {
            var model = _context.Students.Include(x => x.addresses).Where(x => x.Id ==id).FirstOrDefault();
            return PartialView(model);
        }

        [HttpPost]
        public IActionResult Edit(string addressList, string detail)
        {

            var model = new Student();
            model = JsonConvert.DeserializeObject<Student>(detail);

            model.addresses = JsonConvert.DeserializeObject<List<Address>>(addressList);

            var existingParent = _context.Students.Where(p => p.Id == model.Id).Include(p => p.addresses).SingleOrDefault();


            if (existingParent != null)
            {
                // Update parent
                _context.Entry(existingParent).CurrentValues.SetValues(model);

                // Delete children
                foreach (var existingChild in existingParent.addresses.ToList())
                {
                    if (!model.addresses.Any(c => c.Id == existingChild.Id))
                        _context.Addresses.Remove(existingChild);
                }

                // Update and Insert children
                foreach (var childModel in model.addresses)
                {
                    var existingChild = existingParent.addresses
                        .Where(c => c.Id == childModel.Id)
                        .SingleOrDefault();

                    if (existingChild != null)
                        // Update child
                        _context.Entry(existingChild).CurrentValues.SetValues(childModel);
                    else
                    {
                        // Insert child
                        var newChild = new Address
                        {
                            Name = childModel.Name,
                            //...
                        };
                        existingParent.addresses.Add(newChild);
                    }
                }

                _context.SaveChanges();
            }


            //_context.Entry(record).CurrentValues.SetValues(student);
            //foreach (var childModel in student.addresses)
            //{
            //    var existingAddress = record.addresses
            //        .Where(c => c.Id == childModel.Id)
            //        .SingleOrDefault();
            //    _context.Entry(existingAddress).CurrentValues.SetValues(childModel);

            //}
            //_context.SaveChanges();
            return Json("ok");
        }

        public IActionResult Delete(int Id)
        {
            var student = _context.Students.Include(x => x.addresses).Where(_ => _.Id == Id).FirstOrDefault();
            foreach (var item in student.addresses)
            {
                _context.Remove(item);
            }
            _context.Remove(student);
            _context.SaveChanges();
            return Json("ok");
        }

    }
}
