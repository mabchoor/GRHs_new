using GRHs.Data;
using System;

public sealed class DbContextProvider
{
    private static readonly Lazy<EmployeeManagementDbContext> lazy =
        new Lazy<EmployeeManagementDbContext>(() => new EmployeeManagementDbContext());

    public static EmployeeManagementDbContext Instance => lazy.Value;

    // Private constructor to prevent instantiation
    private DbContextProvider() { }
}