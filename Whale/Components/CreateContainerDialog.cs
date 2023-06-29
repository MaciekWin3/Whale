using Terminal.Gui;

namespace Whale.Components
{
    class CreateContainerDialog
    {
        public Dialog Create()
        {
            var dialog = new Dialog("Create Container", 60, 20);
            var label = new Label(1, 1, "Container Name:");
            var textField = new TextField(1, 2, 40, "");
            var okButton = new Button(1, 4, "OK");
            var cancelButton = new Button(10, 4, "Cancel");
            dialog.Add(label, textField, okButton, cancelButton);
            return dialog;
        }
    }
}

