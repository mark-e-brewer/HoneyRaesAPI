
using HoneyRaesAPI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
List<Customer> customers = new List<Customer>
{
    new Customer { Id = 1, Name = "Mark", Address = "520 lyndenbury Drive" },
    new Customer { Id = 2, Name = "Alexis ", Address = "4908 Rollingwood Drive" },
    new Customer { Id = 3, Name = "Kilo Bear", Address = "1337 Stinky lane" }
};

List<Employee> employees = new List<Employee> 
{
    new Employee { Id = 1, Name = "John", Specialty = "he fast" },
    new Employee { Id = 2, Name = "Bobbi", Specialty = "she strong" }
};

List<ServiceTicket> serviceTickets = new List<ServiceTicket> 
{
    new ServiceTicket { Id = 1, CustomerId = 1, EmployeeId = 1, Description = "Help Mark go fast", Emergency = false },
    new ServiceTicket { Id = 2, CustomerId = 2, EmployeeId = 2, Description = "help Alexis get strong", Emergency = true, DateComplete = DateTime.Today.AddYears(-2)},
    new ServiceTicket { Id = 3, CustomerId = 1, EmployeeId = 2, Description = "help Mark get strong", Emergency = false, DateComplete = DateTime.Today.AddYears(-2)},
    new ServiceTicket { Id = 4, CustomerId = 2, EmployeeId = 1, Description = "help Alexis go fast", Emergency = false, DateComplete = DateTime.Today},
    new ServiceTicket { Id = 5, CustomerId = 3, Description = "help Kilo not be stinky", Emergency = true, DateComplete = DateTime.Today}
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

app.MapGet("/servicetickets", () =>
{
    return serviceTickets;
});

app.MapGet("/servicetickets/{id}", (int id) =>
{
    ServiceTicket serviceTicket = serviceTickets.FirstOrDefault(st => st.Id == id);
    if (serviceTicket == null)
    {
        return Results.NotFound();
    }
    serviceTicket.Employee = employees.FirstOrDefault(e => e.Id == serviceTicket.EmployeeId);
    serviceTicket.Customer = customers.FirstOrDefault(c => c.Id == serviceTicket.CustomerId);
    return Results.Ok(serviceTicket);
});

app.MapGet("/employees/{id}", (int id) =>
{
    Employee employee = employees.FirstOrDefault(e => e.Id == id);
    if (employee == null)
    {
        return Results.NotFound();
    }
    employee.ServiceTickets = serviceTickets.Where(st => st.EmployeeId == id).ToList();
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
    customer.ServiceTickets = serviceTickets.Where(st => st.CustomerId == id).ToList();
    return Results.Ok(customer);
});

app.MapPost("/servicetickets", (ServiceTicket serviceTicket) =>
{
    // creates a new id (When we get to it later, our SQL database will do this for us like JSON Server did!)
    serviceTicket.Id = serviceTickets.Max(st => st.Id) + 1;
    serviceTickets.Add(serviceTicket);
    return serviceTicket;
});

app.MapPost("/servicetickets/{id}/complete", (int id) =>
{
    ServiceTicket ticketToComplete = serviceTickets.FirstOrDefault(st => st.Id == id);
    if (ticketToComplete != null)
    {
        ticketToComplete.DateComplete = DateTime.Today;
    }
});

app.MapPut("/servicetickets/{id}", (int id, ServiceTicket serviceTicket) =>
{
    ServiceTicket ticketToUpdate = serviceTickets.FirstOrDefault(st => st.Id == id);
    int ticketIndex = serviceTickets.IndexOf(ticketToUpdate);
    if (ticketToUpdate == null)
    {
        return Results.NotFound();
    }
    //the id in the request route doesn't match the id from the ticket in the request body. That's a bad request!
    if (id != serviceTicket.Id)
    {
        return Results.BadRequest();
    }
    serviceTickets[ticketIndex] = serviceTicket;
    return Results.Ok();
});

app.MapDelete("/serviceticketdelete/{id}", (int id) =>
 {
    ServiceTicket ticketToDelete = serviceTickets.FirstOrDefault(st => st.Id == id);
    if (ticketToDelete != null)
    {
        serviceTickets.Remove(ticketToDelete);
    }
 });

 app.MapGet("/emergencytickets", () => // #1 EMERGENCY TICKETS
{
    IEnumerable<ServiceTicket> emergencyTickets = serviceTickets.Where(st => st.Emergency == true);
    if (!emergencyTickets.Any())
    {
        return Results.NotFound();
    }
    return Results.Ok(emergencyTickets);
});

 app.MapGet("/unassignedtickets", () => //#2 UNASSIGNED TICKETS
{
    IEnumerable<ServiceTicket> unassignedTickets = serviceTickets.Where(st => st.EmployeeId == null);
    if (!unassignedTickets.Any())
    {
        return Results.NotFound();
    }
    return Results.Ok(unassignedTickets);
});

app.MapGet("/inactivecustomers", () => // #3 INACTIVE CUSTOMERS
{
    DateTime oneYearAgo = DateTime.Now.AddYears(-1);

    var inactiveCustomers = serviceTickets
        .GroupBy(st => st.CustomerId) 
        .Select(group =>
        {
            DateTime? mostRecentCompletionDate = group.Max(st => st.DateComplete);
            return new
            {
                CustomerId = group.Key,
                MostRecentCompletionDate = mostRecentCompletionDate
            };
        })
        .Where(customerData => customerData.MostRecentCompletionDate < oneYearAgo)
        .Select(customerData => customerData.CustomerId)
        .ToList();
    return Results.Json(inactiveCustomers);
});

app.MapGet("/inactiveemployees", () =>
{
    var employeesWithIncompleteTickets = serviceTickets
        .Where(st => !st.DateComplete.HasValue)
        .Select(st => st.EmployeeId)
        .Distinct()
        .ToList();

    var inactiveEmployees = employees
        .Where(emp => !employeesWithIncompleteTickets.Contains(emp.Id))
        .ToList();

    return Results.Json(inactiveEmployees);
});



app.Run();
