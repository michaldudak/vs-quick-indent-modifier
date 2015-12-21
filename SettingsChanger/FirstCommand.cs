//------------------------------------------------------------------------------
// <copyright file="FirstCommand.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell.Settings;

namespace SettingsChanger
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class FirstCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int TabCommandId = 0x0100;
        public const int SpaceCommandId = 0x0200;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("aaba3f09-5436-42af-bc60-2dfc390da4c5");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        private readonly DTE dte;

        /// <summary>
        /// Initializes a new instance of the <see cref="FirstCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private FirstCommand(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            this.package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var tabMenuCommandId = new CommandID(CommandSet, TabCommandId);
                var spaceMenuCommandId = new CommandID(CommandSet, SpaceCommandId);
                var tabMenuItem = new OleMenuCommand(TabMenuItemCallback, tabMenuCommandId);
                var spaceMenuItem = new OleMenuCommand(SpaceMenuItemCallback, spaceMenuCommandId);
                tabMenuItem.BeforeQueryStatus += OnTabsBeforeQueryStatus;
                spaceMenuItem.BeforeQueryStatus += OnSpacesBeforeQueryStatus;
                commandService.AddCommand(tabMenuItem);
                commandService.AddCommand(spaceMenuItem);
            }

            dte = (DTE)ServiceProvider.GetService(typeof(DTE));
        }

        private void OnTabsBeforeQueryStatus(object sender, EventArgs e)
        {
            var myCommand = sender as OleMenuCommand;
            if (myCommand == null)
            {
                return;
            }

            myCommand.Text = "TABS";
        }

        private void OnSpacesBeforeQueryStatus(object sender, EventArgs e)
        {
            var myCommand = sender as OleMenuCommand;
            if (myCommand == null)
            {
                return;
            }

            myCommand.Text = "SPACES";
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static FirstCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new FirstCommand(package);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void TabMenuItemCallback(object sender, EventArgs e)
        {
            var props = dte.Properties["TextEditor", "AllLanguages"];
            foreach (var prop in props.Cast<Property>().Where(prop => prop.Name == "InsertTabs"))
            {
                prop.Value = true;
            }
        }

        private void SpaceMenuItemCallback(object sender, EventArgs e)
        {
            var props = dte.Properties["TextEditor", "AllLanguages"];
            foreach (var prop in props.Cast<Property>().Where(prop => prop.Name == "InsertTabs"))
            {
                prop.Value = false;
            }
        }
    }
}
