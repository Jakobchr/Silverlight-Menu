﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Windows.Markup;
using System.Windows.Threading;


namespace MenuControl
{

    [TemplatePart(Name = "LayoutRootMenuItem", Type = typeof(Grid))]
    [TemplatePart(Name = "ItemTextMI", Type = typeof(TextBlock))]
    [TemplatePart(Name = "ItemHighlightMenuItem", Type = typeof(Canvas))]
    [TemplatePart(Name = "ItemHighlightBorderMenuItem", Type = typeof(Rectangle))]
    //[TemplatePart(Name = "ItemText_copyMI", Type = typeof(TextBlock))]
    [TemplateVisualState(Name = "ItemHighlightedMI", GroupName = "menuNavigationMI")]
    [TemplateVisualState(Name = "noneHighlightedMI", GroupName = "menuNavigationMI")]

    [ContentPropertyAttribute("Items")]
    public class MenuItem : Control, IMenuItem
    {
        //graphic elements
        public Grid RootMenuItem;
        public Grid LayoutRootMenuItem;
        public TextBlock ItemTextMI;
        public Grid ItemHighlightMenuItem;
        public Border ItemHighlightBorderMenuItem;
        //public TextBlock ItemText_copyMI;
        public Path arrow;
        //public Path arrowHighlight;
        public Canvas ItemDropDownMenuItem;
        public Border baseRectMI;
        public StackPanel ItemHolderMenuItem;
        public Canvas SubMenuDropDown;
        private bool isNested;

        public Point xy;

        public MenuItem parentMenuItem;
        public MenuBarItem parentMenuBarItem;

        public new String Tag { get; set; }

        private DispatcherTimer MouseOverTimer;

        public event RoutedEventHandler Click;


        public MenuItem()
        {
            this.DefaultStyleKey = typeof(MenuItem);
            this.Loaded += new RoutedEventHandler(MenuItem_Loaded);
            if (Items == null)
                Items = new ObservableCollection<MenuItem>();
            if (string.IsNullOrEmpty(MenuText))
                MenuText = "Default";

            MouseOverTimer = new DispatcherTimer();
            MouseOverTimer.Tick += (se, ev) =>
            {
                MouseOverTimer.Stop();
                ShowDropDown();
            };

            isNested = false;

            parentMenuBarItem = null;
            parentMenuItem = null;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            RootMenuItem = (Grid)GetTemplateChild("RootMenuItem");
            LayoutRootMenuItem = (Grid)GetTemplateChild("LayoutRootMenuItem");
            ItemTextMI = (TextBlock)GetTemplateChild("ItemTextMI");
            ItemHighlightMenuItem = (Grid)GetTemplateChild("ItemHighlightMenuItem");
            ItemHighlightBorderMenuItem = (Border)GetTemplateChild("ItemHighlightBorderMenuItem");
            //ItemText_copyMI = (TextBlock)GetTemplateChild("ItemText_copyMI");
            arrow = (Path)GetTemplateChild("arrow");
            //arrowHighlight = (Path)GetTemplateChild("arrowHighlight");
            ItemDropDownMenuItem = (Canvas)GetTemplateChild("ItemDropDownMenuItem");
            SubMenuDropDown = (Canvas)GetTemplateChild("SubMenuDropDown");
            baseRectMI = (Border)GetTemplateChild("baseRectMI");
            ItemHolderMenuItem = (StackPanel)GetTemplateChild("ItemHolderMenuItem");

            _Icon = (Image)GetTemplateChild("ElementIcon");

            if (this._Icon != null && this.Icon != null)
                _Icon.Source = Icon;

            ItemHighlightMenuItem.MouseEnter += new MouseEventHandler(ItemHighlight_MouseEnter);
            ItemHighlightMenuItem.MouseLeave += new MouseEventHandler(ItemHighlight_MouseLeave);
            ItemHighlightMenuItem.MouseLeftButtonDown += new MouseButtonEventHandler(ItemHighlight_MouseLeftButtonDown);
            ItemDropDownMenuItem.MouseLeave += new MouseEventHandler(ItemDropDownMenuItem_MouseLeave);

            // set dimensions
            ItemTextMI.Width = xy.X;
            ItemTextMI.Height = xy.Y;

            //ItemText_copyMI.Width = xy.X;
            //ItemText_copyMI.Height = xy.Y;


            //set width and height for canvas
            RootMenuItem.Width = xy.X + 15;
            RootMenuItem.Height = xy.Y + 10;
            ItemHighlightMenuItem.Width = xy.X + 15;
            ItemHighlightMenuItem.Height = xy.Y + 10;
            ItemHighlightBorderMenuItem.Width = xy.X + 15;
            ItemHighlightBorderMenuItem.Height = xy.Y + 10;

        }

        void MenuItem_Loaded(object sender, RoutedEventArgs e)
        {
            this.ApplyTemplate();

            if (!string.IsNullOrEmpty(MenuText))
            {
                ItemTextMI.Text = MenuText;
                //ItemText_copyMI.Text = MenuText;
            }

            // add menu Items if any exists
            if (Items != null && Items.Count > 0)
            {
                isNested = true;
                arrow.Visibility = Visibility.Visible;
                //arrowHighlight.Visibility = Visibility.Visible;

                Point xy = getLargest(Items);
                xy.X += 15; //add space for arrow placement

                //arrow.SetValue(Canvas.LeftProperty, this.xy.X + 12);
                //arrowHighlight.SetValue(Canvas.LeftProperty, this.xy.X + 12);

                //set menu holder dimensions
                ItemDropDownMenuItem.Width = xy.X + 23;
                baseRectMI.Width = xy.X + 23;

                //ItemDropDownMenuItem.SetValue(Canvas.TopProperty, RootMenuItem.Height);
                ItemDropDownMenuItem.SetValue(Canvas.LeftProperty, RootMenuItem.Width + 5);

                // height is the height of the textbox plus 10 since we add 10
                //for the box around the text box. Then multiply by count and add 10
                // for margins
                ItemDropDownMenuItem.Height = (xy.Y + 10) * (Items.Count) + 8;
                baseRectMI.Height = (xy.Y + 10) * (Items.Count) + 8;


                foreach (MenuItem item in Items)
                {
                    //set menuItem dimensions before adding
                    item.setDimension(xy);
                    item.parentMenuItem = this;
                    ItemHolderMenuItem.Children.Add(item);
                }
            }
        }


        internal void setDimension(Point xy)
        {
            this.xy = xy;
        }

        public void CollapseDropDown()
        {
            if (isNested)
            {
                foreach (MenuItem item in Items)
                {
                    item.CollapseDropDown();
                }
            }
            ItemDropDownMenuItem.Visibility = Visibility.Collapsed;
        }

        public void ShowDropDown()
        {
            ItemDropDownMenuItem.Visibility = Visibility.Visible;
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
                Point xy = getTextWidthHeight(item.MenuText, ItemTextMI);
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

            Point xy = new Point(tb.ActualWidth, tb.ActualHeight);
            return xy;
        }

        //Properties

        public static readonly DependencyProperty itemsProperty = DependencyProperty.Register("Items", typeof(ObservableCollection<MenuItem>), typeof(MenuItem), null);

        public ObservableCollection<MenuItem> Items
        {
            get { return (ObservableCollection<MenuItem>)GetValue(itemsProperty); }
            set { SetValue(itemsProperty, value); }
        }

        public static readonly DependencyProperty menuTextProperty = DependencyProperty.Register("MenuText", typeof(string), typeof(MenuItem), null);

        public virtual string MenuText
        {
            get { return (string)GetValue(menuTextProperty); }
            set { SetValue(menuTextProperty, value); }
        }

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(ImageSource), typeof(MenuItem), new PropertyMetadata(null, new PropertyChangedCallback(OnIconPropertyChanged)));

        private Image _Icon;

        public virtual ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
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

        private static void OnIconPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MenuItem)d).OnIconPropertyChanged((ImageSource)e.NewValue);
        }

        private void OnIconPropertyChanged(ImageSource newValue)
        {
            if (newValue != null && this._Icon != null)
            {
                _Icon.Source = newValue;
            }
        }

        //EVENTS

        private void ItemHighlight_MouseEnter(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "ItemHighlightedMI", true);
            //if it's a root level menuItem collapse all other drop downs
            if (parentMenuBarItem != null)
            {
                parentMenuBarItem.CollapseChildDropDownMenus();
            }
            else if (parentMenuItem != null)
            {
                parentMenuItem.CollapseChildDropDownMenus();
            }

            if (EnableMouseOverDelay && isNested)
            {
                if (MouseOverTimer.IsEnabled)
                    MouseOverTimer.Stop();

                MouseOverTimer.Interval = new TimeSpan(0, 0, 0, 0, MouseOverDelay);
                MouseOverTimer.Start();
            }
        }

        private void ItemHighlight_MouseLeave(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "noneHighlightedMI", true);

            if (MouseOverTimer.IsEnabled)
                MouseOverTimer.Stop();
        }

        void ItemDropDownMenuItem_MouseLeave(object sender, MouseEventArgs e)
        {
            CollapseDropDown();
        }

        private void ItemHighlight_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (isNested)
            {
                ShowNestedDropDown();
            }
            else
            {
                if (Click != null)
                {
                    Click(this, new RoutedEventArgs());
                }
            }
        }

        private void ShowNestedDropDown()
        {
            //check if the sub menu will be visible to show by default, if not show on the left
            double wholeWidth = Application.Current.Host.Content.ActualWidth;
            double actualWidth = this.ActualWidth;
            Point p = this.TransformFromRootVisual();

            if ((p.X + this.ActualWidth + ItemDropDownMenuItem.Width) > wholeWidth)
                Canvas.SetLeft(ItemDropDownMenuItem, -(ItemDropDownMenuItem.Width));

            ShowDropDown();
        }

        //END EVENTS


    }


    public static class TransformExtention
    {
        private static readonly Point _zero = new Point(0, 0);
        public static Point TransformFromRootVisual(this UIElement element)
        {
            try
            {
                MatrixTransform globalTransform = (MatrixTransform)element.TransformToVisual(null);
                return globalTransform.Matrix.Transform(_zero);
            }
            catch { }
            return _zero;
        }
    }
}
