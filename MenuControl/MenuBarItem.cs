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
using System.Threading;
using System.Windows.Threading;

namespace MenuControl
{
    [TemplatePart(Name = "LayoutRootMenuBarItem", Type = typeof(Grid))]
    [TemplatePart(Name = "ItemBorderMenuBarItem", Type = typeof(Border))]
    [TemplatePart(Name = "ItemTextMBI", Type = typeof(TextBlock))]
    [TemplatePart(Name = "ItemHighlightMenuBarItem", Type = typeof(Grid))]
    [TemplatePart(Name = "ItemHighlightBorderMenuBarItem", Type = typeof(Border))]
    [TemplatePart(Name = "ItemText_copyMBI", Type = typeof(TextBlock))]
    [TemplatePart(Name = "ItemDropDownMenuBarItem", Type = typeof(Canvas))]
    [TemplatePart(Name = "RootItemDropDownMenuBarItem", Type = typeof(Border))]
    [TemplatePart(Name = "RootMenuBar", Type = typeof(Border))]
    [TemplatePart(Name = "ItemHolderMenuBarItem", Type = typeof(StackPanel))]
    
    [TemplateVisualState(Name = "ItemHighlightedMBI", GroupName = "menuNavigationMBI")]
    [TemplateVisualState(Name = "noneHighlightedMBI", GroupName = "menuNavigationMBI")]
    [TemplateVisualState(Name = "ItemSelectedMBI", GroupName = "menuNavigationMBI")]

    [ContentPropertyAttribute("Items")]
    public class MenuBarItem : Control, IMenuItem
    {
        public MenuBar parentMenu;

        //graphic components
        private Grid LayoutRootMenuBarItem;
        private Border ItemBorderMenuBarItem;
        private TextBlock ItemTextMBI;
        private Grid ItemHighlightMenuBarItem;
        private Border ItemHighlightBorderMenuBarItem;
        //private TextBlock ItemText_copyMBI;
        private Canvas ItemDropDownMenuBarItem;
        private Border RootItemDropDownMenuBarItem;
        private StackPanel ItemHolderMenuBarItem;
        private StackPanel RootMenuBar;

        private DispatcherTimer MouseOverTimer;

        public new String Tag { get; set; }

        public bool IsExpanded
        {
            get
            {
                return ItemDropDownMenuBarItem.Visibility == System.Windows.Visibility.Visible;
            }
        }

        public event RoutedEventHandler Click;

        public MenuBarItem()
        {
            this.DefaultStyleKey = typeof(MenuBarItem);
            this.Loaded += new RoutedEventHandler(MenuBarItem_Loaded);

            MouseOverTimer = new DispatcherTimer();

            MouseOverTimer.Tick += (se, ev) =>
            {
                MouseOverTimer.Stop();
                ShowDropDown();
            };

            if (Items == null)
                Items = new ObservableCollection<MenuItem>();
            if (string.IsNullOrEmpty(MenuText))
                MenuText = "Default";

        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            LayoutRootMenuBarItem = (Grid)GetTemplateChild("LayoutRootMenuBarItem");
            ItemBorderMenuBarItem = (Border)GetTemplateChild("ItemBorderMenuBarItem");
            ItemTextMBI = (TextBlock)GetTemplateChild("ItemTextMBI");
            ItemHighlightMenuBarItem = (Grid)GetTemplateChild("ItemHighlightMenuBarItem");
            ItemHighlightBorderMenuBarItem = (Border)GetTemplateChild("ItemHighlightBorderMenuBarItem");
            //ItemText_copyMBI = (TextBlock)GetTemplateChild("ItemText_copyMBI");
            ItemDropDownMenuBarItem = (Canvas)GetTemplateChild("ItemDropDownMenuBarItem");
            RootItemDropDownMenuBarItem = (Border)GetTemplateChild("RootItemDropDownMenuBarItem");
            RootMenuBar = (StackPanel)GetTemplateChild("RootMenuBar");
            ItemHolderMenuBarItem = (StackPanel)GetTemplateChild("ItemHolderMenuBarItem");


            LayoutRootMenuBarItem.MouseEnter += new MouseEventHandler(ItemHighlight_MouseEnter);
            LayoutRootMenuBarItem.MouseLeftButtonDown += new MouseButtonEventHandler(ItemHighlight_MouseLeftButtonDown);
            LayoutRootMenuBarItem.MouseLeave += new MouseEventHandler(ItemHighlight_MouseLeave);
            ItemDropDownMenuBarItem.MouseLeave += new MouseEventHandler(ItemDropDown_MouseLeave);
        }

        void MenuBarItem_Loaded(object sender, RoutedEventArgs e)
        {
            this.ApplyTemplate();

            // change control dimension based on the text
            if (!string.IsNullOrEmpty(MenuText))
            {
                Point xy = getTextWidthHeight(MenuText, ItemTextMBI);
                //ItemTextMBI.Width = xy.X;
                ItemTextMBI.Height = xy.Y;
                ItemTextMBI.Text = MenuText;
                //ItemText_copyMBI.Width = xy.X;
                //ItemText_copyMBI.Height = xy.Y;
                //ItemText_copyMBI.Text = MenuText;

                //set width and height for canvas
                if (LayoutRootMenuBarItem.Width <= xy.X + 20)
                    LayoutRootMenuBarItem.Width = xy.X + 20;
                if (LayoutRootMenuBarItem.Height <= xy.Y + 10)
                    LayoutRootMenuBarItem.Height = xy.Y + 10;

                ItemHighlightMenuBarItem.Width = LayoutRootMenuBarItem.Width;// xy.X + 15;
                ItemHighlightMenuBarItem.Height = LayoutRootMenuBarItem.Height;//xy.Y + 10;
                ItemHighlightBorderMenuBarItem.Width = LayoutRootMenuBarItem.Width;
                ItemHighlightBorderMenuBarItem.Height = LayoutRootMenuBarItem.Height;
                ItemBorderMenuBarItem.Width = LayoutRootMenuBarItem.Width;
                ItemBorderMenuBarItem.Height = LayoutRootMenuBarItem.Height;

                this.Width = LayoutRootMenuBarItem.Width;
                this.Height = LayoutRootMenuBarItem.Height;
                //this.parentMenu.Height = LayoutRootMenuBarItem.Height;
            }

            // add menu items if any exists
            if (Items != null && Items.Count > 0)
            {
                Point xy = getLargest(Items);
                xy.X += 15; //add space for arrow placement

                //set menu holder dimensions
                ItemDropDownMenuBarItem.Width = xy.X + 23;
                RootItemDropDownMenuBarItem.Width = xy.X + 23;
                ItemHolderMenuBarItem.Width = xy.X + 23;

                ItemDropDownMenuBarItem.SetValue(Canvas.TopProperty, Height); //LayoutRootMenuBarItem.Height);

                // height is the height of the textbox plus 10 since we add 10
                // for the box around the text box. Then multiply by count and add 10
                // for margins
                ItemDropDownMenuBarItem.Height = (xy.Y + 10) * (Items.Count) + 8;
                RootItemDropDownMenuBarItem.Height = (xy.Y + 10) * (Items.Count) + 8;
                


                foreach (MenuItem item in Items)
                {
                    //set menuItem dimensions before adding
                    item.setDimension(xy);
                    item.parentMenuBarItem = this;
                    ItemHolderMenuBarItem.Children.Add(item);
                }
            }
        }

        private DependencyObject GetParent(DependencyObject element)
        {
            DependencyObject o = VisualTreeHelper.GetParent(element);
            while (o != null)
            {
                if (VisualTreeHelper.GetParent(o) == null)
                {
                    return o;
                }
                else
                {
                    o = VisualTreeHelper.GetParent(o);
                }
            }
            return null;
        }

        public void CollapseDropDown()
        {
            VisualStateManager.GoToState(this, "noneHighlightedMBI", true);

            foreach (MenuItem item in Items)
            {
                item.CollapseDropDown();
            }
            ItemDropDownMenuBarItem.Visibility = Visibility.Collapsed;
        }

        public void ShowDropDown()
        {
            VisualStateManager.GoToState(this, "ItemSelectedMBI", true);

            ItemDropDownMenuBarItem.Visibility = Visibility.Visible;
        }

        internal void CollapseChildDropDownMenus()
        {
            foreach (MenuItem item in Items)
            {
                item.CollapseDropDown();
            }
        }

        public Point getLargest(ObservableCollection<MenuItem> menuItems)
        {
            double width = 0;
            double height = 0;
            foreach (MenuItem item in menuItems)
            {
                Point xy = getTextWidthHeight(item.MenuText, ItemTextMBI);
                if (xy.X > width) width = xy.X;
                if (xy.Y > height) height = xy.Y;
            }
            return new Point(width, height);
        }


        // return the dimension of the text after aplying styles from given textblock
        private Point getTextWidthHeight(string text, TextBlock itemTextTB)
        {
            TextBlock tb = new TextBlock();
            tb.Text = text;
            tb.Style = itemTextTB.Style;
            tb.FontFamily = itemTextTB.FontFamily;
            tb.FontSize = itemTextTB.FontSize;
            tb.FontSource = itemTextTB.FontSource;
            tb.FontStretch = itemTextTB.FontStretch;
            tb.FontStyle = itemTextTB.FontStyle;
            tb.FontWeight = itemTextTB.FontWeight;
            tb.InvalidateMeasure();

            Point xy = new Point(tb.ActualWidth, tb.ActualHeight);
            return xy;
        }

        //Properties

        public static readonly DependencyProperty itemsProperty = DependencyProperty.Register("Items", typeof(ObservableCollection<MenuItem>), typeof(MenuBarItem), null);

        public ObservableCollection<MenuItem> Items
        {
            get { return (ObservableCollection<MenuItem>)GetValue(itemsProperty); }
            set { SetValue(itemsProperty, value); }
        }

        public static readonly DependencyProperty menuTextProperty = DependencyProperty.Register("MenuText", typeof(string), typeof(MenuBarItem), null);

        public virtual string MenuText
        {
            get { return (string)GetValue(menuTextProperty); }
            set { SetValue(menuTextProperty, value); }
        }

        public static readonly DependencyProperty EnableMouseOverDelayProperty = DependencyProperty.Register("EnableMouseOverDelay", typeof(bool), typeof(MenuBarItem), new PropertyMetadata(true));

        public virtual bool EnableMouseOverDelay
        {
            get { return (bool)GetValue(EnableMouseOverDelayProperty); }
            set { SetValue(EnableMouseOverDelayProperty, value); }
        }


        // Using a DependencyProperty as the backing store for MouseOverDelay.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MouseOverDelayProperty = DependencyProperty.Register("MouseOverDelay", typeof(int), typeof(MenuBarItem), new PropertyMetadata(500));
        public int MouseOverDelay
        {
            get { return (int)GetValue(MouseOverDelayProperty); }
            set { SetValue(MouseOverDelayProperty, value); }
        }

        //END Properties

        //EVENTS


        void ItemDropDown_MouseLeave(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "noneHighlightedMBI", true);
            CollapseDropDown();
        }

        void ItemHighlight_MouseLeave(object sender, MouseEventArgs e)
        {
            if(!IsExpanded)
                VisualStateManager.GoToState(this, "noneHighlightedMBI", true);

            if (MouseOverTimer.IsEnabled)
                MouseOverTimer.Stop();
        }

        void ItemHighlight_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Items != null && Items.Count > 0)
            {
                ShowDropDown();
            }
            else
            {
                //if the top item has no nested childer, provide the click event
                if (Click != null)
                {
                    Click(this, new RoutedEventArgs());
                }
            }
        }

        void ItemHighlight_MouseEnter(object sender, MouseEventArgs e)
        {
            //collapse all open menus
            parentMenu.CollapseChildDropDownMenus();

            VisualStateManager.GoToState(this, "ItemHighlightedMBI", true);

            if (EnableMouseOverDelay && Items != null && Items.Count > 0)
            {
                if (MouseOverTimer.IsEnabled)
                    MouseOverTimer.Stop();

                MouseOverTimer.Interval = new TimeSpan(0, 0, 0, 0, MouseOverDelay);
                MouseOverTimer.Start();
            }
        }

        //END EVENTS



        public void ShowMouseOverDelay()
        {

        }


    }
}
