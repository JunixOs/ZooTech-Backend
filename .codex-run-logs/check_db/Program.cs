using System;
using Microsoft.Data.SqlClient;

class Program
{
    static void Main()
    {
        string connStr = "Server=69.164.246.85,1433;Database=zootech;User Id=sebastian.ramirez;Password=ZooTech@2026#10;TrustServerCertificate=True;";
        using (var conn = new SqlConnection(connStr))
        {
            conn.Open();
            using (var cmd = new SqlCommand("SELECT id, codigo, nombre FROM vacuno", conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine($"ID: {reader["id"]}, Codigo: {reader["codigo"]}, Nombre: {reader["nombre"]}");
                }
            }
        }
    }
}
