using MikietaApi.Models;
using Shouldly;

namespace MikietaApi.Tests.Models;

public class PagedResultTest
{
    [TestCase(30, 61, 3)]
    [TestCase(30, 60, 2)]
    [TestCase(30, 50, 2)]
    [TestCase(30, 30, 1)]
    public void Create(int arrayLength, int maxLength, int expectedPageCount)
    {
        //Arrange
        var array = Enumerable.Range(0, arrayLength).ToArray();

        //Act
        var res = PagedResult<int>.Create(array, maxLength);

        //Assert
        res.MaxRowCount.ShouldBe(maxLength);
        res.MaxPageCount.ShouldBe(expectedPageCount);
    }
}