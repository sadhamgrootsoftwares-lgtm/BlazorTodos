using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorTodos.Shared.DTOs
{
    public class RegisterRequestDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
