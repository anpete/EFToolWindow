using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.EFToolWindow
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration(productName: "#110", productDetails: "#112", productId: "1.0", IconResourceID = 400)]
    [ProvideMenuResource(resourceID: "Menus.ctmenu", version: 1)]
    [ProvideToolWindow(typeof(EFToolWindowPane))]
    [Guid(Constants.Package)]
    public sealed class EFToolWindowPackage : Package
    {
        private EFToolWindowPane _toolWindowPane;

        protected override void Initialize()
        {
            base.Initialize();

            var oleMenuCommandService = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;

            if (oleMenuCommandService != null)
            {
                var commandId = new CommandID(
                    new Guid(Constants.CommandSetGuid),
                    (int)Constants.MenuItemId);

                oleMenuCommandService.AddCommand(new MenuCommand(ShowToolWindow, commandId));
            }

            if (_toolWindowPane == null)
            {
                _toolWindowPane = (EFToolWindowPane)FindToolWindow(typeof(EFToolWindowPane), id: 0, create: true);
            }
        }

        private void ShowToolWindow(object sender, EventArgs e)
        {
            _toolWindowPane =
                (EFToolWindowPane)FindToolWindow(typeof(EFToolWindowPane), id: 0, create: true);

            if (_toolWindowPane?.Frame == null)
            {
                throw new NotSupportedException(Resources.CannotCreateWindow);
            }

            var windowFrame = (IVsWindowFrame)_toolWindowPane.Frame;

            ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }
    }
}