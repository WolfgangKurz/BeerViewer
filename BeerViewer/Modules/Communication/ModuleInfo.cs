namespace BeerViewer.Modules.Communication
{
	public struct ModuleInfo
	{
		public string Name { get; set; }
		public string Template { get; set; }
		public bool Scripted { get; set; }
		public bool Styled { get; set; }
	}
}