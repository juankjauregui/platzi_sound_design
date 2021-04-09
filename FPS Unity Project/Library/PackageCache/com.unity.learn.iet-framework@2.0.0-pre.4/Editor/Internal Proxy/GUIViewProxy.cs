using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Unity.Tutorials.Core.Editor
{
    /// <summary>
    /// Proxy class for accessing UnityEditor.GUIView.
    /// </summary>
    /// <remarks>
    /// Not a pure proxy class, contains some custom functionality.
    /// </remarks>
    public class GUIViewProxy
    {
        /// <summary>
        /// Raised then the position of the underlying GUIView changes.
        /// </summary>
        public static event Action<UnityObject> PositionChanged;

        /// <summary>
        /// Type of internal class GUIView.
        /// </summary>
        public static Type GuiViewType => typeof(GUIView);

        /// <summary>
        /// Type of internal class TooltipView.
        /// </summary>
        public static Type TooltipViewType => typeof(TooltipView);

        /// <summary>
        /// Type of internal class Toolbar.
        /// </summary>
        public static Type ToolbarType => typeof(Toolbar);

        static GUIViewProxy()
        {
            GUIView.positionChanged += (guiView) => PositionChanged?.Invoke(guiView);
        }

        /// <summary>
        /// Returns if typeof(GUIView).IsAssignableFrom(type).
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsAssignableFrom(Type type) => GuiViewType.IsAssignableFrom(type);

        internal GUIView GuiView { get; }

        /// <summary>
        /// Does this GUIViewProxy have a GUIView.
        /// </summary>
        public bool IsValid => GuiView != null;

        /// <summary>
        /// Are the underlying GUIView's window and the window's root view valid.
        /// </summary>
        public bool IsWindowAndRootViewValid => GuiView.window != null && GuiView.window.rootView != null;

        /// <summary>
        /// Position of the GUIView.
        /// </summary>
        public Rect Position => GuiView.position;

        internal GUIViewProxy(GUIView guiView)
        {
            GuiView = guiView;
        }

        /// <summary>
        /// Calls GUIView.Repaint().
        /// </summary>
        public void Repaint() => GuiView.Repaint();

        /// <summary>
        /// Calls GUIView.RepaintImmediately().
        /// </summary>
        public void RepaintImmediately() => GuiView.RepaintImmediately();

        /// <summary>
        /// Is the type instance of underlying HostView's actual view.
        /// </summary>
        /// <param name="editorWindowType"></param>
        /// <returns></returns>
        public bool IsActualViewAssignableTo(Type editorWindowType)
        {
            var hostView = GuiView as HostView;
            return hostView != null && hostView.actualView != null && editorWindowType.IsInstanceOfType(hostView.actualView);
        }

        /// <summary>
        /// Is the GUIView docked to the Editor.
        /// </summary>
        /// <returns></returns>
        public bool IsDockedToEditor()
        {
            var hostView = GuiView as HostView;
            return hostView == null || hostView.window == null || hostView.window.showMode == ShowMode.MainWindow;
        }

        /// <summary>
        /// Is the type instance of the underlying GUIView.
        /// </summary>
        /// <param name="targetViewType"></param>
        /// <returns></returns>
        public bool IsGUIViewAssignableTo(Type targetViewType) => targetViewType.IsInstanceOfType(GuiView);
    }

    internal class GUIViewProxyComparer : IEqualityComparer<GUIViewProxy>
    {
        public bool Equals(GUIViewProxy x, GUIViewProxy y) => x.GuiView == y.GuiView;
        public int GetHashCode(GUIViewProxy obj) => obj.GuiView.GetHashCode();
    }

    internal static class EditorWindowExtension
    {
        public static GUIViewProxy GetParent(this EditorWindow window) => new GUIViewProxy(window.m_Parent);
    }
}
