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

        public static class Email
        {
            public const string IsTaken = "Email is already taken.";
            public const string IsInvalid = "Email is invalid.";
        }

        public static class Client
        {
            public const string DoesNotExist = "Client does not exist.";
        }

        public static class BankAccountNumber
        {
            public const int MinLength = 8;
            public const int MaxLength = 28;
            public const string InvalidFormat = "Bank account number must start with two uppercase letters followed by digits.";
        }

        public static class Customer
        {
            public const string DoesNotExist = "Customer does not exist.";
            public const string WrongClient = "Wrong Client is provided for Customer.";
        }

        public static class User
        {
            public const string DoesNotExist = "User does not exist.";
        }

        public static class UserAssignment
        {
            public const string GlobalRoleAssignment = "Global roles must be assigned to the Global Client";
            public const string ClientRoleAssignment = "Client roles cannot be assigned to the Global Client";
        }

        public static class Currency
        {
            public const string DoesNotExist = "Currency does not exist.";
        }

        public static class Name
        {
            public const string AlreadyTaken = "Name is already taken";
            public const int MaxLength = 100;
            public const string MustStartWithCapital = "Name must begin with a capital letter.";
            public const string IsTaken = "Name is already taken";
        }

        public static class Password
        {
            public const int MinimumLength = 8;
            public const string RequireUppercase = "Password must have at least one uppercase letter.";
            public const string RequireLowercase = "Password must have at least one lowercase letter.";
            public const string RequireDigit = "Password must have at least one digit.";
            public const string RequireSpecialCharacter = "Password must contain at least one special character.";
        }

        public static class Country
        {
            public const int MaxLength = 56;
            public const string MustStartWithCapital = "Country must begin with a capital letter.";
            public const string DoesNotExist = "Country does not exist.";
        }

        public static class Invoice
        {
            public const int InvoiceNumberMaxLength = 15;
            public const string InvalidInvoiceNumberFormat = "Invoice number must start with 'INV' followed by digits.";
            public const string IssueDateInFuture = "Issue date cannot be in the future.";
            public const string DueDateBeforeIssueDate = "Due date must be greater than issue date.";
            public const string TimeComponentNotAllowed = "Date cannot have a time component.";
            public const string DuplicateInvoice = "Invoice number already exists.";
            public const string DuplicateItem = "Items must not be duplicated.";
        }

        public static class Item
        {
            public const int DescriptionMaxLength = 250;
            public const string DescriptionShouldStartWithCapital = "Description must begin with a capital letter.";
            public const string PriceMustBePositive = "Price must be positive";
            public const string ClientIdImmutable = "Client Id cannot be changed.";
            public const string DoesNotExist = "Item does not exist.";
        }

        public static class Import
        {
            public const string InvalidFileExtension = "The file must be in CSV format (.csv extension).";
            public const string EmptyFileUpload = "Uploaded file must not be empty.";
        }

        public static class Plan
        {
            public const string InvalidStartDate = "Start date must be less than end date";
            public const string EndDateInPast = "End date must be in the future";
            public const string AlreadyExistsForItemAndClient = "Plan already exists for this item and client";
        }
    }

    public static class ClientData
    {
        public static readonly Guid GlobalId = Guid.Parse("e47501a8-547b-4dc4-ba97-e65ccfc39477");
    }

    public static class File
    {
        public static class Extension
        {
            public const string Pdf = ".pdf";
        }
    }
}
