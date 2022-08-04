using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cinema_App.Models.ViewModels
{
    public class UserRoleViewModel
    {
        public UserRoleViewModel()
        {
            IsSelected = false;
        }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public bool IsSelected { get; set; }

    }
}
