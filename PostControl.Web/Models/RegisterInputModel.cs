using System.ComponentModel.DataAnnotations;

namespace PostControl.Web.Models;

public class RegisterInputModel
{
    public string CardId { get; set; }

    [Required(ErrorMessage = "Введите ФИО")]
    [Display(Name = "ФИО")]
    public string FullName { get; set; } = string.Empty;

    // ID карты генерируется автоматически

    [Required(ErrorMessage = "Введите Email")]
    [EmailAddress(ErrorMessage = "Некорректный Email")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Выберите уровень доступа")]
    [Display(Name = "Уровень доступа")]
    public string AccessLevel { get; set; } = "employee";

    [Required(ErrorMessage = "Введите пароль")]
    [StringLength(100, MinimumLength = 4, ErrorMessage = "Пароль должен быть от 4 до 100 символов")]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Подтверждение пароля")]
    [Compare("Password", ErrorMessage = "Пароли не совпадают")]
    public string ConfirmPassword { get; set; } = string.Empty;
}