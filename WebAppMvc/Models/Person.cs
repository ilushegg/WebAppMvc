
namespace WebAppMvc.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string? Name { get; set; } // имя пользователя
       // public int Age { get; set; } // возраст пользователя
        public DateTime Birthday { get; set; }
        //получаем байты:
        public byte[] Image { get; set; }
    }
}
/*
namespace WebAppMvc.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] Image { get; set; }
    }
}*/