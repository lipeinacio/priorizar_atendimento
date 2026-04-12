using Microsoft.Extensions.Options;
using Npgsql;
using PriorizarAtendimento.Api.Models;

namespace PriorizarAtendimento.Api.Services;

public class DatabaseConnectionFactory
{
    private readonly DatabaseSettings _settings;

    public DatabaseConnectionFactory(IOptions<DatabaseSettings> options)
    {
        _settings = options.Value;
    }

    public NpgsqlConnection CriarConexao()
    {
        return new NpgsqlConnection(_settings.ConnectionString);
    }
}