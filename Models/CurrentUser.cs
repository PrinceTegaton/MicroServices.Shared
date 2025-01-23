namespace MicroServices.Shared.Models
{
    public class CurrentUser
    {
        public long UserId { get; set; }
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string MobileNo { get; set; }
        public UserRoleName Role { get; set; }
    }
}