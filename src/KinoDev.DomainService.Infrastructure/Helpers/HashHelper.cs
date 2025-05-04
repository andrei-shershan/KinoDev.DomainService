using System.Security.Cryptography;
using System.Text;

namespace KinoDev.DomainService.Infrastructure.Helpers
{
    // TODO: Check if this helper is used
    public static class HashHelper
    {
        public static string CalculateSHA256(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                byte[] hash = sha256.ComputeHash(bytes);
                
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    builder.Append(hash[i].ToString("x2"));
                }
                
                return builder.ToString();
            }
        }

        public static string CalculateSHA256(params string[] inputs)
        {
            if (inputs == null || inputs.Length == 0)
                return string.Empty;

            inputs = inputs.Where(input => !string.IsNullOrWhiteSpace(input)).ToArray();    

            string combinedInput = string.Join("", inputs);
            return CalculateSHA256(combinedInput);
        }
    }
}
