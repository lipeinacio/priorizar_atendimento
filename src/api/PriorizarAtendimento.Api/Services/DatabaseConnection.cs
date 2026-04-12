using Microsoft.Extensions.Options;
using Npgsql;
using PriorizarAtendimento.Api.Models;

namespace PriorizarAtendimento.Api.Services;

public class DatabaseConnection
{
    private readonly DatabaseSettings _settings;

    public DatabaseConnection(IOptions<DatabaseSettings> options)
    {
        _settings = options.Value;
    }

    public NpgsqlConnection CriarConexao()
    {
        return new NpgsqlConnection(_settings.ConnectionString);
    }

    public async Task<int> TestarConexaoAsync()
    {
        await using var conexao = CriarConexao();
        await conexao.OpenAsync();

        await using var comando = new NpgsqlCommand("select 1", conexao);
        var resultado = await comando.ExecuteScalarAsync();

        return Convert.ToInt32(resultado);
    }
}
