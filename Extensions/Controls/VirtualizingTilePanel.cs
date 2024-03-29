﻿using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Extensions
{
    public class VirtualizingTilePanel : VirtualizingPanel, IScrollInfo
    {
        /// <summary>
        /// Controls the size of the child elements.
        /// </summary>
        public static readonly DependencyProperty ChildHeightProperty
           = DependencyProperty.RegisterAttached("ChildHeight", typeof(double), typeof(VirtualizingTilePanel),
              new FrameworkPropertyMetadata(200.0d, FrameworkPropertyMetadataOptions.AffectsMeasure |
              FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <summary>
        /// Controls the number of the child elements in a row.
        /// </summary>
        public static readonly DependencyProperty ColumnsProperty
           = DependencyProperty.RegisterAttached("Columns", typeof(int), typeof(VirtualizingTilePanel),
              new FrameworkPropertyMetadata(10, FrameworkPropertyMetadataOptions.AffectsMeasure |
              FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <summary>
        /// Controls the size of the child elements.
        /// </summary>
        public static readonly DependencyProperty MinChildWidthProperty
           = DependencyProperty.RegisterAttached("MinChildWidth", typeof(double), typeof(VirtualizingTilePanel),
              new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure |
              FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <summary>
        /// If setting is true, the component will calulcate the number
        /// of children per row, the width of each item is set equal
        /// to the height. In this mode the columns property is ignored.
        /// If the setting is false, the component will calulate the
        /// width of each item by dividing the available size by the
        /// number of desired rows.
        /// </summary>
        public static readonly DependencyProperty TileProperty
           = DependencyProperty.RegisterAttached("Tile", typeof(bool), typeof(VirtualizingTilePanel),
              new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsMeasure |
              FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public VirtualizingTilePanel()
        {
            // For use in the IScrollInfo implementation
            RenderTransform = _trans;
        }

        /// <summary>
        /// Gets or sets the height of each child.
        /// </summary>
        public double ChildHeight
        {
            get => (double)GetValue(ChildHeightProperty);
            set => SetValue(ChildHeightProperty, value);
        }

        /// <summary>
        /// Gets or sets the number of desired columns.
        /// </summary>
        public int Columns
        {
            get => (int)GetValue(ColumnsProperty);
            set => SetValue(ColumnsProperty, value);
        }

        /// <summary>
        /// Gets or sets the minimum width of each child.
        /// </summary>
        public double MinChildWidth
        {
            get => (double)GetValue(MinChildWidthProperty);
            set => SetValue(MinChildWidthProperty, value);
        }

        /// <summary>
        /// Gets or sets whether the component is operating
        /// in tile mode.If set to true, the component
        /// will calulcate the number of children per row,
        /// the width of each item is set equal to the height.
        /// In this mode the Columns property is ignored. If the
        /// setting is false, the component will calulate the
        /// width of each item by dividing the available size
        /// by the number of desired columns.
        /// </summary>
        public bool Tile
        {
            get => (bool)GetValue(TileProperty);
            set => SetValue(TileProperty, value);
        }

        /// <summary>
        /// Arrange the children
        /// </summary>
        /// <param name="finalSize">Size available</param>
        /// <returns>Size used</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            IItemContainerGenerator generator = ItemContainerGenerator;

            UpdateScrollInfo(finalSize);

            for (int i = 0; i < Children.Count; i++)
            {
                UIElement child = Children[i];

                // Map the child offset to an item offset
                int itemIndex = generator.IndexFromGeneratorPosition(new GeneratorPosition(i, 0));

                ArrangeChild(itemIndex, child, finalSize);
            }

            return finalSize;
        }

        /// <summary>
        /// Measure the children
        /// </summary>
        /// <param name="availableSize">Size available</param>
        /// <returns>Size desired</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            UpdateScrollInfo(availableSize);

            // Figure out range that's visible based on layout algorithm
            GetVisibleRange(out int firstVisibleItemIndex, out int lastVisibleItemIndex);

            // We need to access InternalChildren before the generator to work around a bug
            UIElementCollection children = InternalChildren;
            IItemContainerGenerator generator = ItemContainerGenerator;

            // Get the generator position of the first visible data item
            GeneratorPosition startPos = generator.GeneratorPositionFromIndex(firstVisibleItemIndex);

            // Get index where we'd insert the child for this position. If the item is realized
            // (position.Offset == 0), it's just position.Index, otherwise we have to add one to
            // insert after the corresponding child
            int childIndex = (startPos.Offset == 0) ? startPos.Index : startPos.Index + 1;

            using (generator.StartAt(startPos, GeneratorDirection.Forward, true))
            {
                for (int itemIndex = firstVisibleItemIndex; itemIndex <= lastVisibleItemIndex; ++itemIndex, ++childIndex)
                {
                    // Get or create the child
                    UIElement child = generator.GenerateNext(out bool newlyRealized) as UIElement;
                    if (newlyRealized)
                    {
                        // Figure out if we need to insert the child at the end or somewhere in the middle
                        if (childIndex >= children.Count)
                        {
                            AddInternalChild(child);
                        }
                        else
                        {
                            InsertInternalChild(childIndex, child);
                        }
                        generator.PrepareItemContainer(child);
                    }
                    else
                    {
                        // The child has already been created, let's be sure it's in the right spot
                        Debug.Assert(child == children[childIndex], "Wrong child was generated");
                    }

                    // Measurements will depend on layout algorithm
                    child.Measure(GetChildSize(availableSize));
                }
            }

            // Note: this could be deferred to idle time for efficiency
            CleanUpItems(firstVisibleItemIndex, lastVisibleItemIndex);

            return availableSize;
        }

        /// <summary>
        /// When items are removed, remove the corresponding UI if necessary
        /// </summary>
        /// <param name="sender">System.Object repersenting the source of the event.</param>
        /// <param name="args">The arguments for the event.</param>
        protected override void OnItemsChanged(object sender, ItemsChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Reset:

                    //Peform layout refreshment.
                    ScrollOwner?.ScrollToTop();
                    break;

                case NotifyCollectionChangedAction.Move:
                    RemoveInternalChildRange(args.Position.Index, args.ItemUICount);
                    break;
            }
        }

        /// <summary>
        /// Revirtualize items that are no longer visible
        /// </summary>
        /// <param name="minDesiredGenerated">first item index that should be visible</param>
        /// <param name="maxDesiredGenerated">last item index that should be visible</param>
        private void CleanUpItems(int minDesiredGenerated, int maxDesiredGenerated)
        {
            UIElementCollection children = InternalChildren;
            IItemContainerGenerator generator = ItemContainerGenerator;

            for (int i = children.Count - 1; i >= 0; i--)
            {
                GeneratorPosition childGeneratorPos = new(i, 0);
                int itemIndex = generator.IndexFromGeneratorPosition(childGeneratorPos);
                if (itemIndex < minDesiredGenerated || itemIndex > maxDesiredGenerated)
                {
                    generator.Remove(childGeneratorPos, 1);
                    RemoveInternalChildRange(i, 1);
                }
            }
        }

        #region Layout specific code

        /*I've isolated the layout specific code to this region. If you want
        to do something other than tiling, this is where you'll make your changes*/

        /// <summary>
        /// Position a child
        /// </summary>
        /// <param name="itemIndex">The data item index of the child</param>
        /// <param name="child">The element to position</param>
        /// <param name="finalSize">The size of the panel</param>
        private void ArrangeChild(int itemIndex, UIElement child, Size finalSize)
        {
            if (Tile)
            {
                int childrenPerRow = CalculateChildrenPerRow(finalSize);

                double childWidth = finalSize.Width / childrenPerRow;

                int row = itemIndex / childrenPerRow;
                int column = itemIndex % childrenPerRow;

                child.Arrange(new Rect(column * childWidth, row * ChildHeight,
                    childWidth, ChildHeight));
            }
            else
            {
                //Get the width of each child.
                double childWidth = CalculateChildWidth(finalSize);

                int _row = itemIndex / Columns;
                int _column = itemIndex % Columns;

                child.Arrange(new Rect(_column * childWidth, _row * ChildHeight,
                    childWidth, ChildHeight));
            }
        }

        /// <summary>
        /// Helper function for tiling layout
        /// </summary>
        /// <param name="availableSize">Size available</param>
        /// <returns>The number of tiles on each row.</returns>
        private int CalculateChildrenPerRow(Size availableSize)
        {
            // Figure out how many children fit on each row
            int childrenPerRow = availableSize.Width == double.PositiveInfinity
                ? Children.Count
                : Math.Max(1, (int)Math.Floor(availableSize.Width / (MinChildWidth > 0 ? MinChildWidth : ChildHeight)));
            return childrenPerRow;
        }

        /// <summary>
        /// Calculate the width of each tile by
        /// dividing the width of available size
        /// by the number of required columns.
        /// </summary>
        /// <param name="availableSize">The total layout size available.</param>
        /// <returns>The width of each tile.</returns>
        private double CalculateChildWidth(Size availableSize)
        {
            return availableSize.Width / Columns;
        }

        /// <summary>
        /// Calculate the extent of the view based on the available size
        /// </summary>
        /// <param name="availableSize">available size</param>
        /// <param name="itemCount">number of data items</param>
        /// <returns>Returns the extent size of the viewer.</returns>
        private Size CalculateExtent(Size availableSize, int itemCount)
        {
            //If tile mode.
            if (Tile)
            {
                //Gets the number of children or items for each row.
                int childrenPerRow = CalculateChildrenPerRow(availableSize);

                // See how big we are
                return new Size(childrenPerRow * (MinChildWidth > 0 ? MinChildWidth : ChildHeight),
                    ChildHeight * Math.Ceiling((double)itemCount / childrenPerRow));
            }
            else
            {
                //Gets the width of each child.
                double childWidth = CalculateChildWidth(availableSize);

                // See how big we are
                return new Size(Columns * childWidth,
                    ChildHeight * Math.Ceiling((double)itemCount / Columns));
            }
        }

        /// <summary>
        /// Get the size of the each child.
        /// </summary>
        /// <returns>The size of each child.</returns>
        private Size GetChildSize(Size availableSize)
        {
            if (Tile)
            {
                //Gets the number of children or items for each row.
                int childrenPerRow = CalculateChildrenPerRow(availableSize);
                return new Size(availableSize.Width / childrenPerRow, ChildHeight);
            }
            else
            {
                return new Size(CalculateChildWidth(availableSize), ChildHeight);
            }
        }

        /// <summary>
        /// Get the range of children that are visible
        /// </summary>
        /// <param name="firstVisibleItemIndex">The item index of the first visible item</param>
        /// <param name="lastVisibleItemIndex">The item index of the last visible item</param>
        private void GetVisibleRange(out int firstVisibleItemIndex, out int lastVisibleItemIndex)
        {
            //If tile mode.
            if (Tile)
            {
                //Get the number of children
                int childrenPerRow = CalculateChildrenPerRow(_extent);

                firstVisibleItemIndex = (int)Math.Floor(_offset.Y / ChildHeight) * childrenPerRow;
                lastVisibleItemIndex = ((int)Math.Ceiling((_offset.Y + _viewport.Height) / ChildHeight) * childrenPerRow) - 1;

                ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);
                int itemCount = itemsControl.HasItems ? itemsControl.Items.Count : 0;
                if (lastVisibleItemIndex >= itemCount)
                {
                    lastVisibleItemIndex = itemCount - 1;
                }
            }
            else
            {
                firstVisibleItemIndex = (int)Math.Floor(_offset.Y / ChildHeight) * Columns;
                lastVisibleItemIndex = ((int)Math.Ceiling((_offset.Y + _viewport.Height) / ChildHeight) * Columns) - 1;

                ItemsControl _itemsControl = ItemsControl.GetItemsOwner(this);
                int _itemCount = _itemsControl.HasItems ? _itemsControl.Items.Count : 0;
                if (lastVisibleItemIndex >= _itemCount)
                {
                    lastVisibleItemIndex = _itemCount - 1;
                }
            }
        }

        #endregion Layout specific code

        #region IScrollInfo implementation

        /// <summary>
        ///  See Ben Constable's series of posts at http://blogs.msdn.com/bencon/
        /// </summary>
        /// <param name="availableSize"></param>
        private void UpdateScrollInfo(Size availableSize)
        {
            //Initialize items control.
            ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);

            //See how many items there are
            int itemCount = itemsControl.HasItems ? itemsControl.Items.Count : 0;

            //Get the total size, visible and invisible.
            Size extent = CalculateExtent(availableSize, itemCount);

            // Update extent
            if (extent != _extent)
            {
                //Store in class scope.
                _extent = extent;

                //Peform layout refreshment.
                ScrollOwner?.InvalidateScrollInfo();
            }

            // Update viewport
            if (availableSize != _viewport)
            {
                //Store in class scope.
                _viewport = availableSize;

                //Perform layout refreshment.
                ScrollOwner?.InvalidateScrollInfo();
            }
        }

        #region Properties

        /// <summary>
        /// Gets or sets whether the viewer can
        /// scroll content horizontally.
        /// </summary>
        public bool CanHorizontallyScroll { get; set; }

        /// <summary>
        /// Gets or sets whether the viewer can
        /// scroll content vertically.
        /// </summary>
        public bool CanVerticallyScroll { get; set; }

        /// <summary>
        /// Gets the total height, visible
        /// and invisible.
        /// </summary>
        public double ExtentHeight => _extent.Height;

        /// <summary>
        /// Gets the total width, visible
        /// and invisible.
        /// </summary>
        public double ExtentWidth => _extent.Width;

        /// <summary>
        /// Gets the horizontal offset value.
        /// </summary>
        public double HorizontalOffset => _offset.X;

        public ScrollViewer ScrollOwner { get; set; }

        /// <summary>
        /// Gets the vertical offset value.
        /// </summary>
        public double VerticalOffset => _offset.Y;

        /// <summary>
        /// Gets the height of the viewable area.
        /// </summary>
        public double ViewportHeight => _viewport.Height;

        /// <summary>
        /// Gets the width of the viewable area.
        /// </summary>
        public double ViewportWidth => _viewport.Width;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Scroll the content down by one line.
        /// </summary>
        public void LineDown()
        {
            SetVerticalOffset(VerticalOffset + 10);
        }

        /// <summary>
        /// Scroll the content left by 1 line.
        /// This method is not implemented and
        /// will throw an exception if called.
        /// </summary>
        public void LineLeft()
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Scroll the content right by 1 line.
        /// This method is not implemented and
        /// will throw an exception if called.
        /// </summary>
        public void LineRight()
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Scroll the content up by one line.
        /// </summary>
        public void LineUp()
        {
            SetVerticalOffset(VerticalOffset - 10);
        }

        public Rect MakeVisible(Visual visual, Rect rectangle)
        {
            return new Rect();
        }

        /// <summary>
        /// Scroll the content down by 10 pixels.
        /// </summary>
        public void MouseWheelDown()
        {
            SetVerticalOffset(VerticalOffset + 10);
        }

        /// <summary>
        /// Scroll the content left by 10 pixels.
        /// This method is not implemented and
        /// will throw an exception if called.
        /// </summary>
        public void MouseWheelLeft()
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Scroll the content right by 10 pixels.
        /// This method is not implemented and
        /// will throw an exception if called.
        /// </summary>
        public void MouseWheelRight()
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Scroll the content up by 10 pixels.
        /// </summary>
        public void MouseWheelUp()
        {
            SetVerticalOffset(VerticalOffset - 10);
        }

        /// <summary>
        /// Scroll the content down one viewable partition.
        /// </summary>
        public void PageDown()
        {
            SetVerticalOffset(VerticalOffset + _viewport.Height);
        }

        /// <summary>
        /// Scroll the content left by 1 viewable.
        /// partition. This method is not implemented
        /// and will throw an exception if called.
        /// </summary>
        public void PageLeft()
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Scroll the content right by 1 viewable.
        /// partition. This method is not implemented
        /// and will throw an exception if called.
        /// </summary>
        public void PageRight()
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Scroll the content up one viewable partition.
        /// </summary>
        public void PageUp()
        {
            SetVerticalOffset(VerticalOffset - _viewport.Height);
        }

        /// <summary>
        /// Set the horizontal offset value of the viewer.
        /// This method is not implemented and will throw
        /// and exception if called.
        /// </summary>
        /// <param name="offset">The new horizontal offset value.</param>
        public void SetHorizontalOffset(double offset)
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Set the vertical offset value of the viewer.
        /// This method is not implemented and will throw
        /// and exception if called.
        /// </summary>
        /// <param name="offset">The new vertical offset value.</param>
        public void SetVerticalOffset(double offset)
        {
            if (offset < 0 || _viewport.Height >= _extent.Height)
            {
                offset = 0;
            }
            else
            {
                if (offset + _viewport.Height >= _extent.Height)
                {
                    offset = _extent.Height - _viewport.Height;
                }
            }

            _offset.Y = offset;

            ScrollOwner?.InvalidateScrollInfo();

            _trans.Y = -offset;

            // Force us to realize the correct children
            InvalidateMeasure();
        }

        protected override void BringIndexIntoView(int index)
        {
            SetVerticalOffset(GetOffsetForFirstVisibleIndex(index).Height);
        }

        private Size GetOffsetForFirstVisibleIndex(int index)
        {
            return new Size(_offset.X, index * ChildHeight / CalculateChildrenPerRow(_extent));
        }

        #endregion Methods

        #region Fields

        private readonly TranslateTransform _trans = new();

        private Size _extent = new(0, 0);

        private Point _offset;

        private Size _viewport = new(0, 0);

        #endregion Fields

        #endregion IScrollInfo implementation
    }
}