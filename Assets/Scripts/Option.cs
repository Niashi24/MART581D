using System;

namespace Mart581d
{
    [System.Serializable]
    public struct Option<T>
    {
        private bool isSome;
        private T value;

        public bool IsNone => !isSome;

        public static Option<T> Some(T value) => new Option<T>()
        {
            isSome = true,
            value = value
        };

        public static Option<T> None() => new();

        public bool IsSome(out T value)
        {
            value = this.value;
            return isSome;
        }

        public T Unwrap()
        {
            if (IsNone)
            {
                throw new NullReferenceException("tried to unwrap Option with no value");
            }

            return value;
        }

        public static Option<T> OfNullable(T value)
        {
            if (value == null)
                return None();
            return Some(value);
        }

        public Option<T> Filter(Predicate<T> predicate)
        {
            if (IsNone)
                return this;
            return predicate(value) ? this : None();
        }

        public bool IsSomeAnd(Predicate<T> predicate)
        {
            return isSome && predicate(value);
        }

        public bool IsNoneOr(Predicate<T> predicate)
        {
            return IsNone || predicate(value);
        }

        public T UnwrapOr(T value) => isSome ? this.value : value;
    }
}