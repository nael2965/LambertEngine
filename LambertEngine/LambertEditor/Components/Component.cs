using System.Diagnostics;
using LambertEditor.Common;

namespace LambertEditor.Components;

public class Component : ViewModelBase
{
    public GameEntity Owner { get; private set; }

    public Component(GameEntity owner)
    {
        Debug.Assert(owner != null);
        Owner = owner;
    }
}