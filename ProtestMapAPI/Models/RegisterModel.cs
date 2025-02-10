namespace ProtestMapAPI.Models
{
    public class RegisterModel
    {
        public required string Email { get; set; }
        public required string FullName { get; set; }
        public required string Password { get; set; }
    }
}
