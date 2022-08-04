using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cinema_App.Models
{
    public class Movie
    {

       
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        
        
        public string Describtion { get; set; }
        

        public int Duration { get; set; }
        
        public DateTime DateTime { get; set; }
        
        public string PhotoPath { get; set; }

        public int FullNumberOfTickets { get; set; }



    }
}
