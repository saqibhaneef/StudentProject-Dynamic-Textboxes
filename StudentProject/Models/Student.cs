﻿namespace StudentProject.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FatherName { get; set; }
        public string Class { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        public List<Address> addresses { get; set; }
        public List<Student> students { get; set; }




    }
}
