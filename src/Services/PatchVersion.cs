using System.Globalization;
using System.Text.RegularExpressions;

internal static class PatchVersion
{
    private static readonly Regex NumericPrefix = new(@"^\d+(\.\d+){0,3}", RegexOptions.Compiled);
    private static readonly Regex SuffixParts = new(@"^(?<prefix>.*?)(?<number>\d*)$", RegexOptions.Compiled | RegexOptions.Singleline);

    /// <summary>Bỏ tiền tố "v", "V" hoặc "v." và khoảng trắng thừa.</summary>
    public static string Normalize(string? version)
    {
        var normalized = (version ?? "").Trim();
        if (normalized.StartsWith("v.", StringComparison.OrdinalIgnoreCase))
            return normalized[2..].Trim();
        if (normalized.StartsWith('v') || normalized.StartsWith('V'))
            return normalized[1..].Trim();
        return normalized;
    }

    public static bool IsParseable(string? version) => TryParse(version, out _, out _);

    public static bool TryParse(string? version, out Version numeric, out string suffix)
    {
        numeric = new Version(0, 0);
        suffix = "";

        var normalized = Normalize(version);
        var match = NumericPrefix.Match(normalized);
        if (!match.Success) return false;

        var parts = new int[4];
        var texts = match.Value.Split('.');
        for (var i = 0; i < texts.Length; i++)
        {
            if (!int.TryParse(texts[i], NumberStyles.None, CultureInfo.InvariantCulture, out parts[i]))
                return false;
        }

        // Định dạng ngày DD.MM.YYYY: so sánh theo (năm, tháng, ngày).
        if (texts.Length == 3 && texts[2].Length == 4)
            numeric = new Version(parts[2], parts[1], parts[0], 0);
        else
            numeric = new Version(parts[0], parts[1], parts[2], parts[3]); // thành phần thiếu coi như 0

        suffix = normalized[match.Length..].Trim().TrimStart('-', '+', '_', '.', ' ');
        return true;
    }

    /// <summary>
    /// So sánh hai phiên bản. Trả về null nếu một trong hai không phân tích được.
    /// Bản có hậu tố tiền phát hành (ví dụ -beta.2) thấp hơn bản không có hậu tố cùng số.
    /// </summary>
    public static int? Compare(string? left, string? right)
    {
        if (!TryParse(left, out var leftNumeric, out var leftSuffix)
            || !TryParse(right, out var rightNumeric, out var rightSuffix))
            return null;

        var result = leftNumeric.CompareTo(rightNumeric);
        if (result != 0) return Math.Sign(result);

        var leftHasSuffix = leftSuffix.Length > 0;
        var rightHasSuffix = rightSuffix.Length > 0;
        if (leftHasSuffix != rightHasSuffix)
            return leftHasSuffix ? -1 : 1;

        return CompareSuffix(leftSuffix, rightSuffix);
    }

    private static int CompareSuffix(string left, string right)
    {
        // Cùng phần chữ (beta, rc...) thì so sánh số ở cuối theo giá trị: beta2 < beta10.
        var l = SuffixParts.Match(left);
        var r = SuffixParts.Match(right);
        if (l.Success && r.Success
            && string.Equals(l.Groups["prefix"].Value, r.Groups["prefix"].Value, StringComparison.OrdinalIgnoreCase))
        {
            var ln = ParseNumber(l.Groups["number"].Value);
            var rn = ParseNumber(r.Groups["number"].Value);
            return ln.CompareTo(rn);
        }

        return Math.Sign(string.Compare(left, right, StringComparison.OrdinalIgnoreCase));
    }

    private static System.Numerics.BigInteger ParseNumber(string digits) =>
        digits.Length == 0 ? System.Numerics.BigInteger.Zero : System.Numerics.BigInteger.Parse(digits, CultureInfo.InvariantCulture);
}
