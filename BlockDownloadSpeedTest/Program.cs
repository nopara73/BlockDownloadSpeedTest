using NBitcoin;
using QBitNinja.Client;
using QBitNinja.Client.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BlockDownloadSpeedTest
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Every day 144 new blocks added to The Blockchain");

			var blockDownNum = 144;
			var startHeight = 462001;
			var endHeight = startHeight + blockDownNum - 1;
			Console.WriteLine($"{nameof(startHeight)}: {startHeight}; {nameof(endHeight)}: {endHeight}");

			Console.WriteLine($"Downloading {blockDownNum} blocks....");

			Console.WriteLine();
			Console.WriteLine("One by one...");
			DownloadBlocks(blockDownNum, startHeight, endHeight, 1);

			Console.WriteLine();
			Console.WriteLine("2 parallel...");
			DownloadBlocks(blockDownNum, startHeight, endHeight, 2);

			Console.WriteLine();
			Console.WriteLine("3 parallel...");
			DownloadBlocks(blockDownNum, startHeight, endHeight, 3);

			Console.WriteLine();
			Console.WriteLine("4 parallel...");
			DownloadBlocks(blockDownNum, startHeight, endHeight, 4);

			Console.WriteLine();
			Console.WriteLine("5 parallel...");
			DownloadBlocks(blockDownNum, startHeight, endHeight, 5);

			Console.WriteLine();
			Console.WriteLine("7 parallel...");
			DownloadBlocks(blockDownNum, startHeight, endHeight, 7);

			Console.WriteLine();
			Console.WriteLine("10 parallel...");
			DownloadBlocks(blockDownNum, startHeight, endHeight, 10);

			Console.WriteLine();
			Console.WriteLine("21 parallel...");
			DownloadBlocks(blockDownNum, startHeight, endHeight, 21);

			Console.WriteLine();

			Console.BackgroundColor = ConsoleColor.White;
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Test finished, press a key to exit...");
			Console.ResetColor();
			Console.ReadKey();
		}

		private static void DownloadBlocks(int blockDownNum, int startHeight, int endHeight, int parallel)
		{
			var client = new QBitNinjaClient(Network.Main);

			var sw = Stopwatch.StartNew();

			var sizes = new HashSet<int>();
			var tasks = new HashSet<Task<GetBlockResponse>>();
			for (int currentHeight = startHeight; currentHeight <= endHeight; currentHeight++)
			{
				var bf = new BlockFeature(currentHeight);
				var task = client.GetBlock(bf);
				tasks.Add(task);

				if (tasks.Count >= parallel || currentHeight == endHeight)
				{
					Task.WhenAll(tasks).Wait();

					foreach (var t in tasks)
					{
						var block = t.Result.Block;
						sizes.Add(block.GetSerializedSize());
					}
					tasks.Clear();
					Console.Write($"{currentHeight - startHeight + 1};");
				}
			}

			sw.Stop();
			Console.WriteLine();

			var size = sizes.Sum();

			Console.WriteLine("Average blocksize (KB): {0}", (size / blockDownNum) / 1024);
			Console.WriteLine("Download duration: {0}", sw.Elapsed.ToString("mm\\:ss"));

			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("Speed kbps: {0}", (int)((size / 1024) / sw.Elapsed.TotalSeconds));
			Console.ResetColor();
		}
	}
}