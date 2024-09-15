using System.Diagnostics;
using System.Runtime.Serialization;
using LambertEditor.Common;

namespace LambertEditor.Components;
[DataContract]
public class Component : ViewModelBase
{
    [DataMember]
    public GameEntity Owner { get; private set; }

    public Component(GameEntity owner)
    {
        Debug.Assert(owner != null);
        Owner = owner;
    }
}