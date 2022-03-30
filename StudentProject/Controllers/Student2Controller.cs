using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StudentProject.Models;

namespace StudentProject.Controllers
{
    public class Student2Controller : Controller
    {
        private readonly AppDbContext _context;

        public Student2Controller(AppDbContext context)
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
            var model = _context.Students.ToList();
            return PartialView(model);
        }
        public IActionResult _Create()
        {
            return PartialView();
        }
        [HttpPost]
        public IActionResult Create(StudentViewModel model)
        {
            model.addresses = JsonConvert.DeserializeObject<List<Address>>(model.addresses2);
            Student student = new Student()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                FatherName = model.FatherName,
                City = model.City,
                Country= model.Country,
                Class = model.Class,
                 addresses = model.addresses,
            };
            _context.Add(student);
            _context.SaveChanges();
            return Json("ok");
            
        }
        public IActionResult _Details(int id)
        {
            var model = _context.Students.Include(x => x.addresses).Where(x => x.Id == id).FirstOrDefault();
            return PartialView(model);
        }

        public IActionResult _Edit(int id)
        {
            var model = _context.Students.Include(x => x.addresses).Where(x => x.Id == id).FirstOrDefault();
            return PartialView(model);
        }

        [HttpPost]
        public IActionResult Edit(StudentViewModel studentModel)
        {
            studentModel.addresses = JsonConvert.DeserializeObject<List<Address>>(studentModel.addresses2);
            Student model = new Student()
            {
                Id = studentModel.Id,
                FirstName = studentModel.FirstName,
                LastName = studentModel.LastName,
                FatherName = studentModel.FatherName,
                City = studentModel.City,
                Country = studentModel.Country,
                Class = studentModel.Class,
                addresses = studentModel.addresses,
            };


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

                    if (childModel.Id != 0)
                    {
                        // Update child
                        var existingChild=_context.Addresses.Find(childModel.Id);
                        _context.Entry(existingChild).CurrentValues.SetValues(childModel);
                    }
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
