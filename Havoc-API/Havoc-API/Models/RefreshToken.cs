namespace Havoc_API.Models
{
    public class RefreshToken
    {
        public string Token { get; set; } = null!;  // Сам токен
        public DateTime Expires { get; set; }      // Дата истечения
        public DateTime Created { get; set; }      // Дата создания
        public bool IsExpired => DateTime.UtcNow >= Expires;  // Логическое свойство: истёк ли токен
        public bool IsActive => !IsExpired;        // Логическое свойство: активен ли токен
    }

}
