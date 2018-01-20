using fNbt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schematic2Json
{
	public class World
	{
		public int Width { get; set; }

		public int Height { get; set; }

		public int Depth { get; set; }

		public byte[] Blocks { get; set; }

		public byte[] BlockData { get; set; }

		public World(int w, int h, int d)
		{
			Width = w;
			Height = h;
			Depth = d;
			Blocks = new byte[w * h * d];
		}

		public World(NbtCompound rootTag)
		{
			Width = rootTag.Get<NbtShort>("Width").ShortValue;
			Height = rootTag.Get<NbtShort>("Height").ShortValue;
			Depth = rootTag.Get<NbtShort>("Depth").ShortValue;
			Blocks = rootTag.Get<NbtByteArray>("Blocks").ByteArrayValue;
			BlockData = rootTag.Get<NbtByteArray>("Data").ByteArrayValue;
		}

		public int Index(int x, int y, int z)
		{
			return (y * Depth + z) * Width + x;
		}

		public int[] Vec(int index)
		{
			int[] vec = new int[3];
			vec[0] = index % Width;
			index /= Width;
			vec[2] = index % Depth;
			index /= Depth;
			vec[1] = index;
			return vec;
		}

		public byte this[int x, int y, int z]
		{
			get
			{
				if (x < 0 || y < 0 || z < 0 || x >= Width || y >= Height || z >= Depth)
					return 0;
				return Blocks[Index(x, y, z)];
			}
			set
			{
				Blocks[Index(x, y, z)] = value;
			}
		}

		private bool this[byte block]
		{
			get
			{
				switch (block)
				{
					case 0:
					case 44:
						return true;
					default:
						return false;
				}
			}
		}

		public bool this[int x, int y, int z, Face relative]
		{
			get
			{
				byte block = this[x, y, z];
				if (block == 0)
					return false;
				if (x == 0 && relative == Face.XN)
					return true;
				if (y == 0 && relative == Face.YN)
					return true;
				if (z == 0 && relative == Face.ZN)
					return true;
				if (x == Width - 1 && relative == Face.XP)
					return true;
				if (y == Height - 1 && relative == Face.YP)
					return true;
				if (z == Depth - 1 && relative == Face.ZP)
					return true;
				switch (relative)
				{
					case Face.XN:
						return this[this[x - 1, y, z]];
					case Face.YN:
						return this[this[x, y - 1, z]];
					case Face.ZN:
						return this[this[x, y, z - 1]];
					case Face.XP:
						return this[this[x + 1, y, z]];
					case Face.YP:
						return this[this[x, y + 1, z]];
					case Face.ZP:
						return this[this[x, y, z + 1]];
					default:
						return false;
				}
			}
		}
	}
}
