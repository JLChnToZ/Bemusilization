using System;
using System.Collections.Generic;

namespace Utils {
    public static class Base36 {
        private static readonly char[] CharList = "0123456789abcdefghijklmnopqrstuvwxyz".ToCharArray();

        public static string Encode(int input) {
            if(input < 0) throw new ArgumentOutOfRangeException(nameof(input), input, "input cannot be negative");
            var result = new Stack<char>();
            do {
                result.Push(CharList[input % 36]);
                input /= 36;
            } while(input > 0);
            return new string(result.ToArray());
        }

        public static int Decode(string input) {
            int result = 0;
            int pos = input.Length - 1;
            int idx;
            foreach(char c in input.ToLower()) {
                idx = Array.IndexOf(CharList, c);
                if(idx < 0) return -1;
                result += idx * (int)Math.Pow(36, pos);
                pos--;
            }
            return result;
        }
    }
}
