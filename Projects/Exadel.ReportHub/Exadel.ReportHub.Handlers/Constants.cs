namespace Exadel.ReportHub.Handlers;

public static class Constants
{
    public static class Validation
    {
        public static class RuleSet
        {
            public const string Names = nameof(Names);
            public const string Passwords = nameof(Passwords);
            public const string Countries = nameof(Countries);
        }

        public static class Name
        {
            public const int MaxLength = 100;
            public const string ShouldStartWithCapitalMessage = "Name must begin with a capital letter.";
        }

        public static class Password
        {
            public const int MinimumLength = 8;
            public const string UppercaseMessage = "Password must have at least one uppercase letter.";
            public const string LowercaseMessage = "Password must have at least one lowercase letter.";
            public const string DigitMessage = "Password must have at least one digit.";
            public const string SpecialCharacterMessage = "Password must contain at least one special character.";
        }

        public static class Country
        {
            public const int MaxLength = 56;
            public const string ShouldStartWithCapitalMessage = "Country must begin with a capital letter.";
        }

        public static class User
        {
            public const string EmailTakenMessage = "Email is already taken.";
            public const string EmailInvalidMessage = "Email is invalid.";
        }

        public static class Customer
        {
            public const string EmailTakenMessage = "Email is already taken.";
            public const string EmailInvalidMessage = "Email is invalid.";
            public const string CountryDoesNotExistMessage = "Country does not exist.";
        }

        public static class Client
        {
            public const string NameTakenMessage = "Name is already taken";
        }

        public static class Invoice
        {
            public const int InvoiceMaximumNumberLength = 15;
            public const string InvoiceNumberErrorMessage = "Invoice number must start with 'INV' followed by digits.";
            public const string IssueDateErrorMessage = "Issue date cannot be in the future.";
            public const string DueDateErrorMessage = "Due date must be greater than issue date.";
            public const string TimeComponentErrorMassage = "Date cannot have a time component.";
            public const int BankAccountNumberMinLength = 8;
            public const int BankAccountNumberMaxLength = 28;
            public const string BankAccountNumberErrorMessage = "Bank account number must start with two uppercase letters followed by digits.";
            public const string CustomerDoesntExistsErrorMessage = "Customer does not exist.";
            public const string ClientDoesntExistsErrorMessage = "Client does not exist.";
            public const string InvoiceNumberExistsMessage = "Invoice number already exists.";
            public const string ItemsDuplicateErrorMessage = "Items must not be duplicated.";
            public const string ItemDoesNotExistsErrorMessage = "Item does not exist.";
        }

        public static class Item
        {
            public const int DescriptionMaxLength = 250;
            public const string DescriptionShouldStartWithCapitalMessage = "Description must begin with a capital letter.";
            public const string ClientDoesNotExistMessage = "Client does not exist.";
            public const string CurrencyDoesNotExistMessage = "Currency does not exist.";
            public const string NegativePriceErrorMessage = "Price must be positive";
            public const string ClientIdCannotBeChangedMessage = "Client Id cannot be changed.";
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

        public static class Plan
        {
            public const string ItemDoesNotExistMessage = "Item does not exist";
            public const string ClientDoesNotExistMessage = "Client does not exist";
            public const string PlanStartDateErrorMessage = "Start date must be less than end date";
            public const string PlandEndDateInThePastErrorMessage = "End date must be in the future";
            public const string PlanAlreadyExistsForItemAndClient = "Plan already exists for this item and client";
        }
    }

    public static class File
    {
        public static class Extension
        {
            public const string Pdf = ".pdf";
        }

        public static class Name
        {
            public const string Invoice = "Invoice_";
        }
    }
}
