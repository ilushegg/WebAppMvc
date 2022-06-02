using Microsoft.AspNetCore.Mvc;

namespace WebAppMvc.Models
{
    public class NameBirthdayModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Birthday { get; set; }

    }
}