namespace EventTicket.Domain;

public class UserRefreshToken
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Token { get; set; }
    public DateTime ExpiryDate { get; set; }
}