using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Mtf.LanguageService.Windows.Forms
{
    public static class Translator
    {
        /// <summary>
        /// Translates a Windows.Forms.Form and all of its children.
        /// </summary>
        /// <param name="form">The Form to be translated.</param>
        /// <param name="toolTip">The tooltip to be translated.</param>
        /// <returns>Returns the original texts for the controls.</returns>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException if form is null.</exception>
        public static Dictionary<object, string> Translate(Form form, ToolTip toolTip = null)
        {
            if (form == null)
            {
                throw new ArgumentNullException(nameof(form));
            }

            var originalTexts = new Dictionary<object, string>
            {
                { form, form.Text }
            };
            form.Text = Lng.Elem(form.Text);
            var originals = Translate(form.Controls, toolTip);
            foreach (var original in originals)
            {
                if (!originalTexts.ContainsKey(original.Key))
                {
                    originalTexts.Add(original.Key, original.Value);
                }
            }
            return originalTexts;
        }

        /// <summary>
        /// Translates a UserControl and all of its children.
        /// </summary>
        /// <param name="userControl">The custom user control</param>
        /// <param name="toolTip">The tooltip to be translated.</param>
        /// <returns>Returns the original texts for the controls.</returns>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException if userControl is null.</exception>
        public static Dictionary<object, string> Translate(UserControl userControl, ToolTip toolTip = null)
        {
            if (userControl == null)
            {
                throw new ArgumentNullException(nameof(userControl));
            }

            var originalTexts = new Dictionary<object, string>();
            originalTexts.Add(userControl, userControl.Text);
            userControl.Text = Lng.Elem(userControl.Text);
            var originals = Translate(userControl.Controls, toolTip);
            foreach (var original in originals)
            {
                originalTexts.Add(original.Key, original.Value);
            }
            return originalTexts;
        }

        /// <summary>
        /// Translates a ControlCollection.
        /// </summary>
        /// <param name="controls">The ControlCollection to be translated.</param>
        /// <param name="toolTip">The tooltip to be translated.</param>
        /// <returns>Returns the original texts for the controls.</returns>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException if controls is null.</exception>
        public static List<KeyValuePair<object, string>> Translate(Control.ControlCollection controls, ToolTip toolTip = null)
        {
            if (controls == null)
            {
                throw new ArgumentNullException(nameof(controls));
            }

            var result = new List<KeyValuePair<object, string>>();
            foreach (var control in controls)
            {
                if (control is WebBrowser)
                {
                    continue;
                }

                if (control is Control controlWithTextProperty)
                {
                    result.Add(new KeyValuePair<object, string>(controlWithTextProperty, controlWithTextProperty.Text));
                    controlWithTextProperty.Text = Lng.Elem(controlWithTextProperty.Text);
                    var originals = Translate(controlWithTextProperty.Controls, toolTip);
                    foreach (var original in originals)
                    {
                        result.Add(original);
                    }
                    TranslateTooltips(controlWithTextProperty, toolTip);
                    if (controlWithTextProperty.ContextMenuStrip != null)
                    {
                        result.AddRange(Translate(controlWithTextProperty.ContextMenuStrip.Items, toolTip));
                    }
                }

                if (control is ListView listView)
                {
                    foreach (ListViewGroup listViewGroup in listView.Groups)
                    {
                        result.Add(new KeyValuePair<object, string>(listViewGroup, listViewGroup.Header));
                        listViewGroup.Header = Lng.Elem(listViewGroup.Header);
                    }
                    foreach (ColumnHeader column in listView.Columns)
                    {
                        result.Add(new KeyValuePair<object, string>(column, column.Text));
                        column.Text = Lng.Elem(column.Text);
                    }
                }
                else if (control is TreeView treeview)
                {
                    foreach (TreeNode node in treeview.Nodes)
                    {
                        result.AddRange(Translate(node, toolTip));
                    }
                }
                else if (control is MenuStrip menuStrip)
                {
                    result.AddRange(Translate(menuStrip.Items, toolTip));
                }
                else if (control is StatusStrip statusStrip)
                {
                    result.AddRange(Translate(statusStrip.Items, toolTip));
                }
                else if (control is ToolStrip toolStrip)
                {
                    result.AddRange(Translate(toolStrip.Items, toolTip));
                }
                else if (control is ComboBox comboBox)
                {
                    Translate(comboBox);
                }
                else if (control is ContextMenuStrip contextMenuStrip)
                {
                    result.AddRange(Translate(contextMenuStrip.Items, toolTip));
                }
#if NET481_OR_GREATER
                else if (control is ContextMenu contextMenu)
                {
                    result.AddRange(Translate(contextMenu.MenuItems, toolTip));
                }
#endif
                else if (control is DataGridView dataGridView)
                {
                    result.AddRange(Translate(dataGridView, toolTip));
                }
            }

            return result;
        }

        public static void SetOriginalTexts(Dictionary<object, string> originalTexts)
        {
            if (originalTexts == null)
            {
                throw new ArgumentNullException(nameof(originalTexts));
            }

            foreach (var originalText in originalTexts)
            {
                if (originalText.Key is Control control)
                {
                    control.Text = originalText.Value;
                }
                else if(originalText.Key is DataGridViewColumn dataGridViewColumn)
                {
                    dataGridViewColumn.HeaderText = originalText.Value;
                }
                else if (originalText.Key is TreeNode treeNode)
                {
                    treeNode.Text = originalText.Value;
                }
#if NET481_OR_GREATER
                else if (originalText.Key is MenuItem menuItem)
                {
                    menuItem.Text = originalText.Value;
                }
#endif
                else if (originalText.Key is ToolStripItem toolStripItem)
                {
                    toolStripItem.Text = originalText.Value;
                }
                else if (originalText.Key is ToolStripMenuItem toolStripMenuItem)
                {
                    toolStripMenuItem.Text = originalText.Value;
                }
                else if (originalText.Key is ColumnHeader column)
                {
                    column.Text = originalText.Value;
                }
                else if (originalText.Key is ListViewGroup listViewGroup)
                {
                    listViewGroup.Header = originalText.Value;
                }
            }
        }

        public static List<KeyValuePair<object, string>> Translate(DataGridView dataGridView, ToolTip toolTip = null)
        {
            if (dataGridView == null)
            {
                throw new ArgumentNullException(nameof(dataGridView));
            }

            var originalTexts = new List<KeyValuePair<object, string>>();
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                originalTexts.Add(new KeyValuePair<object, string>(column, column.HeaderText));
                column.HeaderText = Lng.Elem(column.HeaderText);
            }
            return originalTexts;
        }

        public static List<KeyValuePair<object, string>> Translate(TreeNode node, ToolTip toolTip = null)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            var originalTexts = new List<KeyValuePair<object, string>>();
            foreach (TreeNode childNode in node.Nodes)
            {
                originalTexts.AddRange(Translate(childNode, toolTip));
            }
            originalTexts.Add(new KeyValuePair<object, string>(node, node.Text));
            node.Text = Lng.Elem(node.Text);
            return originalTexts;
        }

#if NET481_OR_GREATER
        public static List<KeyValuePair<object, string>> Translate(Menu.MenuItemCollection items, ToolTip toolTip)
        {
            var originalTexts = new List<KeyValuePair<object, string>>();
            if (items != null)
            {
                foreach (MenuItem item in items)
                {
                    originalTexts.Add(new KeyValuePair<object, string>(item, item.Text));
                    item.Text = Lng.Elem(item.Text);
                    originalTexts.AddRange(Translate(item.MenuItems, toolTip));
                }
            }
            return originalTexts;
        }
#endif

        public static void Translate(ComboBox comboBox)
        {
            if (comboBox == null)
            {
                throw new ArgumentNullException(nameof(comboBox));
            }

            var items = comboBox.Items;
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var translated = false;
            var result = new List<object>();
            for (var i = 0; i < items.Count; i++)
            {
                if (items[i] is string text)
                {
                    translated = true;
                    result.Add(Lng.Elem(text));
                }
                else
                {
                    result.Add(items[i]);
                }
            }
            if (translated)
            {
                var selectedIndex = comboBox.SelectedIndex;
                comboBox.Items.Clear();
                comboBox.Items.AddRange(result.ToArray());
                comboBox.SelectedIndex = selectedIndex;
            }
        }

        public static List<KeyValuePair<object, string>> Translate(ToolStripItemCollection toolStripItems, ToolTip toolTip = null)
        {
            if (toolStripItems == null)
            {
                throw new ArgumentNullException(nameof(toolStripItems));
            }

            var originalTexts = new List<KeyValuePair<object, string>>();
            foreach (ToolStripItem toolStripItem in toolStripItems)
            {
                originalTexts.Add(new KeyValuePair<object, string>(toolStripItem, toolStripItem.Text));
                toolStripItem.Text = Lng.Elem(toolStripItem.Text);
                TranslateTooltips(toolStripItem);
                if (toolStripItem is ToolStripMenuItem toolStripMenuItem)
                {
                    originalTexts.AddRange(Translate(toolStripMenuItem.DropDownItems, toolTip));
                }
                else if (toolStripItem is ToolStripDropDownButton toolStripDropDownButton)
                {
                    originalTexts.AddRange(Translate(toolStripDropDownButton.DropDownItems, toolTip));
                }
            }
            return originalTexts;
        }

        public static void TranslateTooltips(Control control, ToolTip toolTip)
        {
            var hint = toolTip?.GetToolTip(control);
            if (!String.IsNullOrEmpty(hint))
            {
                toolTip.SetToolTip(control, Lng.Elem(hint));
            }
        }

        public static void TranslateTooltips(ToolStripItem toolStripItem)
        {
            if (toolStripItem is ToolStripButton button)
            {
                var hint = button.ToolTipText;
                if (!String.IsNullOrEmpty(hint))
                {
                    button.ToolTipText = Lng.Elem(hint);
                }
            }
            else if (toolStripItem is ToolStripMenuItem menuItem)
            {
                var hint = menuItem.ToolTipText;
                if (!String.IsNullOrEmpty(hint))
                {
                    menuItem.ToolTipText = Lng.Elem(hint);
                }
            }
            else if (toolStripItem is ToolStripLabel label)
            {
                var hint = label.ToolTipText;
                if (!String.IsNullOrEmpty(hint))
                {
                    label.ToolTipText = Lng.Elem(hint);
                }
            }
        }
    }
}
