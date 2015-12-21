using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using System.Linq;
using EnvDTE;

namespace SettingsChanger
{
	internal sealed class IndentCommand
	{
		private const int UseTabsCommandId = 0x0100;
		private const int UseSpacesCommandId = 0x0200;
		private static readonly Guid CommandSet = new Guid("aaba3f09-5436-42af-bc60-2dfc390da4c5");

		private readonly Package _package;
		private readonly DTE _dte;

		/// <summary>
		/// Initializes a new instance of the <see cref="IndentCommand"/> class.
		/// Adds our command handlers for menu (commands must exist in the command table file)
		/// </summary>
		/// <param name="package">Owner package, not null.</param>
		private IndentCommand(Package package)
		{
			if (package == null)
			{
				throw new ArgumentNullException(nameof(package));
			}

			_package = package;
			_dte = (DTE)ServiceProvider.GetService(typeof(DTE));

			var commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
			if (commandService == null)
			{
				throw new InvalidOperationException($"{nameof(IMenuCommandService)} could not be resolved.");
			}

			var tabMenuCommandId = new CommandID(CommandSet, UseTabsCommandId);
			var spaceMenuCommandId = new CommandID(CommandSet, UseSpacesCommandId);
			var tabMenuItem = new MenuCommand(TabMenuItemCallback, tabMenuCommandId);
			var spaceMenuItem = new MenuCommand(SpaceMenuItemCallback, spaceMenuCommandId);

			commandService.AddCommand(tabMenuItem);
			commandService.AddCommand(spaceMenuItem);
		}

		/// <summary>
		/// Gets the instance of the command.
		/// </summary>
		public static IndentCommand Instance { get; private set; }

		/// <summary>
		/// Gets the service provider from the owner package.
		/// </summary>
		private IServiceProvider ServiceProvider => _package;

		/// <summary>
		/// Initializes the singleton instance of the command.
		/// </summary>
		/// <param name="package">Owner package, not null.</param>
		public static void Initialize(Package package)
		{
			Instance = new IndentCommand(package);
		}

		private void ChangeTabSetting(bool useTabs)
		{
			var props = _dte.Properties["TextEditor", "AllLanguages"];
			foreach (var prop in props.Cast<Property>().Where(prop => prop.Name == "InsertTabs"))
			{
				prop.Value = useTabs;
			}
		}

		private void TabMenuItemCallback(object sender, EventArgs e) => ChangeTabSetting(true);
		
		private void SpaceMenuItemCallback(object sender, EventArgs e) => ChangeTabSetting(false);
	}
}
