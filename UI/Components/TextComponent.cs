using LiveSplit.Model;
using LiveSplit.Model.Comparisons;
using LiveSplit.TimeFormatters;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveSplit.UI.Components
{
    public class TextComponent : IComponent
    {
        protected TextTextComponent InternalComponent { get; set; }
        public TextComponentSettings Settings { get; set; }

        public float PaddingTop { get { return InternalComponent.PaddingTop; } }
        public float PaddingLeft { get { return InternalComponent.PaddingLeft; } }
        public float PaddingBottom { get { return InternalComponent.PaddingBottom; } }
        public float PaddingRight { get { return InternalComponent.PaddingRight; } }

        public IDictionary<string, Action> ContextMenuControls
        {
            get { return null; }
        }

        public TextComponent(LiveSplitState state)
        {
            Settings = new TextComponentSettings()
            {
                CurrentState = state
            };
            InternalComponent = new TextTextComponent(Settings);
        }

        private void PrepareDraw(LiveSplitState state, LayoutMode mode)
        {
            InternalComponent.DisplayTwoRows = Settings.Display2Rows;

            InternalComponent.NameLabel.HasShadow 
                = InternalComponent.ValueLabel.HasShadow
                = state.LayoutSettings.DropShadows;

            if (String.IsNullOrEmpty(Settings.Text1) || String.IsNullOrEmpty(Settings.Text2))
            {
                InternalComponent.NameLabel.HorizontalAlignment = StringAlignment.Center;
                InternalComponent.ValueLabel.HorizontalAlignment = StringAlignment.Center;
                InternalComponent.NameLabel.VerticalAlignment = StringAlignment.Center;
                InternalComponent.ValueLabel.VerticalAlignment = StringAlignment.Center;
            }
            else
            {
                InternalComponent.NameLabel.HorizontalAlignment = StringAlignment.Near;
                InternalComponent.ValueLabel.HorizontalAlignment = StringAlignment.Far;
                InternalComponent.NameLabel.VerticalAlignment = 
                    mode == LayoutMode.Horizontal || Settings.Display2Rows ? StringAlignment.Near : StringAlignment.Center;
                InternalComponent.ValueLabel.VerticalAlignment =
                    mode == LayoutMode.Horizontal || Settings.Display2Rows ? StringAlignment.Far : StringAlignment.Center;
            }

            InternalComponent.NameLabel.ForeColor = Settings.OverrideTextColor ? Settings.TextColor : state.LayoutSettings.TextColor;
            InternalComponent.ValueLabel.ForeColor = Settings.OverrideTimeColor ? Settings.TimeColor : state.LayoutSettings.TextColor;
        }

        private void DrawBackground(Graphics g, LiveSplitState state, float width, float height)
        {
            if (Settings.BackgroundColor.ToArgb() != Color.Transparent.ToArgb()
                || Settings.BackgroundGradient != GradientType.Plain
                && Settings.BackgroundColor2.ToArgb() != Color.Transparent.ToArgb())
            {
                var gradientBrush = new LinearGradientBrush(
                            new PointF(0, 0),
                            Settings.BackgroundGradient == GradientType.Horizontal
                            ? new PointF(width, 0)
                            : new PointF(0, height),
                            Settings.BackgroundColor,
                            Settings.BackgroundGradient == GradientType.Plain
                            ? Settings.BackgroundColor
                            : Settings.BackgroundColor2);
                g.FillRectangle(gradientBrush, 0, 0, width, height);
            }
        }

        public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
        {
            DrawBackground(g, state, width, VerticalHeight);
            PrepareDraw(state, LayoutMode.Vertical);
            InternalComponent.DrawVertical(g, state, width, clipRegion);
        }

        public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
        {
            DrawBackground(g, state, HorizontalWidth, height);
            PrepareDraw(state, LayoutMode.Horizontal);
            InternalComponent.DrawHorizontal(g, state, height, clipRegion);
        }

        public float VerticalHeight
        {
            get { return InternalComponent.VerticalHeight; }
        }

        public float MinimumWidth
        {
            get { return InternalComponent.MinimumWidth; }
        }

        public float HorizontalWidth
        {
            get { return InternalComponent.HorizontalWidth; }
        }

        public float MinimumHeight
        {
            get { return InternalComponent.MinimumHeight; }
        }

        public string ComponentName
        {
            get { return Settings.Text1 + " " + Settings.Text2; }
        }

        public Control GetSettingsControl(LayoutMode mode)
        {
            Settings.Mode = mode;
            return Settings;
        }

        public void SetSettings(System.Xml.XmlNode settings)
        {
            Settings.SetSettings(settings);
        }

        public System.Xml.XmlNode GetSettings(System.Xml.XmlDocument document)
        {
            return Settings.GetSettings(document);
        }

        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            InternalComponent.InformationName = Settings.Text1;
            InternalComponent.InformationValue = Settings.Text2;
            InternalComponent.LongestString = Settings.Text1.Length > Settings.Text2.Length
                ? Settings.Text1
                : Settings.Text2;

            InternalComponent.Update(invalidator, state, width, height, mode);
        }

        public void Dispose()
        {
        }
    }
}
