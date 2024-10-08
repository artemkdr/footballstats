using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Runtime.CompilerServices;
using API.Data;
using API.Models;
using API.Tests;
using Microsoft.EntityFrameworkCore;

public class EFUtils
{
    private static string ConnectionString = TestContainers.GetConnectionString();

    private static readonly object _lock = new();
    
    public static void ClearTable(DbContext context, string tableName)
    {
        lock (_lock)
        {            
            context.Database.ExecuteSql(FormattableStringFactory.Create($"truncate table {tableName}"));            
        }
    }
    
    public static string GetTableName(Type entityType)
    {
        // Get the TableAttribute from the entity type
        var tableAttribute = entityType.GetCustomAttribute<TableAttribute>();

        // If the attribute exists, return the table name
        return tableAttribute?.Name ?? entityType.Name; // Fallback to class name if no attribute
    }
}