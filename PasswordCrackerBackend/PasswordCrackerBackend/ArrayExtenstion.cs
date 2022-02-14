namespace PasswordCrackerBackend
{
    public static class ArrayExtenstion
    {
        public static IEnumerable<T[]> Split<T>(this T[] array, int size, int remainder)
        {
            for (var i = 0; i < array.Length / size; i++)
            {
                if (remainder == 0)
                {
                    yield return array.Skip(i * size).Take(size).ToArray();
                }
                if (remainder > i)
                {
                    yield return array.Skip(i * size + i).Take(size + 1).ToArray();
                }
                else
                {
                    yield return array.Skip(i * size + remainder).Take(size).ToArray();
                }
            }
        }
    }
}
