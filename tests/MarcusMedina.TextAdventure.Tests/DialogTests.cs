using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class DialogTests
{
    [Fact]
    public void DialogNode_StoresTextAndOptions()
    {
        var node = new DialogNode("Hello there.");

        Assert.Equal("Hello there.", node.Text);
        Assert.Empty(node.Options);
    }

    [Fact]
    public void DialogNode_AddOption_WiresNextNode()
    {
        var next = new DialogNode("Second line.");
        var node = new DialogNode("First line.")
            .AddOption("Continue", next);

        Assert.Single(node.Options);
        Assert.Equal("Continue", node.Options[0].Text);
        Assert.Equal(next, node.Options[0].Next);
    }
}
