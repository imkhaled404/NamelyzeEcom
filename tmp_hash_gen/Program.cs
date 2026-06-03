using System;
namespace tmp_hash_gen
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(BCrypt.Net.BCrypt.HashPassword("Admin@123"));
        }
    }
}
