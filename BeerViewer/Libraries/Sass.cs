﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using SharpScss;

namespace BeerViewer.Libraries
{
	public class Sass
	{
		public static Sass Instance { get; } = new Sass();

		public void Compile(string[] SassFiles)
		{
			var targets = SassFiles.Where(x => !Path.GetFileName(x).StartsWith("_"));

			foreach (var file in targets) {
				var result = Scss.ConvertFileToCss(file, new ScssOptions {
					OutputStyle = ScssOutputStyle.Expanded,
					GenerateSourceMap = true,
					OutputFile = Path.ChangeExtension(file, ".css")
				});

				File.WriteAllText(
					Path.ChangeExtension(file, ".css"),
					result.Css
				);
				if (result.SourceMap != null)
				{
					File.WriteAllText(
						Path.ChangeExtension(file, ".css.map"),
						result.SourceMap
					);
				}
			}
		}
		public void CompileRecursive(string dir)
		{
			this.Compile(Directory.GetFiles(dir, "*.scss"));

			var dirs = Directory.GetDirectories(dir);
			foreach(var _dir in dirs) this.CompileRecursive(_dir);
		}
	}
}
