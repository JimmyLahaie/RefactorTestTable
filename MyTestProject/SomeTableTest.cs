using System.Globalization;
using Bunit;
using FluentAssertions;
using HtmlTableTest.Pages;

namespace MyTestProject;

public class SomeTableTest
{
    private readonly IRenderedComponent<SomeTable> _component;

    public SomeTableTest()
    {
        Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-ca", false);
        var context = new TestContext();
        _component = context.RenderComponent<SomeTable>(p =>
            p.Add(x => x.Data, [
                new(1, "DEP0001", "BRAULT & BOUTHILLIER LTÉE", "0123", new DateOnly(2011, 5, 1), "00001-80002", 0, 10000.99, "Revenus", "une raison", new DateOnly(2011, 5, 31)),
                new(2, "DEP0002", "Pacini inc.", "0456", new DateOnly(2015, 7, 8), "11004-90006", 0, 5, "Dépenses", "Un commentaire", new DateOnly(2015, 7, 15)),
            ]));
    }
    
    [Fact]
    public void LeTableauAfficheCorrectement()
    {
        var headers = _component.FindAll("#theTable thead tr th").ToList();
        headers.Select(x => x.TextContent).Should()
            .BeEquivalentTo(["# Ref","Compagnie","# Produit","Date comptable","Compte","Débit","Crédit","Type","Description","Date de création","",""], opt => opt.WithStrictOrdering());
        
        var firstLine = _component.FindAll("#theTable tbody tr:nth-child(1) td");
        var secondLine = _component.FindAll("#theTable tbody tr:nth-child(2) td");

        firstLine.Select(x => x.TextContent).Should()
            .BeEquivalentTo([
                "DEP0001", "BRAULT & BOUTHILLIER LTÉE", "0123", "2011-05-01", "00001-80002", "0,00", "10000,99", "Revenus",
                "une raison", "2011-05-31", "", ""
            ], opt => opt.WithStrictOrdering());
        (firstLine[10].Children[0].Attributes["href"]?.Value).Should().Be("/edit/1");
        firstLine[10].Children[0].Children[0].ClassList.Should().Contain("bi-pen");
        (firstLine[11].Children[0].Attributes["href"]?.Value).Should().Be("/delete/1");
        firstLine[11].Children[0].Children[0].ClassList.Should().Contain("bi-trash");
        
        secondLine.Select(x => x.TextContent).Should()
            .BeEquivalentTo([
                "DEP0002", "Pacini inc.", "0456", "2015-07-08", "11004-90006", "0,00", "5,00", "Dépenses",
                "Un commentaire", "2015-07-15", "", ""
            ], opt => opt.WithStrictOrdering());
        (secondLine[10].Children[0].Attributes["href"]?.Value).Should().Be("/edit/2");
        secondLine[10].Children[0].Children[0].ClassList.Should().Contain("bi-pen");
        (secondLine[11].Children[0].Attributes["href"]?.Value).Should().Be("/delete/2");
        secondLine[11].Children[0].Children[0].ClassList.Should().Contain("bi-trash");
    }
}