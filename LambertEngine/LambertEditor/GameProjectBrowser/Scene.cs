﻿using System.Diagnostics;
using System.Runtime.Serialization;
using LambertEditor.Common;

namespace LambertEditor.GameProjectBrowser
{
    [DataContract]
    public class Scene : ViewModelBase
    {
        private string _name;
        [DataMember]
        public string Name
        {
            get=>_name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }
        [DataMember]
        public Project Project { get; private set; }

        public Scene(Project project, string name)
        {
            Debug.Assert(project != null);
            Project = project;
            Name = name;
        }
    }
}

