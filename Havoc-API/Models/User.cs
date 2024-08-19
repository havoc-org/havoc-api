using Havoc_API.Exceptions;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using WarehouseApp2.Exceptions;

namespace Havoc_API.Models;

public partial class User
{
    private string _firstName;
    private string _lastName;
    private string _password;
    private string _email;


    public User(string firstName, string lastName, string email, string password)
    {
        FirstName = firstName;  
        LastName = lastName;    
        Email = email;          
        Password = password;    
    }

    public int UserId { get; private set; }

    public string FirstName
    {
        get => _firstName;
        private set
        {
            if (value.Length > 50)
                throw new StringLengthException(nameof(FirstName));
            _firstName = value;
        }
    }

    public string LastName
    {
        get => _lastName;
        private set
        {
            if (value.Length > 50)
                throw new StringLengthException(nameof(LastName));
            _lastName = value;
        }
    }

    public string Email
    {
        get => _email;
        private set
        {
            Regex EmailRegex = new Regex(
           @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (value.Length > 100)
                throw new StringLengthException(nameof(Email));
            if (!EmailRegex.IsMatch(value))
                throw new DomainException("wrong email format");
            _email = value;
        }
    }

    public string Password
    {
        get => _password;
        private set
        {
            if (value.Length > 128)
                throw new StringLengthException(nameof(Password));
            _password = value;
        }
    }

    public byte[]? Avatar { get; private set; }

    public virtual ICollection<Assignment> Assignments { get; private set; } = new List<Assignment>();

    public virtual ICollection<Attachment> Attachments { get; private set; } = new List<Attachment>();

    public virtual ICollection<Comment> Comments { get; private set; } = new List<Comment>();

    public virtual ICollection<Participation> Participations { get; private set; } = new List<Participation>();

    public virtual ICollection<Project> Projects { get; private set; } = new List<Project>();

    public virtual ICollection<Task> Tasks { get; private set; } = new List<Task>();
}
