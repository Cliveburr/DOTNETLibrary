using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;

namespace Build.Scripts
{
    public static class CheckAssert
    {
        public static List<string> Errors { get; } = new List<string>();

        [StackTraceHidden]
        public static void MustNotNull<T>([NotNull] T? obj, string message)
        {
            if (obj is null)
            {
                Errors.Add(message);
            }
        }

        [StackTraceHidden]
        public static void Complete()
        {
            var message = string.Join(Environment.NewLine, Errors);
            Errors.Clear();
            if (!string.IsNullOrEmpty(message))
            {
                throw new Exception(message);
            }
        }
    }
}
