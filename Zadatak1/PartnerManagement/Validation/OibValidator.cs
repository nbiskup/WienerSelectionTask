namespace PartnerManagement.Validation;

public static class OibValidator
{
    public static bool IsValid(string? value)
    {
        if (!HasExpectedFormat(value))
        {
            return false;
        }

        return HasValidCheckDigit(value!);
    }

    private static bool HasExpectedFormat(string? value)
    {
        return !string.IsNullOrWhiteSpace(value)
            && value.Length == 11
            && value.All(char.IsDigit);
    }

    private static bool HasValidCheckDigit(string value)
    {
        // Croatian OIB uses the ISO 7064 MOD 11,10 control digit algorithm.
        var remainder = 10;

        for (var index = 0; index < 10; index++)
        {
            var digit = value[index] - '0';
            remainder = (remainder + digit) % 10;

            if (remainder == 0)
            {
                remainder = 10;
            }

            remainder = (remainder * 2) % 11;
        }

        var expectedCheckDigit = 11 - remainder;
        if (expectedCheckDigit == 10)
        {
            expectedCheckDigit = 0;
        }

        var actualCheckDigit = value[10] - '0';
        return expectedCheckDigit == actualCheckDigit;
    }
}