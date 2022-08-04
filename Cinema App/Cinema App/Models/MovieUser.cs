using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cinema_App.Models
{
    public class MovieUser
    {
       
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public int MovieId { get; set; }
        public int Tickets { get; set; }
    }
}
