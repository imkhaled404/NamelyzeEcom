namespace NamelyzeEcom.Common.Constants;

public static class AppConstants
{
    public const string DefaultConnection = "DefaultConnection";
    public const string JwtSecretKey = "Jwt:SecretKey";
    public const string JwtIssuer = "Jwt:Issuer";
    public const string JwtAudience = "Jwt:Audience";
    public const string JwtExpiration = "Jwt:ExpirationMinutes";

    public static class RoleNames
    {
        public const string SuperAdmin = "SuperAdmin";
        public const string Admin = "Admin";
        public const string Manager = "Manager";
        public const string Customer = "Customer";
    }

    public static class NotificationEvents
    {
        public const string Registration = "Registration";
        public const string OrderConfirmation = "OrderConfirmation";
        public const string Shipping = "Shipping";
        public const string Delivery = "Delivery";
    }
}
