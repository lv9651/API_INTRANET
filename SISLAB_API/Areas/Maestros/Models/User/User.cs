namespace SISLAB_API.Areas.Maestros.Models
{
    public class User
    {
        public int id { get; set; }
        public string dni   { get; set; }
        public string username { get; set; }

        public string password { get; set; }
        public string role { get; set; }
        public string NOMBRE { get; set; }




    }

    public class UserPasswordUpdate
    {
        public string Dni { get; set; }
        public string NewPassword { get; set; }
    }

}
