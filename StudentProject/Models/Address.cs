namespace StudentProject.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int StudentId { get; set; }
        public Student student { get; set; }
    }
}
