using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cinema_App.Models.ViewModels
{
    public class MovieSearchingViewModel
    {
        public MovieSearchingViewModel()
        {
            Movies = new List<Movie>();
        }
        public List<Movie> Movies { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }
    }
}
