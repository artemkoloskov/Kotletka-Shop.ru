namespace KotletkaShop.Tests;

public class PaginatedListTests
{
    [Fact]
    public void Constructor_SetsProperties()
    {
        // Arrange
        var items = new List<int> { 1, 2, 3 };
        int count = 3;
        int pageIndex = 1;
        int pageSize = 10;

        // Act
        var paginatedList = new PaginatedList<int>(items, count, pageIndex, pageSize);

        // Assert
        Assert.Equal(pageIndex, paginatedList.PageIndex);
        Assert.Equal(1, paginatedList.TotalPages);
        Assert.Equal(items, paginatedList);
    }
}
