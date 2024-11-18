using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Havoc_API.Exceptions;

namespace Havoc_API.Models;

public partial class Tag
{
    private string _name = null!;
    private string _colorHex = null!;

    public int TagId { get; private set; }

    public string Name 
    {
        get => _name;
        private set
        {
            string trimmedValue = value.Trim();

            if (trimmedValue.Length > 20 || trimmedValue.Length == 0)
                throw new StringLengthException(nameof(Name));

            _name = trimmedValue;
        } 
    }

    public string ColorHex
    {
        get => _colorHex;
        private set
        {
            var hexColorRegex = new Regex(@"^#[0-9A-Fa-f]{6}$");   
            if(!hexColorRegex.IsMatch(value))
                throw new MismatchedRegexException(nameof(ColorHex));
            
            _colorHex = value; 
        }
    }

    public virtual ICollection<Task> Tasks { get; private set; } = new List<Task>();

    private Tag() {  }

    public Tag(string name, string colorHex)
    {
        Name = name;
        ColorHex = colorHex;
    }
}
