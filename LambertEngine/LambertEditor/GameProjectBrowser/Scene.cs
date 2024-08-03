using System.Diagnostics;
using LambertEditor.Common;

namespace LambertEditor.GameProjectBrowser
{
    public class Scene : ViewModelBase
    {
        private string _name;
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

        public Project Project { get; private set; }

        public Scene(Project project, string name)
        {
            Debug.Assert(project != null);
            Project = project;
            Name = name;
        }
    }
}

