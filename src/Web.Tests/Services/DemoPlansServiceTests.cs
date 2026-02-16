using Microsoft.VisualStudio.TestTools.UnitTesting;
using Web.Models.Factory;
using Web.Services;

namespace Web.Tests.Services;

[TestClass]
public class DemoPlansServiceTests
{
    [TestMethod]
    public void GetSimpleDemoPlanShouldReturnFactories()
    {
        // Arrange
        DemoPlansService service = new DemoPlansService();

        // Act
        List<Factory> factories = service.GetSimpleDemoPlan();

        // Assert
        Assert.IsNotNull(factories);
        Assert.AreEqual(2, factories.Count);
    }

    [TestMethod]
    public void GetSimpleDemoPlanShouldHaveCorrectFactoryNames()
    {
        // Arrange
        DemoPlansService service = new DemoPlansService();

        // Act
        List<Factory> factories = service.GetSimpleDemoPlan();

        // Assert
        Assert.AreEqual("Iron Ingots", factories[0].Name);
        Assert.AreEqual("Iron Plates", factories[1].Name);
    }

    [TestMethod]
    public void GetSimpleDemoPlanShouldHaveCorrectFactoryIds()
    {
        // Arrange
        DemoPlansService service = new DemoPlansService();

        // Act
        List<Factory> factories = service.GetSimpleDemoPlan();

        // Assert
        Assert.AreEqual(0, factories[0].Id);
        Assert.AreEqual(1, factories[1].Id);
    }

    [TestMethod]
    public void GetSimpleDemoPlanFirstFactoryShouldHaveProduct()
    {
        // Arrange
        DemoPlansService service = new DemoPlansService();

        // Act
        List<Factory> factories = service.GetSimpleDemoPlan();

        // Assert
        Assert.IsNotNull(factories[0].Products);
        Assert.AreEqual(1, factories[0].Products.Count);
        Assert.AreEqual("IronIngot", factories[0].Products[0].Id);
        Assert.AreEqual(100, factories[0].Products[0].Amount);
    }

    [TestMethod]
    public void GetSimpleDemoPlanSecondFactoryShouldHaveInputAndProduct()
    {
        // Arrange
        DemoPlansService service = new DemoPlansService();

        // Act
        List<Factory> factories = service.GetSimpleDemoPlan();

        // Assert
        // Check product
        Assert.IsNotNull(factories[1].Products);
        Assert.AreEqual(1, factories[1].Products.Count);
        Assert.AreEqual("IronPlate", factories[1].Products[0].Id);
        Assert.AreEqual(100, factories[1].Products[0].Amount);

        // Check input
        Assert.IsNotNull(factories[1].Inputs);
        Assert.AreEqual(1, factories[1].Inputs.Count);
        Assert.AreEqual(0, factories[1].Inputs[0].FactoryId);
        Assert.AreEqual("IronIngot", factories[1].Inputs[0].OutputPart);
        Assert.AreEqual(100, factories[1].Inputs[0].Amount);
    }

    [TestMethod]
    public void GetAvailableTemplatesShouldReturnTemplateList()
    {
        // Arrange
        DemoPlansService service = new DemoPlansService();

        // Act
        List<DemoPlanTemplate> templates = service.GetAvailableTemplates();

        // Assert
        Assert.IsNotNull(templates);
        Assert.IsTrue(templates.Count > 0);
    }

    [TestMethod]
    public void GetAvailableTemplatesShouldIncludeSimpleTemplate()
    {
        // Arrange
        DemoPlansService service = new DemoPlansService();

        // Act
        List<DemoPlanTemplate> templates = service.GetAvailableTemplates();

        // Assert
        DemoPlanTemplate? simpleTemplate = templates.Find(t => t.Name == "Simple");
        Assert.IsNotNull(simpleTemplate);
        Assert.IsFalse(simpleTemplate.IsDebug);
        Assert.IsFalse(string.IsNullOrEmpty(simpleTemplate.Description));
    }

    [TestMethod]
    public void GetSimpleDemoPlanShouldReturnNewInstancesEachTime()
    {
        // Arrange
        DemoPlansService service = new DemoPlansService();

        // Act
        List<Factory> factories1 = service.GetSimpleDemoPlan();
        List<Factory> factories2 = service.GetSimpleDemoPlan();

        // Assert
        Assert.AreNotSame(factories1, factories2);
    }
}
