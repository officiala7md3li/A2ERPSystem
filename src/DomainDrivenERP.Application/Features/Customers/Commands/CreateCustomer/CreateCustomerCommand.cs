using DomainDrivenERP.Application.Abstractions.Messaging;
using DomainDrivenERP.Domain.Entities.Customers;

namespace DomainDrivenERP.Application.Features.Customers.Commands.CreateCustomer;

public class CreateCustomerCommand : ICommand<Customer>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public CreateCustomerCommand()
    {
    }

    public CreateCustomerCommand(string firstName, string lastName, string phone, string email)
    {
        FirstName = firstName;
        LastName = lastName;
        Phone = phone;
        Email = email;
    }
}
