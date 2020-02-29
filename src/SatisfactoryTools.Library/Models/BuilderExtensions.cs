namespace SatisfactoryTools.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class BuilderExtensions
    {
        public static bool IsBuilding(this Builder builder)
        {
            return builder switch
            {
                Builder.BuildTool => false,
                Builder.Workbench => false,
                Builder.Workshop => false,
                var _ => true
            };
        }
    }
}