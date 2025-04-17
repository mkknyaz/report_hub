namespace Exadel.ReportHub.Handlers;

public static class Constants
{
    public static class Validation
    {
        public static class User
        {
            public const int FullNameMaxLength = 100;
            public const string EmailTakenMessage = "Email is already taken.";
            public const string EmailInvalidMessage = "Email is invalid.";

            public const int PasswordMinimumLength = 8;
            public const string PasswordUppercaseMessage = "Password must have at least one uppercase letter.";
            public const string PasswordLowercaseMessage = "Password must have at least one lowercase letter.";
            public const string PasswordDigitMessage = "Password must have at least one digit.";
            public const string PasswordSpecialCharacterMessage = "Password must contain at least one special character.";
        }

        public static class Customer
        {
            public const int CountryMaxLength = 56;
            public const int NameMaxLength = 100;
            public const string CountryShouldStartWithCapitalMessage = "Country must begin with a capital letter.";
            public const string NameShouldStartWithCapitalMessage = "Name must begin with a capital letter.";
            public const string EmailTakenMessage = "Email is already taken.";
            public const string EmailInvalidMessage = "Email is invalid.";
        }

        public static class Client
        {
            public const string ShouldStartWithCapitalMessage = "The name must begin with a capital letter.";
            public const string NameTakenMessage = "Name is already taken";
            public const int ClientMaximumNameLength = 20;
        }

        public static class Invoice
        {
            public const int CurrencyCodeLength = 3;
            public const int InvoiceMaximumNumberLength = 15;
            public const string InvoiceNumberErrorMessage = "Invoice number must start with 'INV' followed by digits.";
            public const string IssueDateErrorMessage = "Issue date cannot be in the future.";
            public const string DueDateErrorMessage = "Due date must be greater than issue date.";
            public const string TimeComponentErrorMassage = "Date cannot have a time component.";
            public const int BankAccountNumberMinLength = 8;
            public const int BankAccountNumberMaxLength = 28;
            public const string BankAccountNumberErrorMessage = "Bank account number must only contain digits and dashes.";
            public const string CustomerDoesntExistsErrorMessage = "Customer does not exist.";
            public const string ClientDoesntExistsErrorMessage = "Client does not exist.";
        }

        public static class UserAssignment
        {
            public const string UserDoesNotExistMessage = "User does not exist.";
            public const string ClientDoesNotExistMessage = "Client does not exist.";
        }

        public static class Import
        {
            public const string FileExtentionError = "The file must be in CSV format (.csv extension).";
            public const string UploadedFileLengthError = "Uploaded file must not be empty.";
        }
    }
}
