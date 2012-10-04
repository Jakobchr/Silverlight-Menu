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
using MenuControl;

namespace MenuTester
{
    public partial class Page : UserControl
    {
        public Page()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Page_Loaded);
        }

        void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // an example of how to create the control programatically           
            //MenuBarItem it1 = new MenuBarItem();
            //it1.MenuText = "File";
            //MenuItem f1 = new MenuItem();
            //f1.MenuText = "new item";
            //it1.items.Add(f1);
            //MenuItem f2 = new MenuItem();
            //f2.MenuText = "new item 2";
            //it1.items.Add(f2);
            //MenuItem f3 = new MenuItem();
            //f3.MenuText = "very";
            //it1.items.Add(f3);
            //MenuItem f4 = new MenuItem();
            //f4.MenuText = "largeeeeeeeeee";
            //it1.items.Add(f4);
            //MenuBarItem it2 = new MenuBarItem();
            //it2.MenuText = "Edit";
            //MenuItem a1 = new MenuItem();
            //a1.MenuText = "new item";
            //it2.items.Add(a1);
            //MenuBarItem it3 = new MenuBarItem();
            //it3.MenuText = "Help";
            //MenuItem b1 = new MenuItem();
            //b1.MenuText = "this";
            //it3.items.Add(b1);
            //MenuItem b2 = new MenuItem();
            //b2.MenuText = "that";
            //it3.items.Add(b2);
            //MenuItem b3 = new MenuItem();
            //b3.MenuText = "that and that this";
            //it3.items.Add(b3);
            //MenuItem b4 = new MenuItem();
            //b4.MenuText = "crappy do little do";
            //it3.items.Add(b4);
            //MenuItem b5 = new MenuItem();
            //b5.MenuText = "thatssd ";
            //it3.items.Add(b5);

            //MenuBar bar = new MenuBar();
            //bar.items.Add(it1);
            //bar.items.Add(it2);
            //bar.items.Add(it3);
           
            //LRoot.Children.Add(bar);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Browser.HtmlPage.Window.Alert("you pressed: " + (sender as MenuItem).MenuText);

        }
        
    }
}
