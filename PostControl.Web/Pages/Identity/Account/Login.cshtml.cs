using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PostControl.Web.Models;
using System.ComponentModel.DataAnnotations;

namespace PostControl.Web.Pages.Identity.Account;

public class LoginModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public LoginModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [BindProperty]
    public LoginInputModel Input { get; set; } = new();

    public string ReturnUrl { get; set; } = string.Empty;

    public async Task OnGetAsync(string returnUrl = "/Employee/Dashboard")
    {
        ReturnUrl = returnUrl ?? "/Employee/Dashboard";
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = "/Employee/Dashboard")
    {
        returnUrl = returnUrl ?? "/Employee/Dashboard";

        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByNameAsync(Input.UserName);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Неверный ID карты или пароль");
                return Page();
            }

            var result = await _signInManager.PasswordSignInAsync(
                user, Input.Password, false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // Перенаправление по роли
                if (await _userManager.IsInRoleAsync(user, "Guard"))
                {
                    return RedirectToPage("/Guard/Dashboard");
                }
                return RedirectToPage("/Employee/Dashboard");
            }

            ModelState.AddModelError(string.Empty, "Неверный ID карты или пароль");
        }

        return Page();
    }

    public class LoginInputModel
    {
        [Required(ErrorMessage = "Введите ID карты")]
        [Display(Name = "ID карты")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = string.Empty;
    }
}