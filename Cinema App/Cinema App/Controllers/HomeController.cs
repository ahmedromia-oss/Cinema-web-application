using Cinema_App.Models;
using Cinema_App.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Cinema_App.Controllers
{
    public class HomeController : Controller
    {
        public int MovieIdForUser { get; set; }
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        private readonly DatabaseContext context;
        private readonly IWebHostEnvironment environment;
        public List<Movie> Movies { get; set; }

        public List<Movie> MovieForUser { get; set; }

        public HomeController(UserManager<ApplicationUser> userManager
               , SignInManager<ApplicationUser> signInManager, DatabaseContext context, IWebHostEnvironment environment)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.context = context;
            this.environment = environment;
            Movies = new List<Movie>();
            MovieForUser = new List<Movie>();
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult AddingMovie(int? id)
        {
            
            

            var result = context.Movies.Find(id);
            
        
            
               
            

            MovieViewModel movieViewModel = new MovieViewModel()
            {
                Name = result.Name,

                Describtion = result.Describtion,

                Duration = result.Duration,

                Hour = result.DateTime.Hour,
                Year = result.DateTime.Year,
                Month = result.DateTime.Month,

                Day = result.DateTime.Day,

                Minute = result.DateTime.Day,

                NumberOfTickets = result.FullNumberOfTickets



        };

            ViewBag.id = id;

            if(result == null)
            {
               
                
                return View();
            }
            else
            {
                return View(movieViewModel);
            }
           
            
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult AddingMovie(MovieViewModel model , int id)
        {

            ViewBag.id = id;

            var result = context.Movies.Find(id);

            
            if (ModelState.IsValid)
            {
               
                
                string UniqueFileName = null;
                if (model.JustPhoto != null)
                {

                    string UploadsFolder = Path.Combine(environment.WebRootPath, "lib/Images");
                    UniqueFileName = Guid.NewGuid().ToString() + "_" + model.JustPhoto.FileName;
                    string filepath = Path.Combine(UploadsFolder, UniqueFileName);
                    model.JustPhoto.CopyTo(new FileStream(filepath, FileMode.Create));
                }

                

                Movie movie = new Movie
                {
                    Name = model.Name,
                    Describtion = model.Describtion,
                    DateTime = new DateTime(model.Year, model.Month, model.Day, model.Hour, model.Minute, 0), 
                    Duration = model.Duration,
                    PhotoPath = UniqueFileName,
                    FullNumberOfTickets = model.NumberOfTickets
                    

                };

                if(result == null)
                {
                    context.Movies.Add(movie);
                    context.SaveChanges();
                    return RedirectToAction("index", "Home");
                }

                else
                {
                    result.Name = model.Name;
                    result.Describtion = model.Describtion;
                    result.DateTime = new DateTime(model.Year, model.Month, model.Day, model.Hour, model.Minute, 0);
                    result.Duration = model.Duration;

                    result.FullNumberOfTickets = model.NumberOfTickets;

                    

                    context.Movies.Update(result);
                    context.SaveChanges();
                    return RedirectToAction("MovieDetails", "Home" , new { id = id});
                }
            
            }
            
           
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "Home");
        }


        [HttpPost]
        public IActionResult Index(MovieSearchingViewModel model)
        {
            if (model.SearchTerm != null)
            {
                model.Movies = context.Movies.Where(e => e.Name.ToUpper().Contains(model.SearchTerm.ToUpper())&& DateTime.Now.Day == e.DateTime.Day && DateTime.Now.Month == e.DateTime.Month && DateTime.Now.Day == e.DateTime.Day).ToList();
            }
            else
            {
                var result2 = context.Movies.OrderBy(e => e.DateTime);
                var result = result2.Where(e => DateTime.Now.Day == e.DateTime.Day && DateTime.Now.Month == e.DateTime.Month && DateTime.Now.Day == e.DateTime.Day).ToList();


                model.Movies = result.ToList();


            }
            return View(model);
        }

        [HttpGet]

        public IActionResult Index()
        {
            MovieSearchingViewModel model = new MovieSearchingViewModel();

            foreach (var movie in context.Movies)
            {
                Movies.Add(movie);
            }
            foreach(var movie in Movies)
            {
                if(DateTime.Now.Year >= movie.DateTime.Year && DateTime.Now.Month >= movie.DateTime.Month && DateTime.Now.Day >= movie.DateTime.Day && DateTime.Now.Hour >= movie.DateTime.Hour && DateTime.Now.Minute >= movie.DateTime.Minute && DateTime.Now.Second >= movie.DateTime.Second)
                {
                    context.Movies.Remove(movie);

                    var DeleteBooking = context.MovieUser.Where(e => e.MovieId == movie.Id);

                    var deleteBooking = DeleteBooking.ToList();

                    foreach(var usermovie in deleteBooking)
                    {
                        context.MovieUser.Remove(usermovie);
                        context.SaveChanges();
                    }
                    context.SaveChanges();

                }
            }
            
            var result2 = context.Movies.OrderBy(e => e.DateTime);
            model.Movies = result2.Where(e => DateTime.Now.Day == e.DateTime.Day && DateTime.Now.Month == e.DateTime.Month && DateTime.Now.Day == e.DateTime.Day).ToList();
            context.SaveChanges();
            

            return View(model);
        }


        [HttpGet]
        public IActionResult Rigester()
        {

            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }


        [HttpPost]

        public async Task<IActionResult> Rigester(User model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email };
                var result = await userManager.CreateAsync(user, model.Password);



                if (result.Succeeded)
                {


                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }
        [HttpGet]

        public IActionResult Login()
        {
            if(signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Email, model.password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("index", "Home");
                    }
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt");

            }
            return View(model);
        }
        [Authorize]
        
        public IActionResult MovieDetails(int id)
        {
            

            Movie movie = context.Movies.Find(id);

            

            return View(movie);
           






        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> MovieAddUser(int id)
        {
           

            ApplicationUser user1 = await GetCurrentUserAsync();
            MovieUser user = context.MovieUser.FirstOrDefault(a => a.MovieId == id && a.UserId == user1.Id);

            ViewBag.id = id;
          
;


            if(user != null)
            {
                
                return View(user);
            }

            else
            {
                return View();
            }


            

            
            
            
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> MovieAddUser(int id , MovieUser movieUser)
        {

            ViewBag.Error = " ";

            Movie movie = context.Movies.FirstOrDefault(o => o.Id == id);

            ApplicationUser user1 = await GetCurrentUserAsync();

            

            MovieUser user = new MovieUser()
            {
                MovieId = movie.Id,
                UserId = user1.Id,
                Tickets = movieUser.Tickets
            };
            
            var result =  context.MovieUser.FirstOrDefault(o => o.MovieId == id && o.UserId == user1.Id );
            Movie movie1 = context.Movies.Find(id);
            ViewBag.id = id;
            if (result == null)
            {
                if (user.Tickets > 0 && user.Tickets <= movie1.FullNumberOfTickets)
                {
                    
                    movie1.FullNumberOfTickets -= user.Tickets;
                    context.MovieUser.Add(user);
                    context.Movies.Update(movie1);
                    context.SaveChanges();
                    
                }
                else
                {
                    ViewBag.Error = "Not Allowed Number of Tickets";
                    return View();
                }
            }
            else
            {
                
                movie1.FullNumberOfTickets += result.Tickets;
                context.MovieUser.Remove(result);
                context.Movies.Update(movie1);
                context.SaveChanges();
                
            }


            return RedirectToAction("MovieDetails", "Home" , new { id = id });

           

            



        }
       
        
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult DeleteMovie(int id)
        {
            
           Movie movie = context.Movies.Find(id);

            var result = context.Movies.Remove(movie);
            var DeleteBooking = context.MovieUser.Where(e => e.MovieId == movie.Id);

            var deleteBooking = DeleteBooking.ToList();

            foreach (var usermovie in deleteBooking)
            {
                context.MovieUser.Remove(usermovie);
                context.SaveChanges();
            }

            context.SaveChanges();


            return RedirectToAction("Index", "Home");

            
        }
        [HttpGet]
       public IActionResult ThisMonth()
        {
            MovieSearchingViewModel model = new MovieSearchingViewModel();
            foreach (var movie in context.Movies)
            {
                Movies.Add(movie);
            }
            foreach (var movie in Movies)
            {
                if (DateTime.Now.Year >= movie.DateTime.Year && DateTime.Now.Month >= movie.DateTime.Month && DateTime.Now.Day >= movie.DateTime.Day && DateTime.Now.Hour >= movie.DateTime.Hour && DateTime.Now.Minute >= movie.DateTime.Minute && DateTime.Now.Second >= movie.DateTime.Second)
                {
                    context.Movies.Remove(movie);

                    var DeleteBooking = context.MovieUser.Where(e => e.MovieId == movie.Id);

                    var deleteBooking = DeleteBooking.ToList();

                    foreach (var usermovie in deleteBooking)
                    {
                        context.MovieUser.Remove(usermovie);
                        context.SaveChanges();
                    }
                    context.SaveChanges();

                }
            }

            var result2 = context.Movies.OrderBy(e =>e.DateTime);
            var result= result2.Where(e => e.DateTime.Month == DateTime.Now.Month && DateTime.Now.Year == e.DateTime.Year);
           
            context.SaveChanges();
            model.Movies = result.ToList();

            return View(model);
        }


        [HttpPost]
        public IActionResult ThisMonth(MovieSearchingViewModel model)
        {
            if (model.SearchTerm != null)
            {
                model.Movies = context.Movies.Where(e => e.Name.ToUpper().Contains(model.SearchTerm.ToUpper()) && e.DateTime.Month == DateTime.Now.Month && DateTime.Now.Year == e.DateTime.Year).ToList();
            }
            else
            {
                var result2 = context.Movies.OrderBy(e => e.DateTime);
                var result = result2.Where(e => e.DateTime.Month == DateTime.Now.Month && DateTime.Now.Year == e.DateTime.Year);


                model.Movies = result.ToList();

                
            }
            return View(model);
        }


        [HttpPost]
        public IActionResult ThisYear(MovieSearchingViewModel model)
        {
            if (model.SearchTerm != null)
            {
                if (DateTime.Now.Month != 12)
                {
                    model.Movies = context.Movies.Where(e => e.Name.ToUpper().Contains(model.SearchTerm.ToUpper()) && DateTime.Now.Year == e.DateTime.Year).ToList();
                }
                else
                {
                    model.Movies = context.Movies.Where(e => e.Name.ToUpper().Contains(model.SearchTerm.ToUpper()) && (DateTime.Now.Year == e.DateTime.Year || e.DateTime.Year == (DateTime.Now.Year) + 1)).ToList();
                }
            }
            else
            {
                var result2 = context.Movies.OrderBy(e => e.DateTime);

                var result = result2.Where(e => DateTime.Now.Year == e.DateTime.Year);

                if (DateTime.Now.Month == 12)
                {
                    result = result2.Where(e => DateTime.Now.Year == e.DateTime.Year || e.DateTime.Year == (DateTime.Now.Year) + 1);
                }
                else
                {
                    result = result2.Where(e => DateTime.Now.Year == e.DateTime.Year);
                }

                model.Movies = result.ToList();

                
            }
            return View(model);
        }


        [HttpGet]
        public IActionResult ThisYear()
        {
            MovieSearchingViewModel model = new MovieSearchingViewModel();
            foreach (var movie in context.Movies)
            {
                Movies.Add(movie);
            }
            foreach (var movie in Movies)
            {
                if (DateTime.Now.Year >= movie.DateTime.Year && DateTime.Now.Month >= movie.DateTime.Month && DateTime.Now.Day >= movie.DateTime.Day && DateTime.Now.Hour >= movie.DateTime.Hour && DateTime.Now.Minute >= movie.DateTime.Minute && DateTime.Now.Second >= movie.DateTime.Second)
                {
                    context.Movies.Remove(movie);

                    var DeleteBooking = context.MovieUser.Where(e => e.MovieId == movie.Id);

                    var deleteBooking = DeleteBooking.ToList();

                    foreach (var usermovie in deleteBooking)
                    {
                        context.MovieUser.Remove(usermovie);
                        context.SaveChanges();
                    }

                    
                }
            }

            var result2 = context.Movies.OrderBy(e => e.DateTime);

            var result = result2.Where(e => DateTime.Now.Year == e.DateTime.Year);

            if (DateTime.Now.Month == 12)
            {
               result = result2.Where(e => DateTime.Now.Year == e.DateTime.Year || e.DateTime.Year == (DateTime.Now.Year)+1);
            }
            else
            {
                result = result2.Where(e => DateTime.Now.Year == e.DateTime.Year);
            }
               
     

            context.SaveChanges();

            
            model.Movies = result.ToList();

            return View(model);
            
        }
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            foreach (var movie in context.Movies)
            {
                Movies.Add(movie);
            }
            foreach (var movie in Movies)
            {
                if (DateTime.Now.Year >= movie.DateTime.Year && DateTime.Now.Month >= movie.DateTime.Month && DateTime.Now.Day >= movie.DateTime.Day && DateTime.Now.Hour >= movie.DateTime.Hour && DateTime.Now.Minute >= movie.DateTime.Minute && DateTime.Now.Second >= movie.DateTime.Second)
                {
                    context.Movies.Remove(movie);

                    var DeleteBooking = context.MovieUser.Where(e => e.MovieId == movie.Id);

                    var deleteBooking = DeleteBooking.ToList();

                    foreach (var usermovie in deleteBooking)
                    {
                        context.MovieUser.Remove(usermovie);
                        context.SaveChanges();
                    }


                }
            }

            ApplicationUser user1 = await GetCurrentUserAsync();

            var result1 = context.MovieUser.Where(e => e.UserId == user1.Id);
            List<int> ovies = new List<int>();

            foreach(var result in result1)
            {
                ovies.Add(result.MovieId);
            }

            foreach(var Movie in ovies)
            {
                var movie = context.Movies.Find(Movie);

                if(movie != null)
                {
                    MovieForUser.Add(movie);
                }




            }

            
            


            return View(MovieForUser);
        }


        private Task<ApplicationUser> GetCurrentUserAsync() => userManager.GetUserAsync(HttpContext.User);
        






    }


    
    





        /*
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        */
    
}
