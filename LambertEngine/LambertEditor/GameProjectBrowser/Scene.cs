﻿using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using LambertEditor.Common;
using LambertEditor.Components;

namespace LambertEditor.GameProjectBrowser;

[DataContract]
public class Scene : ViewModelBase
{
    private string _name;

    [DataMember]
    public string Name
    {
        get => _name;
        set
        {
            if (_name == value) return;
            _name = value;
            OnPropertyChanged(nameof(Name));
        }
    }

    [DataMember] public Project Project { get; private set; }
    private bool _isActive;

    [DataMember]
    public bool IsActive
    {
        get => _isActive;
        set
        {
            if (_isActive) return;
            _isActive = value;
            OnPropertyChanged(nameof(IsActive));
        }
    }
    [DataMember(Name = nameof(GameEntities))]
    private readonly ObservableCollection<GameEntity> _gameEntities = new();
    public ReadOnlyObservableCollection<GameEntity> GameEntities { get; }

    public Scene(Project project, string name)
    {
        Debug.Assert(project != null);
        Project = project;
        Name = name;
    }
}