using fNbt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schematic2Json
{
	public class BlockHandler
	{
		public static void AddTextures(Model model)
		{
			foreach (WoolColor color in Enum.GetValues(typeof(WoolColor)))
			{
				model.textures.Add(((byte)color).ToString(), "blocks/wool_colored_" + (object)color);
				model.textures.Add((color + (byte)16).ToString(), "blocks/hardened_clay_stained_" + (object)color);
			}
			AddTexture(model, 32, "blocks/stone_slab_top", "blocks/sandstone_top", "blocks/cobblestone", "blocks/brick", "blocks/stonebrick", "blocks/nether_brick", "blocks/quartz_block_top");
		}

		private static void AddTexture(Model model, int start, params string[] textures)
		{
			for (int i = 0; i < textures.Length; i++)
				model.textures.Add((i + start).ToString(), textures[i]);
		}
	}
}
