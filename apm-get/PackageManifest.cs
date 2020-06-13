using System;
using System.Collections.Generic;
using System.Text;

namespace apm_get
{
    public class PackageManifest
    {
        public enum ActionType { Copy , Run , Delete };

        public class Action
        {
            public ActionType Type { get; set; }
            public string Content { get; set; }
        }

        public string AppName { get; set; }

        public List<string> Dependencies { get; set; } = new List<string>();
        public List<Action> Actions { get; set; } = new List<Action>();

        public string Directory;
    }
}
