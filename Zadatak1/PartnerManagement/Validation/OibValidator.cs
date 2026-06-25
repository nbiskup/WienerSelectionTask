namespace PartnerManagement.Validation;

public static class OibValidator
{
    public static bool IsValid(string? value)
    {
        return !string.IsNullOrWhiteSpace(value)
            && value.Length == 11
            && value.All(char.IsDigit);
    }
}