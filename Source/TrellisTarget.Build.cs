using Flax.Build;

public sealed class TrellisTarget : GameProjectTarget
{
    /// <inheritdoc />
    public override void Init()
    {
        base.Init();

        // Reference the modules for game
        Modules.Add("Trellis");
    }
}