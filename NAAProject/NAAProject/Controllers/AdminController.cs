﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NAAProject.Data;
using NAAProject.Data.Models.Domain;

namespace NAAProject.Controllers
{
    public class AdminController : Controller
    {

        ApplicationDbContext context;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AdminController(SignInManager<IdentityUser> signInManager)
        {
            context = new ApplicationDbContext();
            _signInManager = signInManager;
        }

        [Authorize(Roles = "Admin")]
        public ActionResult AddRole()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult AddRole(IFormCollection collection)
        {
            IdentityRole role = new IdentityRole();
            role.Name = collection["RoleName"].ToString();
            role.NormalizedName = collection["RoleName"].ToString().ToUpper();
            context.Roles.Add(role);
            context.SaveChanges();
            return RedirectToAction("Admin", "Home");
        }



        [Authorize(Roles = "Admin")]
        public ActionResult GetRolesForUser()
        {
            FillInDropDowns();
            return View();
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GetRolesForUser(string userName)
        {
            IdentityUser user = await _signInManager.UserManager.FindByNameAsync(userName);
            ViewData["UserName"] = user.UserName;
            IEnumerable<string> userRoles = await _signInManager.UserManager.GetRolesAsync(user);
            return View("GetRolesForUserResult", userRoles);

        }

        [Authorize(Roles = "Admin")]
        public ActionResult AddUserToRole()
        {
            FillInDropDowns();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddUserToRole(string username, string rolename)
        {
            IdentityUser user = _signInManager.UserManager.FindByEmailAsync(username).Result;
            await _signInManager.UserManager.AddToRoleAsync(user, rolename);
            FillInDropDowns();
            return RedirectToAction("AddUserToRole", "Admin");
        }

        void FillInDropDowns() {
            var userList = context.Users.OrderBy(
                u => u.UserName).ToList().Select
                (
                    uu => new SelectListItem
                    {
                        Value = uu.UserName.ToString(),
                        Text = uu.UserName
                    }
                ).ToList();

            ViewData["Users"] = userList;

            var roleList = context.Roles.OrderBy(
                r => r.Name).ToList().Select(
                    rr => new SelectListItem
                    {
                        Value = rr.Name.ToString(),
                        Text = rr.Name
                    }
                ).ToList();

            ViewData["Roles"] = roleList;
        }

    }
}
