using System.Linq;

/// <summary>
/// A group of miscellaneous helper functions.
/// </summary>
public static class Miscellaneous
{
    /// <summary>
    /// Counts the true booleans in a sequence. E.g. count_true_booleans(true, false, true) returns 2.
    /// </summary>
    /// <param name="boolean_list">A comma separated list of (or array of) booleans.</param>
    /// <returns>The number of true values in the sequence.</returns>
    public static int count_true_booleans(params bool[] boolean_list)
    {
       return boolean_list.Count(is_true => is_true);
    }
}
