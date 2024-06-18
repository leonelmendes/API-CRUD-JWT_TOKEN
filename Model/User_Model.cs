namespace Api.Model
{
    public class User_Model
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public int Phone { get; set; }
        public string? Password { get; set; }
        public string? Token_Login { get; set; }
    }
}