using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Windows.Markup;

namespace MenuControl
{
    [TemplatePart(Name="RootMenuBar", Type = typeof(StackPanel))]
    [ContentPropertyAttribute("Items")]
    public class MenuBar : Control
    {

        //add generic click event to handle all the clicks
        public event RoutedEventHandler ItemClick;

        private void OnItemClick(object sender, RoutedEventArgs e)
        {
            if (ItemClick != null)
            {
                ItemClick(sender, e);

                CollapseChildDropDownMenus();//close the menu item when clicked
            }
        }

        //graphic items
        public StackPanel RootMenuBar;

        public MenuBar()
        {            
            this.DefaultStyleKey = typeof(MenuBar);
            this.Loaded += new RoutedEventHandler(MenuBar_Loaded);
            
            if (Items == null)
                Items = new ObservableCollection<MenuBarItem>();                                  
        }
      
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            RootMenuBar = (StackPanel)GetTemplateChild("RootMenuBar");

            foreach (MenuBarItem item in Items)
            {
                item.parentMenu = this;
                //item.Height = Height;
                //add the click logic for the menubar and any nested items so that clicks are
                //handled globally
                item.Click += new RoutedEventHandler(OnItemClick);
                if (item.Items.Count > 0)
                    SetItemClick(item.Items);
            }
        }

        private Point GetMaxItemsHeight()
        {
            double width = 0;
            double height = 0;

            foreach (MenuBarItem item in Items)
            {
                Point xy = new Point(item.Height, item.Width);
                if (xy.X > width) width = xy.X;
                if (xy.Y > height) height = xy.Y;
            }

            return new Point(width, height);
        }

        private void SetItemClick(ObservableCollection<MenuItem> items)
        {
            foreach (MenuItem item in items)
            {
                item.Click += new RoutedEventHandler(OnItemClick);
                if (item.Items.Count > 0)
                    SetItemClick(item.Items);
            }
        }


        void MenuBar_Loaded(object sender, RoutedEventArgs e)
        {
            this.ApplyTemplate();

            foreach (MenuBarItem item in Items)
            {
                RootMenuBar.Children.Add(item);
            }
        }

        public static readonly DependencyProperty itemsProperty = DependencyProperty.Register("Items", typeof(ObservableCollection<MenuBarItem>), typeof(MenuBar), null);

        public ObservableCollection<MenuBarItem> Items
        {
            get { return (ObservableCollection<MenuBarItem>)GetValue(itemsProperty); }
            set
            {
                SetValue(itemsProperty, value);
            }
        }

        internal void CollapseChildDropDownMenus()
        {
            foreach (MenuBarItem item in Items)
            {
                item.CollapseDropDown();
            }
        }
    }
}
