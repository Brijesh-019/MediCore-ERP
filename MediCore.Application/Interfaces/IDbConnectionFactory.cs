using System.Data;

namespace MediCore.Application.Interfaces;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}