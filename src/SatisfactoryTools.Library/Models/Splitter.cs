namespace SatisfactoryTools.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Splitter : NodeTransformer
    {
        private readonly PartIo[] outputs;

        public Splitter()
        {
            this.Input = new PartIo {Part = Part.None, Count = 0, Rate = 0};
            this.outputs = new[]
            {
                PartIo.CreateNone(),
                PartIo.CreateNone(),
                PartIo.CreateNone()
            };

            this.SetInputs(new[] {this.Input});
            this.SetOutputs(this.outputs);
        }

        public Splitter(PartIo input, bool[] connections)
        {
            if (connections.Length != 3)
            {
                throw new ArgumentOutOfRangeException(nameof(connections), "Expected 3 connection definitions");
            }

            this.Input = input;
            this.outputs = new PartIo[3];

            for (int i = 0; i < connections.Length; i++)
            {
                this.outputs[i] = connections[i] ? new PartIo {Part = input.Part} : PartIo.CreateNone();
            }

            this.ConfigureOutputs();
        }

        public int ConnectedOutputs => this.outputs.Count(x => x.Part != Part.None);

        public PartIo Input
        {
            get => this.Inputs.Count == 0 ? null : this.Inputs[0];
            set
            {
                this.SetInputs(value == null ? Array.Empty<PartIo>() : new[] {value});
                this.ConfigureOutputs();
            }
        }

        internal override NodeTransformer Clone(CloneFilters filters)
        {
            Splitter clone;

            if (filters.HasFlag(CloneFilters.Backward))
            {
                var connections = new bool[3];

                for (int i = 0; i < 3; i++)
                {
                    connections[i] = this.outputs[i].Part != Part.None;
                }

                clone = new Splitter(this.Input.Clone(), connections);
            }
            else
            {
                clone = new Splitter();
            }

            return clone;
        }


        private void ConfigureOutputs()
        {
            if (this.Input.Part == Part.None)
            {
                return;
            }

            int connections = this.ConnectedOutputs;
            double rate = this.Input.Rate / connections;

            for (int i = 0; i < connections; i++)
            {
                this.outputs[i].Part = this.Input.Part;
                this.outputs[i].Rate = rate;
            }
        }

        public void ConnectOutput(int outputNumber)
        {
            if (outputNumber < 0 || outputNumber > 2)
            {
                throw new ArgumentOutOfRangeException(nameof(outputNumber), "Input number must be between 0 and 2");
            }

            this.ConfigureOutputs();
        }
    }
}