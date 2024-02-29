using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;

namespace Build.Scripts
{
    public static class Assert
    {
        [StackTraceHidden]
        public static void MustNotNull<T>([NotNull] T? obj, string message)
        {
            if (obj is null)
            {
                throw new Exception(message);
            }
        }
    }
}
