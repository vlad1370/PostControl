using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PostControl.Web.Data;
using PostControl.Web.Models;

namespace PostControl.Web.Pages.Identity.Account;

public class RegisterModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<RegisterModel> _logger;

    public RegisterModel(
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext dbContext,
        ILogger<RegisterModel> logger)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _logger = logger;
    }

    [BindProperty]
    public RegisterInputModel Input { get; set; } = new();

    public string ReturnUrl { get; set; } = string.Empty;

    public string GeneratedCardId { get; set; } = string.Empty;

    public async Task OnGetAsync(string returnUrl = "/Employee/Dashboard")
    {
        ReturnUrl = returnUrl;
        GeneratedCardId = await GenerateCardIdAsync();
        Input.CardId = GeneratedCardId;
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = "/Employee/Dashboard")
    {
        returnUrl = returnUrl ?? Url.Content("/Employee/Dashboard");

        if (ModelState.IsValid)
        {
            // Проверяем, что ID карты уникален
            var existingUser = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.CardId == Input.CardId);

            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "Этот ID карты уже используется.");
                return Page();
            }

            // Определяем время доступа по уровню
            var timeWindows = GetTimeWindowsByLevel(Input.AccessLevel);

            // Создаём пользователя (используем CardId как UserName)
            var user = new ApplicationUser
            {
                UserName = Input.CardId,
                Email = Input.Email,
                FullName = Input.FullName,
                CardId = Input.CardId,
                AccessLevel = Input.AccessLevel,
                TimeWindows = timeWindows,
                IsBlocked = false,
                FailedAttempts = 0
            };

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User created: {CardId}", Input.CardId);

                // Добавляем роль Employee
                await _userManager.AddToRoleAsync(user, "Employee");

                // Автоматически входим
                await _userManager.UpdateAsync(user);

                return LocalRedirect(returnUrl);
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        // Если ошибка - генерируем новый ID
        GeneratedCardId = await GenerateCardIdAsync();
        return Page();
    }

    private string GetTimeWindowsByLevel(string level)
    {
        return level switch
        {
            "employee" => "09:00-18:00",
            "manager" => "07:00-20:00",
            "vip" => "00:00-23:59",
            "visitor" => "09:00-17:00",
            _ => "09:00-18:00"
        };
    }

    private async Task<string> GenerateCardIdAsync()
    {
        var random = new Random();
        string cardId;
        bool exists;

        do
        {
            // Генерируем ID формата EMPXXXX (Employee)
            cardId = $"EMP{random.Next(1000, 9999)}";
            exists = await _dbContext.Users.AnyAsync(u => u.CardId == cardId);
        } while (exists);

        return cardId;
    }
}