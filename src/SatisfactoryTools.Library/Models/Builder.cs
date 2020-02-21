namespace SatisfactoryTools.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    public enum Builder
    {
        [Description("Build Tool")]
        BuildTool,

        Workbench,

        Workshop,

        Smelter,

        Foundry,

        Constructor,

        Assembler,

        Manufacturer,

        [Description("Water Extractor")]
        WaterExtractor,

        [Description("Oil Pump")]
        OilPump,

        [Description("Oil Refinery")]
        OilRefinery,

        Miner
    }
}