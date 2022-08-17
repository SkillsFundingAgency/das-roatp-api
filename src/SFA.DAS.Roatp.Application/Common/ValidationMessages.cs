namespace SFA.DAS.Roatp.Application.Common
{
    public static class ValidationMessages
    {
        public static class EmailValidationMessages
        {
            public const string EmailAddressTooLong = "Email address is too long, must be 256 characters or fewer";
            public const string EmailAddressWrongFormat = "Email address must be in the correct format, like name@example.com";
        }

        public static class PhoneNumberValidationMessages
        {
            public const string PhoneNumberWrongLength = "Telephone number must be between 10 and 50 characters";
        }

        public static class UrlValidationMessages
        {
            public static string UrlTooLong (string fieldName) => $"{fieldName} address is too long, must be 500 characters or fewer";
            public static string UrlWrongFormat(string fieldName) => $"{fieldName} address must be in the correct format, like www.example.com";
        }
    }
}
