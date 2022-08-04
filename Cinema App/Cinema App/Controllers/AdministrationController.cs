using Cinema_App.Models;
using Cinema_App.Models.ViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cinema_App.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        public List<ApplicationUser> users { get; set; }
        public List<ApplicationUser> BLA { get; set; }
       

        public AdministrationController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            BLA = new List<ApplicationUser>();
            
            users = new List<ApplicationUser>();


        }

        [HttpGet]

        public IActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole(RoleViewModel role)
        {
            if (ModelState.IsValid)
            {
                IdentityRole Role = new IdentityRole()
                {
                    Name = role.RoleName
                };

                IdentityResult result = await roleManager.CreateAsync(Role);

                if (result.Succeeded)
                {
                    return RedirectToAction("RoleList", "Administration");
                }

            }


            return View();
        }
        [HttpGet]
        public IActionResult RoleList()
        {
            var roles = roleManager.Roles;
            return View(roles);
        }
        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var e = new EditRoleViewModel()
            {
                Id = id
            };
           
            var role = await roleManager.FindByIdAsync(e.Id);

            var model = new EditRoleViewModel()
            {
                Id = role.Id,
                Name = role.Name,

            };

            foreach(var user in userManager.Users)
            {
                users.Add(user);
            }

            foreach(var user in users)
            {
                if(await userManager.IsInRoleAsync(user , role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }


            return View(model);
            



        }
        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel editRoleViewModel)
        {
            

            var role = await roleManager.FindByIdAsync(editRoleViewModel.Id);

            role.Name = editRoleViewModel.Name;
            var result = await roleManager.UpdateAsync(role);
            if(result.Succeeded)
            {
                return RedirectToAction("RoleList");
            }

            

           


            return View(editRoleViewModel);



        }
        [HttpGet]
        public async Task<IActionResult> EditUserInRole(string roleId)
        {


            EditRoleViewModel editRoleViewModel = new EditRoleViewModel()
            {
                Id = roleId
            };

            var role = await roleManager.FindByIdAsync(editRoleViewModel.Id);

            
            
            var model = new List<UserRoleViewModel>();
            

            
            foreach(var user in userManager.Users)
            {
                BLA.Add(user);
            }

            foreach (var user in BLA)
            {
               
                var userRoleViewModel = new UserRoleViewModel()
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                };  

                if (await userManager.IsInRoleAsync(user, role.Name))
                    {
                        userRoleViewModel.IsSelected = true;

                    }
                    else
                    {
                        userRoleViewModel.IsSelected = false;
                    }
                
                

                model.Add(userRoleViewModel);
            }
            return View(model);
            
        }

        [HttpPost]
        public async Task<IActionResult> EditUserInRole(List<UserRoleViewModel> model ,string roleId)
        {
           
            EditRoleViewModel editRoleViewModel = new EditRoleViewModel()
            {
                Id = roleId
            };

            var role = await roleManager.FindByIdAsync(editRoleViewModel.Id);
            

            for(int i = 0; i < model.Count; i++ )
            {
                var user = await userManager.FindByIdAsync(model[i].UserId);

                IdentityResult result = null;

                if(model[i].IsSelected && !(await userManager.IsInRoleAsync(user , role.Name )))
                {
                    result = await userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!(model[i].IsSelected) && (await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }
                if(result.Succeeded)
                {
                    if (i < (model.Count - 1))
                        continue;
                    else
                        return RedirectToAction("EditRole", new { id = roleId });
                }
               
            }
            return RedirectToAction("EditRole", new { id = roleId });
        }

        

    }
    
}
