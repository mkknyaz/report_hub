namespace Exadel.ReportHub.Handlers;

public static class Constants
{
    public static class Validation
    {
        public static class User
        {
            public const string EmailTakenMessage = "Email is already taken.";
            public const string EmailInvalidMessage = "Email is invalid.";

            public const int PasswordMinimumLength = 8;
            public const string PasswordUppercaseMessage = "Password must have at least one uppercase letter.";
            public const string PasswordLowercaseMessage = "Password must have at least one lowercase letter.";
            public const string PasswordDigitMessage = "Password must have at least one digit.";
            public const string PasswordSpecialCharacterMessage = "Password must contain at least one special character.";
        }
    }
}
