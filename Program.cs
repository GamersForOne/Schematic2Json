using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using fNbt;
using Newtonsoft.Json;

namespace Schematic2Json
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string file;

            if (args.Length > 0)
            {
                file = args[0];
            }
            else
            {
                Console.Write("Schematic file path: ");
                file = Console.ReadLine();
                Console.WriteLine();
            }

            if (file == null || !File.Exists(file))
            {
                Console.WriteLine("File not found.");
                if (args.Contains("nopause")) return;
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
                Environment.Exit(1);
            }

			var extension = Path.GetExtension(file);
			var newFile = file.Replace(extension, ".json");

			string reason;

			var scale = 1.0f;
			if (args.Any(p=>p.StartsWith("scale")))
			{
				var scaleStr = args.Where(p => p.StartsWith("scale")).Select(p => p.Substring(5)).First();
				
				if (!float.TryParse(scaleStr, out scale))
				{
					Console.WriteLine("Invalid scale format.");
					scale = 1.0f;
				}
			}

			if (!Convert(file, newFile, args, scale, out reason))
			{
				Console.WriteLine($"Error: {reason}");
				if (args.Contains("nopause")) return;
				Console.WriteLine("Press any key to continue...");
				Console.ReadKey(true);
				Environment.Exit(2);
			}

			Console.WriteLine("Press any key to continue...");
			Console.ReadKey(true);
        }

		static bool Convert(string fromFile, string toFile, string[] args, float scale, out string reason)
		{
			if (!File.Exists(fromFile))
				return ConvertResult("Input file not found", out reason);

			Console.WriteLine("Converting...");

			var file = new NbtFile();
			file.LoadFromFile(fromFile);
			var root = file.RootTag;
			var world = new World(root);
			var elements = new List<Model.Element>();
			var random = new Random();
			var usedTextures = new List<string>();

			var model = new Model
			{
				__comment = "Converted using Schematic2Json by MicleBrick, edited by Kirdow. NBT data read with https://github.com/fragmer/fNbt. JSON written with https://json.net",
				textures = new Dictionary<string, string>()
			};
			BlockHandler.AddTextures(model);

			for (var x = 0; x < world.Width; x++)
			{
				for (var y = 0; y < world.Height; y++)
				{
					for (var z = 0; z < world.Depth; z++)
					{
						var block = world[x, y, z];
						if (block == 0)
							continue;
						var data = world.BlockData[world.Index(x, y, z)];

						byte texture = 0;

						if (block == 35 || block == 159) texture = data;
						if (block == 159) texture += 16;

						var slab = false;
						var top = false;

						if (block == 44)
						{
							slab = true;

							switch (data)
							{
								case 8:
									texture = 32;
									top = true;
									break;
								case 0:
									texture = 32;
									break;
								case 1:
									texture = 33;
									break;
								case 9:
									texture = 33;
									top = true;
									break;
								case 3:
									texture = 34;
									break;
								case 11:
									texture = 34;
									top = true;
									break;
								case 4:
									texture = 35;
									break;
								case 12:
									texture = 35;
									top = true;
									break;
								case 5:
									texture = 36;
									break;
								case 13:
									texture = 36;
									top = true;
									break;
								case 6:
									texture = 37;
									break;
								case 14:
									texture = 37;
									top = true;
									break;
								case 7:
									texture = 38;
									break;
								case 15:
									texture = 38;
									top = true;
									break;
								default:
									texture = 32;
									break;
							}
						}

						var rnd = (float)random.NextDouble() * 16;
						if (args.Contains("nonoise")) rnd = 0;
						var face = new Model.Element.Face { texture = "#" + texture, uv = new[] { rnd, rnd, rnd, rnd } };
						var element = new Model.Element { from = new[] { x * scale - 32, (y + (slab && top ? 0.5f : 0)) * scale - 32, z * scale - 32 }, to = new[] { (x + 1) * scale - 32, (y + (slab && !top ? 0.5f : 1f)) * scale - 32, (z + 1) * scale - 32 }, faces = new Dictionary<string, Model.Element.Face>() };
						if (world[x, y, z, Face.XN])
							element.faces.Add("West", face);
						if (world[x, y, z, Face.ZN])
							element.faces.Add("North", face);
						if (world[x, y, z, Face.XP])
							element.faces.Add("East", face);
						if (world[x, y, z, Face.ZP])
							element.faces.Add("South", face);
						if (world[x, y, z, Face.YN])
							element.faces.Add("Down", face);
						if (world[x, y, z, Face.YP])
							element.faces.Add("Up", face);

						if (element.faces.Count > 0)
						{
							if (!usedTextures.Contains(texture.ToString()))
								usedTextures.Add(texture.ToString());

							elements.Add(element);
						}
					}
				}
			}

			foreach (var key in model.textures.Keys.ToList())
				if (!usedTextures.Contains(key))
					model.textures.Remove(key);
			if (!args.Contains("noscale"))
			{
				var max = elements.Select(element => element.to.Max()).Concat(new float[] { 0 }).Max();
				var changeAmount = (max - 32.0f) / max;
				if (changeAmount > 0.0)
				{
					Console.WriteLine("Scaling...");
					foreach (var element in elements)
					{
						for (var i = 0; i < 3; i++)
							element.from[i] = Math.Max(element.from[i] - changeAmount * element.from[i], 0);
						for (var i = 0; i < 3; i++)
							element.to[i] = Math.Max(element.to[i] - changeAmount * element.to[i], 0);
					}
				}
			}

			model.elements = elements.ToArray();
			Console.WriteLine("Serializing...");
			File.WriteAllText(toFile, JsonConvert.SerializeObject(model, Formatting.Indented));
			Console.WriteLine("Written to json File: " + toFile);
			return ConvertResult(null, out reason);
		}

		static bool ConvertResult(string reasonText, out string reason)
		{
			if (reasonText == null)
			{
				reason = null;
				return true;
			}
			reason = reasonText;
			return false;
		}
    }
}