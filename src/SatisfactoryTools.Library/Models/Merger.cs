namespace SatisfactoryTools.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Merger : NodeTransformer
    {
        private readonly PartIo[] inputs;

        public Merger()
        {
            this.inputs = new PartIo[3];
            this.Output = new PartIo {Part = Part.None, Count = 0, Rate = 0};
            this.SetOutputs(new[] {this.Output});
            this.SetInputs(this.inputs);
        }

        public PartIo Output { get; }

        internal override NodeTransformer Clone(CloneFilters filters)
        {
            var clone = new Merger();

            for (int i = 0; i < 3; i++)
            {
                PartIo input = this.inputs[i]?.Clone();

                if (input != null)
                {
                    this.ConnectInput(input, i);
                }
            }

            return clone;
        }

        public void ConnectInput(PartIo io, int inputNumber)
        {
            if (inputNumber < 0 || inputNumber > 2)
            {
                throw new ArgumentOutOfRangeException(nameof(inputNumber), "Input number must be between 0 and 2");
            }

            this.inputs[inputNumber] = null;

            if (this.inputs.Where(x => x != null).Any(x => x.Part != io.Part))
            {
                throw new NotImplementedException("Cannot model different parts into a Merger yet");
            }

            this.inputs[inputNumber] = io;
            this.Output.Part = io.Part;
            this.Output.Rate = this.inputs.Sum(x => x.Rate);
        }
    }
}