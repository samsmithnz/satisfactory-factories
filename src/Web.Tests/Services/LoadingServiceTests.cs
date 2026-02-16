using Microsoft.VisualStudio.TestTools.UnitTesting;
using Web.Services;

namespace Web.Tests.Services;

[TestClass]
public class LoadingServiceTests
{
    [TestMethod]
    public void InitializeShouldSetLoadingState()
    {
        // Arrange
        LoadingService service = new LoadingService();

        // Act
        service.Initialize("Test Loading", 5);

        // Assert
        Assert.IsTrue(service.IsLoading);
        Assert.AreEqual("Test Loading", service.Title);
        Assert.AreEqual(5, service.TotalSteps);
        Assert.AreEqual(0, service.CurrentStep);
        Assert.AreEqual(string.Empty, service.Message);
    }

    [TestMethod]
    public void IncrementStepShouldUpdateProgress()
    {
        // Arrange
        LoadingService service = new LoadingService();
        service.Initialize("Test Loading", 3);

        // Act
        service.IncrementStep("Step 1");

        // Assert
        Assert.AreEqual(1, service.CurrentStep);
        Assert.AreEqual("Step 1", service.Message);
        Assert.IsTrue(service.IsLoading);
    }

    [TestMethod]
    public void IncrementStepShouldCompleteWhenReachingTotalSteps()
    {
        // Arrange
        LoadingService service = new LoadingService();
        service.Initialize("Test Loading", 2);
        bool changeEventFired = false;
        service.OnChange += () => changeEventFired = true;

        // Act
        service.IncrementStep("Step 1");
        service.IncrementStep("Step 2");
        
        // Wait a bit for the async completion
        System.Threading.Thread.Sleep(200);

        // Assert
        Assert.IsFalse(service.IsLoading);
        Assert.IsTrue(changeEventFired);
    }

    [TestMethod]
    public void IncrementStepWithFinalStepFlagShouldComplete()
    {
        // Arrange
        LoadingService service = new LoadingService();
        service.Initialize("Test Loading", 5);

        // Act
        service.IncrementStep("Final step", isFinalStep: true);
        
        // Wait a bit for the async completion
        System.Threading.Thread.Sleep(200);

        // Assert
        Assert.IsFalse(service.IsLoading);
    }

    [TestMethod]
    public void SetStepShouldUpdateCurrentStepAndMessage()
    {
        // Arrange
        LoadingService service = new LoadingService();
        service.Initialize("Test Loading", 5);

        // Act
        service.SetStep(3, "Jump to step 3");

        // Assert
        Assert.AreEqual(3, service.CurrentStep);
        Assert.AreEqual("Jump to step 3", service.Message);
    }

    [TestMethod]
    public void CompleteShouldResetLoadingState()
    {
        // Arrange
        LoadingService service = new LoadingService();
        service.Initialize("Test Loading", 3);
        service.IncrementStep("Step 1");

        // Act
        service.Complete();

        // Assert
        Assert.IsFalse(service.IsLoading);
        Assert.AreEqual(0, service.CurrentStep);
        Assert.AreEqual(string.Empty, service.Message);
    }

    [TestMethod]
    public void OnChangeShouldFireWhenStateChanges()
    {
        // Arrange
        LoadingService service = new LoadingService();
        int changeCount = 0;
        service.OnChange += () => changeCount++;

        // Act
        service.Initialize("Test", 1);
        service.IncrementStep("Step 1");
        service.Complete();

        // Assert
        Assert.IsTrue(changeCount >= 3); // At least 3 state changes
    }
}
