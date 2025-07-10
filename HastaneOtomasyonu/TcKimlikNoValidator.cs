using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HastaneOtomasyonu
{
    internal static class TcKimlikNoValidator
    {
        public static bool GecerliMi(string tc)
        {
            // Boşluk, uzunluk ve rakam kontrolü
            if (string.IsNullOrEmpty(tc) || tc.Length != 11 || !tc.All(char.IsDigit))
                return false;

            // Sayılar diziye çevriliyor
            int[] digits = tc.Select(x => int.Parse(x.ToString())).ToArray();

            // TC No 0 ile başlayamaz
            if (digits[0] == 0)
                return false;

            // İlk 9 haneye göre 10. hane kontrolü
            int sumOdd = digits[0] + digits[2] + digits[4] + digits[6] + digits[8];
            int sumEven = digits[1] + digits[3] + digits[5] + digits[7];
            int digit10 = ((sumOdd * 7) - sumEven) % 10;

            // İlk 10 haneye göre 11. hane kontrolü
            int digit11 = digits.Take(10).Sum() % 10;

            return digit10 == digits[9] && digit11 == digits[10];
        }
    }
}
