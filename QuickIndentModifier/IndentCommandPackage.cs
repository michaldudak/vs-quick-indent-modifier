using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace SettingsChanger
{
	[PackageRegistration(UseManagedResourcesOnly = true)]
	[InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
	[ProvideMenuResource("Menus.ctmenu", 1)]
	[Guid(PackageGuidString)]
	public sealed class IndentCommandPackage : Package
	{
		public const string PackageGuidString = "6856dd3c-2a96-4074-8d72-85ed3b6c38cd";

		protected override void Initialize()
		{
			IndentCommand.Initialize(this);
			base.Initialize();
		}
	}
}
