using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;
using System.Text;

namespace WebAppMvc.Models
{
    public class EmailService
    {
        public Timer timer { get; set; }
        public long interval { get; set; } = 60000; 
        
        public string sent { get; set; } 
        public string email { get; set; }
        public string subject { get; set; }
        public string message { get; set; }
        public bool send { get; set; } = false;
        public List<Person> db { get; set; }
        public int hour { get; set; }
        public int minute { get; set; }
        //public ApplicationContext db { get; set; }
        public EmailService(string path)
        {
            if (System.IO.File.Exists(path))
            {
                using (FileStream fstream = System.IO.File.OpenRead(path))
                {
                    byte[] buffer = new byte[fstream.Length];
                    fstream.Read(buffer, 0, buffer.Length);
                    string str = Encoding.Default.GetString(buffer);
                    string[] temp = str.Split("\t");
                    email = temp[0];
                    string[] hourMinute = temp[2].Split(":");
                    hour = Convert.ToInt32(hourMinute[0]);
                    minute = Convert.ToInt32(hourMinute[1]);
                    sent = temp[1];
                }
            }
        }
        public void Init()
        {
            timer = new Timer(new TimerCallback(SendEmail), null, 0, interval);
        }

        private void SendEmail(object obj)
        {
            int count = 0;


            message = "У ваших друзей сегодня день рождения!\n";
            
                foreach (var item in db.Where(p => p.Birthday.Day == DateTime.Now.Day && p.Birthday.Month == DateTime.Now.Month))
                {
                    message += item.Name + ".\n";
                    count++;
                }
            
            subject = "Не забудьте поздравить друга";

            
            DateTime dd = DateTime.Now;
                if (count>0 && dd.Hour == hour && dd.Minute == minute )
                {
                
                    SendEmailAsync();
                count = 0;

            }
                
            
        }
        public async Task SendEmailAsync()
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Поздравь друга!", "marnopolskiyi@mail.ru"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.mail.ru", 465, true);
                await client.AuthenticateAsync("marnopolskiyi@mail.ru", "xMtm9AeLwDppvHBZk2xw");
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
            Console.WriteLine("Отправлено");
        }
        public void Dispose()
        { }
        

