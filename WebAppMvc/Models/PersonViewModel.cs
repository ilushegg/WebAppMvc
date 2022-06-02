using Microsoft.AspNetCore.Mvc;

namespace WebAppMvc.Models
{
    public class PersonViewModel
    {
        //public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        public IFormFile Image { get; set; }
    }
}
/*
using Microsoft.AspNetCore.Http;
public class PersonViewModel
{
    public string Name { get; set; }
    public IFormFile Image { get; set; }
}
*/