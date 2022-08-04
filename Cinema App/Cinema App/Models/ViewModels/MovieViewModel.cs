using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cinema_App.Models.ViewModels
{
    public class MovieViewModel
    {
       
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string Describtion { get; set; }
        
        [Required]
        public int Duration { get; set; }
        [Required]
        
        public int Year { get; set; }
        [Range(1,12)]
        public int Month { get; set; }
        [Range(1,31)]
        public int Day { get; set; }
        [Range(0,23)]
        public int Hour { get; set; }
        [Range(0,59)]
        public int Minute { get; set; }

        
        public IFormFile JustPhoto { get; set; }
        [Required]
        public int NumberOfTickets { get; set; }

        
    }
}
