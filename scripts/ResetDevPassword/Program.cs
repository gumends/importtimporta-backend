using Microsoft.Data.SqlClient;

var email = args.Length > 0 ? args[0] : "gustavo.mendes@gmail.com";
var senha = args.Length > 1 ? args[1] : "Import@123";
var hash = BCrypt.Net.BCrypt.HashPassword(senha);

const string cs =
    "Server=localhost\\SQLEXPRESS;Database=importtimporta;Trusted_Connection=True;TrustServerCertificate=True";

await using var conn = new SqlConnection(cs);
await conn.OpenAsync();

await using var cmd = conn.CreateCommand();
cmd.CommandText = "UPDATE users SET Senha = @senha WHERE Email = @email";
cmd.Parameters.AddWithValue("@senha", hash);
cmd.Parameters.AddWithValue("@email", email);

var n = await cmd.ExecuteNonQueryAsync();
Console.WriteLine(n > 0
    ? $"Senha atualizada para {email} -> {senha}"
    : $"Usuário não encontrado: {email}");
