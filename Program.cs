
using HoneyRaesAPI.Models;
List<Customer> customers = new List<Customer>
{
    new Customer { Id = 1, Name = "Mark", Address = "520 lyndenbury Drive" },
    new Customer { Id = 2, Name = "Alexis", Address = "4908 Rollingwood Drive" },
    new Customer { Id = 3, Name = "Kilo", Address = "1337 Stinky lane" }
};
List<Employee> employees = new List<Employee> 
{
    new Employee { Id = 1, Name = "John", Specialty = "he fast" },
    new Employee { Id = 2, Name = "Bobbi", Specialty = "she strong" }
};
List<ServiceTicket> serviceTickets = new List<ServiceTicket> 
{
    new ServiceTicket { Id = 1, CustomerId = 1, EmployeeId = 1, Description = "Help Mark go fast", Emergency = false },
    new ServiceTicket { Id = 2, CustomerId = 2, EmployeeId = 2, Description = "help Alexis get strong", Emergency = true, DateComplete = "04/17/1998"},
    new ServiceTicket { Id = 3, CustomerId = 1, EmployeeId = 2, Description = "help Mark get strong", Emergency = false, DateComplete = "02/11/1998"},
    new ServiceTicket { Id = 3, CustomerId = 1, EmployeeId = 2, Description = "help Mark get strong", Emergency = false, DateComplete = "02/11/1998"},
    new ServiceTicket { Id = 4, CustomerId = 2, EmployeeId = 1, Description = "help Alexis go fast", Emergency = false, DateComplete = "02/19/1998"},
    new ServiceTicket { Id = 5, CustomerId = 3, Description = "help Kilo not be stinky", Emergency = true, DateComplete = "09/28/1999"}
};

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//ServiceTickets GETs

app.MapGet("/servicetickets", () =>
{
    return serviceTickets;
});

app.MapGet("/servicetickets/{id}", (int id) =>
{
    ServiceTicket serviceTicket = serviceTickets.FirstOrDefault(s => s.Id == id);
    if (serviceTicket == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(serviceTicket);
});

//emplyee GETs
//emplyee GETs

app.MapGet("/employees/{id}", (int id) =>
{
    Employee employee = employees.FirstOrDefault(e => e.Id == id);
    if (employee == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(employee);
});

app.MapGet("/employee/{id}", (int id) =>
{
    return employees.FirstOrDefault(e => e.Id == id);
});

//Customer GETs

app.MapGet("/customer", () =>
{
    return customers;
});

app.MapGet("/customer/{id}", (int id) =>
{
    Customer customer = customers.FirstOrDefault(c => c.Id == id);
    if (customer == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(customer);
});

app.Run();
