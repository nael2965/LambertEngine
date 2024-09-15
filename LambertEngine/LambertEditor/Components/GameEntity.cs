using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using LambertEditor.Common;
using LambertEditor.GameProjectBrowser;

namespace LambertEditor.Components;

[DataContract]
[KnownType(typeof(Transform))]
public class GameEntity : ViewModelBase
{
    private string _name;
    [DataMember]
    public string Name
    {
        get => _name;
        set
        {
            if (value != _name)
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
    }
    
    [DataMember]
    public Scene ParentScene { get; private set; }
    
    [DataMember(Name = nameof(Components))]
    private readonly ObservableCollection<Component> _components = new ObservableCollection<Component>();
    
    public ReadOnlyObservableCollection<Component> Components { get; private set; }

    [OnDeserialized]
    void OnDeserialized(StreamingContext context)
    {
        if (_components != null)
        {
            Components = new ReadOnlyObservableCollection<Component>(_components);
            OnPropertyChanged(nameof(Components));
        }
    }
    
    public GameEntity(Scene scene/*, ObservableCollection<Component> components*/)
    {
        Debug.Assert(scene != null);
        ParentScene = scene;
        _components.Add(new Transform(this));
        /*Components = components;*/
    }
}