using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Threading.Tasks;
using trialofAD.Models;

namespace trialofAD.Controllers
{
    public class UserController : Controller
    {
        // GET: UserController
       
        public ActionResult GetAllUsers()
        {
            List<User> ADUsers = GetallAdUsers();
            return View(ADUsers);
        }

        public static List<User> GetallAdUsers()
        {
            List<User> AdUsers = new List<User>();
            //SmplID.com My Domain Controller which i created 

            var ctx = new PrincipalContext(ContextType.Domain, "SmplID", "DC=SmplID,DC=com");
            UserPrincipal userPrin = new UserPrincipal(ctx);
            userPrin.Name = "*";
            var searcher = new PrincipalSearcher();
            searcher.QueryFilter = userPrin;
            var results = searcher.FindAll();
            foreach (Principal p in results)
            {
                AdUsers.Add(new User
                {
                    DisplayName = p.DisplayName,
                    Samaccountname = p.SamAccountName
                });
            }
            return AdUsers;
        }

        // GET: UserController/Details/5
        public ActionResult Details(int UserId)
        {
            return View();
        }

        // GET: UserController/Create
        public ActionResult Create(int UserId)
        {
            User user = new User();

            return View(user);
        }

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User user)
        {
            try
            {
                using (var pc = new PrincipalContext(ContextType.Domain))
                {
                    using (var up = new UserPrincipal(pc))
                    {
                        up.Name = user.DisplayName;
                        up.SamAccountName = user.Samaccountname;
                        up.Save();
                    }
                }
            return RedirectToPage("./Details",new {UserId = user.Id } );
            }
            catch
            {
                return View();
            }
        }

        // GET: UserController/Edit/5
        public ActionResult Edit(int? UserId)
        {
            User user = new User();
            return View();
        }

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int UserId, User user)
        {
            try
            {
                using (PrincipalContext AD = new PrincipalContext(ContextType.Domain))
                {
                    UserPrincipal u = new UserPrincipal(AD);
                    u.DisplayName = user.DisplayName;

                    PrincipalSearcher search = new PrincipalSearcher(u);
                    UserPrincipal result = (UserPrincipal)search.FindOne();
                    search.Dispose();
                   
                    result.DisplayName = user.DisplayName;
                    result.SamAccountName = user.Samaccountname;
                    result.Save();
                }
                return RedirectToPage("./Details", new { UserId = user.Id });
            }
            catch
            {

                return View();
            }
        }
    }
}
