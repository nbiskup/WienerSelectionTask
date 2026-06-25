namespace PartnerManagement.Validation;

public static class OibValidator
{
    public static bool IsValid(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length != 11 || value.Any(c => !char.IsDigit(c)))
        {
            return false;
        }

        var a = 10;
        for (var i = 0; i < 10; i++)
        {
            a += value[i] - '0';
            a %= 10;
            if (a == 0)
            {
                a = 10;
            }

            a *= 2;
            a %= 11;
        }

        var controlDigit = 11 - a;
        if (controlDigit == 10)
        {
            controlDigit = 0;
        }

        return controlDigit == value[10] - '0';
    }
}