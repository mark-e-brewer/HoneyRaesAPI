namespace HoneyRaesAPI.Models;
public class ServiceTicket
{
    public string Id { get; set; }

    public int CustomerId { get; set; }

    public int EmployeeId { get; set; }

    public string Description { get; set; }

    public bool Emergency { get; set; }

    public int DateComplete { get; set; }
}