using Newtonsoft.Json;
using System.Collections.Generic;
// ReSharper disable InconsistentNaming

namespace Schematic2Json
{
    public class Model
    {
		[JsonProperty(Order = 0)]
        public string __comment;
		[JsonProperty(Order = 1)]
        public Dictionary<string, string> textures;
		[JsonProperty(Order = 2)]
        public Element[] elements;

        public class Element
        {
			[JsonProperty(Order = 0)]
            public float[] from;
			[JsonProperty(Order = 1)]
            public float[] to;
			[JsonProperty(Order = 2)]
            public Dictionary<string, Face> faces;

            public class Face
            {
				[JsonProperty(Order = 0)]
                public string texture;
				[JsonProperty(Order = 1)]
                public float[] uv;
            }
        }
    }
}