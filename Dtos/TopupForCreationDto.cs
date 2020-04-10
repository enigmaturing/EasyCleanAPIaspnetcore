using EasyClean.API.Models;
using System;

namespace EasyClean.API.Dtos
{
    public class TopupForCreationDto
    {
        public double Amount { get; set; }
        public int EmployeeId { get; set; }
        public int UserId { get; set; }
    }
}
