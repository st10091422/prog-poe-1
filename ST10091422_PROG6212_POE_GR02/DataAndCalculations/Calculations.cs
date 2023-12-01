using System;
using System.Security.Cryptography;
using System.Text;

namespace DataAndCalculations
{
    // This class was adapted from YouTube
    //https://youtu.be/WYYRja4kee8?si=M7cb9Q7bdbofVB_o
    //SPDVI
    //https://www.youtube.com/@spdvi7370
    public class Calculations
    {
        public int SelfStudyHoursPerWeek(int numberOfCredits, int classHoursPerWeek, int numberOfWeeks)
        {// this method is derived frm the POE and uses the formula given
     
            return ((numberOfCredits * 10)/ numberOfWeeks) - classHoursPerWeek;
        }
        // Generate a random salt
        public byte[] GenerateSalt()
        {
            // Create a new byte array to store the salt
            byte[] salt = new byte[16];

            // Use a cryptographic random number generator to fill the salt with random bytes
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            // Return the generated salt
            return salt;
        }

        // Combine password and salt, then hash
        public byte[] HashPassword(string password, byte[] salt)
        {
            // Convert the user's password to bytes
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            // Create a new byte array to store the salted password
            byte[] saltedPassword = new byte[passwordBytes.Length + salt.Length];

            // Copy the user's password bytes and salt bytes into the saltedPassword array
            Buffer.BlockCopy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length);
            Buffer.BlockCopy(salt, 0, saltedPassword, passwordBytes.Length, salt.Length);

            // Create a SHA-256 hash object
            using (SHA256 sha256 = SHA256.Create())
            {
                // Compute the hash of the salted password
                return sha256.ComputeHash(saltedPassword);
            }
        }

        // Example of verifying a password
        public bool VerifyPassword(string enteredPassword, byte[] storedSalt, byte[] storedHashedPassword)
        {
            // Recreate the salted entered password for verification
            byte[] saltedEnteredPassword = HashPassword(enteredPassword, storedSalt);

            // Compare the recreated salted entered password with the stored hashed password
            return ByteArraysEqual(saltedEnteredPassword, storedHashedPassword);
        }

        // Compare two byte arrays for equality
        private bool ByteArraysEqual(byte[] a1, byte[] a2)
        {
            // Check if the arrays have the same length
            if (a1.Length != a2.Length)
                return false;

            // Compare each byte in the arrays
            for (int i = 0; i < a1.Length; i++)
            {
                if (a1[i] != a2[i])
                    return false;
            }

            // If all bytes are equal, the arrays are equal
            return true;
        }
    }

}