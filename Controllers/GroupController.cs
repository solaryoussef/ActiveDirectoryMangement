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
    public class GroupController : Controller
    {

        public ActionResult GetAllGroups()
        {
            List<Group> ADGroups = GetallGroups();
            return View(ADGroups);
        }

        //if you want to get all Groups of Specific OU you have to add OU Name in Context 
        public static List<Group> GetallGroups()
        {
            List<Group> AdGroups = new List<Group>();
            var ctx = new PrincipalContext(ContextType.Domain, "MBS", "DC=SmplID,DC=com");
            GroupPrincipal _groupPrincipal = new GroupPrincipal(ctx);

            PrincipalSearcher srch = new PrincipalSearcher(_groupPrincipal);

            foreach (var found in srch.FindAll())
            {
                AdGroups.Add(new Group { GroupName = found.ToString() });

            }
            return AdGroups;
        }
        // GET: GroupController/Create
        public ActionResult Create()
        {
            Group group = new Group();
            return View(group);
        }

        // POST: GroupController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string name)
        {
            try
            {
                if (!DirectoryEntry.Exists("LDAP://CN=" + name ))
                {
                    try
                    {
                        DirectoryEntry entry = new DirectoryEntry("LDAP://");
                        DirectoryEntry group = entry.Children.Add("CN=" + name, "group");
                        group.Properties["sAmAccountName"].Value = name;
                        group.CommitChanges();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message.ToString());
                    }

                }
                else { Console.WriteLine(" already exists"); }
                return RedirectToPage("./GetAllGroups");

            }
            catch
            {
                return View();
            }
        }

        public ActionResult DeleteUser()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DeleteUser(string UserName, string GroupName)
        {
            try
            {
                DirectoryEntry dirEntry = new DirectoryEntry("LDAP://" + GroupName);
                dirEntry.Properties["member"].Remove(UserName);
                dirEntry.CommitChanges();
                dirEntry.Close();
                return RedirectToPage("./GetAllGroups");
            }

            catch (Exception)
            {
                return View();

            }
        }

        public ActionResult AssignUser()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AssignUser(string groupName, string userName)
        {
            try
                {
                    DirectoryEntry dirEntry = new DirectoryEntry("LDAP://" + groupName);
                    dirEntry.Properties["member"].Add(userName);
                    dirEntry.CommitChanges();
                    dirEntry.Close();
                return RedirectToPage("./GetAllGroups");
            }
            catch (Exception)
                {
                return View();
                }
            }
        


        public static PrincipalSearchResult<Principal> GetMembers(Group group)
        {
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

            GroupPrincipal ADgroup = GroupPrincipal.FindByIdentity(ctx, group.GroupName);

            var users = ADgroup.GetMembers();

            return users;
        }
    }
    }

