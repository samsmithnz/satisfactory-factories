using Web.Services;

namespace Web.Tests.Services;

[TestClass]
public sealed class GameDataServiceTests
{
    [TestMethod]
    public void GetWikiUrl_ShouldReturnEmptyString_ForEmptyDisplayName()
    {
        // Act
        string result = GameDataService.GetWikiUrl(string.Empty);

        // Assert
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void GetWikiUrl_ShouldReturnEmptyString_ForNullDisplayName()
    {
        // Act
        string result = GameDataService.GetWikiUrl(null!);

        // Assert
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void GetWikiUrl_ShouldConvertSpacesToUnderscores()
    {
        // Act
        string result = GameDataService.GetWikiUrl("Iron Ore");

        // Assert
        Assert.AreEqual("https://satisfactory.wiki.gg/wiki/Iron_Ore", result);
    }

    [TestMethod]
    public void GetWikiUrl_ShouldHandleMultipleSpaces()
    {
        // Act
        string result = GameDataService.GetWikiUrl("AI Limiter");

        // Assert
        Assert.AreEqual("https://satisfactory.wiki.gg/wiki/AI_Limiter", result);
    }

    [TestMethod]
    public void GetWikiUrl_ShouldHandleSingleWordDisplayName()
    {
        // Act
        string result = GameDataService.GetWikiUrl("Iron");

        // Assert
        Assert.AreEqual("https://satisfactory.wiki.gg/wiki/Iron", result);
    }

    [TestMethod]
    public void GetWikiUrl_ShouldEncodeSpecialCharacters()
    {
        // Act
        string result = GameDataService.GetWikiUrl("Smart Plating");

        // Assert
        Assert.AreEqual("https://satisfactory.wiki.gg/wiki/Smart_Plating", result);
    }

    [TestMethod]
    public void GetWikiUrl_ShouldPreserveCaseSensitivity()
    {
        // Act
        string result = GameDataService.GetWikiUrl("Quantum Encoder");

        // Assert
        Assert.AreEqual("https://satisfactory.wiki.gg/wiki/Quantum_Encoder", result);
    }
}
