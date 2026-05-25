using System.IO;

namespace NoisyNeighbors
{
	public struct Input
	{
	
		public static Input Parse(TextReader reader)
		{
          
		    return new Input
		    {
		        Row = reader.NextNumber(),
		        Columns = reader.NextNumber(),
		        Tenants = reader.NextNumber(),
		    };
		}

        public int Row { get; set; }
        public int Columns { get; set; }
        public int Tenants  { get; set; }
      
	}
}