namespace SatisfactoryTools.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Merger : NodeTransformer
    {
        private IReadOnlyList<PartIo> inputs;

        public Merger()
        {
            base.Outputs = new[] { new PartIo { Part = Part.None, Count = 0, Rate = 0 } };
        }

        public override IReadOnlyList<PartIo> Inputs
        {
            get => base.Inputs;
            set
            {
                if (value.Count > 3)
                {
                    throw new NotSupportedException("Cannot assign more than 3 inputs to a Merger");
                }

                if (value.Select(x => x.Part).Distinct().Count() > 1)
                {
                    throw new NotImplementedException("Cannot model different parts into a Merger yet");
                }

                this.inputs = value;

                this.Output.Part = this.inputs[0].Part;
                this.Output.Rate = this.inputs.Sum(x => x.Rate);
            }
        }

        public PartIo Output => base.Outputs[0];

        public override IReadOnlyList<PartIo> Outputs
        {
            get => base.Outputs;
            set => throw new NotSupportedException($"Adjust outputs through {nameof(this.Inputs)}");
        }

        internal override NodeTransformer Clone(CloneFilters filters)
        {
            var clone = new Merger
            {
                Inputs = filters.HasFlag(CloneFilters.Backward) ? this.inputs.Select(x => x.Clone()).ToArray() : Array.Empty<PartIo>(),
            };

            return clone;
        }
    }
}