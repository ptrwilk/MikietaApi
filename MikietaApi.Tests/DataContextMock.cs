using Microsoft.EntityFrameworkCore;
using MikietaApi.Data;

namespace MikietaApi.Tests;

public class DataContextMock : DataContext
{
    public DataContextMock() : base(new DbContextOptions<DataContext>())
    {
    }
}