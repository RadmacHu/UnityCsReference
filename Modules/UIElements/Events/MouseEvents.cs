// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

namespace UnityEngine.UIElements
{
    public interface IMouseEvent
    {
        EventModifiers modifiers { get; }
        Vector2 mousePosition { get; }
        Vector2 localMousePosition { get; }
        Vector2 mouseDelta { get; }
        int clickCount { get; }
        int button { get; }

        bool shiftKey { get; }
        bool ctrlKey { get; }
        bool commandKey { get; }
        bool altKey { get; }
        bool actionKey { get; }
    }

    internal interface IMouseEventInternal
    {
        bool triggeredByOS { get; set; }
        bool recomputeTopElementUnderMouse { get; set; }
    }

    public abstract class MouseEventBase<T> : EventBase<T>, IMouseEvent, IMouseEventInternal where T : MouseEventBase<T>, new()
    {
        public EventModifiers modifiers { get; protected set; }
        public Vector2 mousePosition { get; protected set; }
        public Vector2 localMousePosition { get; internal set; }
        public Vector2 mouseDelta { get; protected set; }
        public int clickCount { get; protected set; }
        public int button { get; protected set; }
        public int pointerId { get; protected set; }

        public bool shiftKey
        {
            get { return (modifiers & EventModifiers.Shift) != 0; }
        }

        public bool ctrlKey
        {
            get { return (modifiers & EventModifiers.Control) != 0; }
        }

        public bool commandKey
        {
            get { return (modifiers & EventModifiers.Command) != 0; }
        }

        public bool altKey
        {
            get { return (modifiers & EventModifiers.Alt) != 0; }
        }

        public bool actionKey
        {
            get
            {
                if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
                {
                    return commandKey;
                }
                else
                {
                    return ctrlKey;
                }
            }
        }

        bool IMouseEventInternal.triggeredByOS { get; set; }

        bool IMouseEventInternal.recomputeTopElementUnderMouse { get; set; }

        protected override void Init()
        {
            base.Init();
            LocalInit();
        }

        void LocalInit()
        {
            propagation = EventPropagation.Bubbles | EventPropagation.TricklesDown | EventPropagation.Cancellable;
            modifiers = EventModifiers.None;
            mousePosition = Vector2.zero;
            localMousePosition = Vector2.zero;
            mouseDelta = Vector2.zero;
            clickCount = 0;
            button = 0;
            pointerId = -1;
            ((IMouseEventInternal)this).triggeredByOS = false;
            ((IMouseEventInternal)this).recomputeTopElementUnderMouse = true;
        }

        public override IEventHandler currentTarget
        {
            get { return base.currentTarget; }
            internal set
            {
                base.currentTarget = value;

                var element = currentTarget as VisualElement;
                if (element != null)
                {
                    localMousePosition = element.WorldToLocal(mousePosition);
                }
            }
        }

        public static T GetPooled(Event systemEvent)
        {
            T e = GetPooled();
            e.imguiEvent = systemEvent;
            if (systemEvent != null)
            {
                e.modifiers = systemEvent.modifiers;
                e.mousePosition = systemEvent.mousePosition;
                e.localMousePosition = systemEvent.mousePosition;
                e.mouseDelta = systemEvent.delta;
                e.button = systemEvent.button;
                e.clickCount = systemEvent.clickCount;
                ((IMouseEventInternal)e).triggeredByOS = true;
                ((IMouseEventInternal)e).recomputeTopElementUnderMouse = true;
            }
            return e;
        }

        public static T GetPooled(Vector2 position, int button, int clickCount, Vector2 delta,
            EventModifiers modifiers = EventModifiers.None, int pointerId = -1)
        {
            return GetPooled(position, button, clickCount, delta, modifiers, pointerId, fromOS: false);
        }

        internal static T GetPooled(Vector2 position, int button, int clickCount, Vector2 delta,
            EventModifiers modifiers = EventModifiers.None, int pointerId = -1, bool fromOS = false)
        {
            T e = GetPooled();

            e.modifiers = EventModifiers.None;
            e.mousePosition = position;
            e.localMousePosition = position;
            e.mouseDelta = delta;
            e.button = 0;
            e.clickCount = clickCount;
            e.pointerId = pointerId;
            ((IMouseEventInternal)e).triggeredByOS = fromOS;
            ((IMouseEventInternal)e).recomputeTopElementUnderMouse = true;

            return e;
        }

        internal static T GetPooled(IMouseEvent triggerEvent, Vector2 mousePosition, bool recomputeTopElementUnderMouse)
        {
            if (triggerEvent != null)
            {
                return GetPooled(triggerEvent);
            }

            T e = GetPooled();
            e.mousePosition = mousePosition;
            ((IMouseEventInternal)e).recomputeTopElementUnderMouse = recomputeTopElementUnderMouse;
            return e;
        }

        public static T GetPooled(IMouseEvent triggerEvent)
        {
            T e = EventBase<T>.GetPooled(triggerEvent as EventBase);
            if (triggerEvent != null)
            {
                e.modifiers = triggerEvent.modifiers;
                e.mousePosition = triggerEvent.mousePosition;
                e.localMousePosition = triggerEvent.mousePosition;
                e.mouseDelta = triggerEvent.mouseDelta;
                e.button = triggerEvent.button;
                e.clickCount = triggerEvent.clickCount;

                IMouseEventInternal mouseEventInternal = triggerEvent as IMouseEventInternal;
                if (mouseEventInternal != null)
                {
                    ((IMouseEventInternal)e).triggeredByOS = mouseEventInternal.triggeredByOS;
                    ((IMouseEventInternal)e).recomputeTopElementUnderMouse = false;
                }
            }
            return e;
        }

        protected MouseEventBase()
        {
            LocalInit();
        }
    }

    public class MouseDownEvent : MouseEventBase<MouseDownEvent>
    {
    }

    public class MouseUpEvent : MouseEventBase<MouseUpEvent>
    {
    }

    public class MouseMoveEvent : MouseEventBase<MouseMoveEvent>
    {
    }

    public class ContextClickEvent : MouseEventBase<ContextClickEvent>
    {
    }

    public class WheelEvent : MouseEventBase<WheelEvent>
    {
        public Vector3 delta { get; private set; }

        public new static WheelEvent GetPooled(Event systemEvent)
        {
            WheelEvent e = MouseEventBase<WheelEvent>.GetPooled(systemEvent);
            e.imguiEvent = systemEvent;
            if (systemEvent != null)
            {
                e.delta = systemEvent.delta;
            }
            return e;
        }

        protected override void Init()
        {
            base.Init();
            LocalInit();
        }

        void LocalInit()
        {
            delta = Vector3.zero;
        }

        public WheelEvent()
        {
            LocalInit();
        }
    }

    public class MouseEnterEvent : MouseEventBase<MouseEnterEvent>
    {
        protected override void Init()
        {
            base.Init();
            LocalInit();
        }

        void LocalInit()
        {
            propagation = EventPropagation.TricklesDown | EventPropagation.Cancellable;
        }

        public MouseEnterEvent()
        {
            LocalInit();
        }
    }

    public class MouseLeaveEvent : MouseEventBase<MouseLeaveEvent>
    {
        protected override void Init()
        {
            base.Init();
            LocalInit();
        }

        void LocalInit()
        {
            propagation = EventPropagation.TricklesDown | EventPropagation.Cancellable;
        }

        public MouseLeaveEvent()
        {
            LocalInit();
        }
    }

    public class MouseEnterWindowEvent : MouseEventBase<MouseEnterWindowEvent>
    {
        protected override void Init()
        {
            base.Init();
            LocalInit();
        }

        void LocalInit()
        {
            propagation = EventPropagation.Cancellable;
        }

        public MouseEnterWindowEvent()
        {
            LocalInit();
        }
    }

    public class MouseLeaveWindowEvent : MouseEventBase<MouseLeaveWindowEvent>
    {
        protected override void Init()
        {
            base.Init();
            LocalInit();
        }

        void LocalInit()
        {
            propagation = EventPropagation.Cancellable;
        }

        public MouseLeaveWindowEvent()
        {
            LocalInit();
        }
    }

    public class MouseOverEvent : MouseEventBase<MouseOverEvent>
    {
    }

    public class MouseOutEvent : MouseEventBase<MouseOutEvent>
    {
    }

    public class ContextualMenuPopulateEvent : MouseEventBase<ContextualMenuPopulateEvent>
    {
        public DropdownMenu menu { get; private set; }
        public EventBase triggerEvent { get; private set; }

        ContextualMenuManager m_ContextualMenuManager;

        public static ContextualMenuPopulateEvent GetPooled(EventBase triggerEvent, DropdownMenu menu, IEventHandler target, ContextualMenuManager menuManager)
        {
            ContextualMenuPopulateEvent e = GetPooled(triggerEvent);
            if (triggerEvent != null)
            {
                triggerEvent.Acquire();
                e.triggerEvent = triggerEvent;

                IMouseEvent mouseEvent = triggerEvent as IMouseEvent;
                if (mouseEvent != null)
                {
                    e.modifiers = mouseEvent.modifiers;
                    e.mousePosition = mouseEvent.mousePosition;
                    e.localMousePosition = mouseEvent.mousePosition;
                    e.mouseDelta = mouseEvent.mouseDelta;
                    e.button = mouseEvent.button;
                    e.clickCount = mouseEvent.clickCount;
                }

                IMouseEventInternal mouseEventInternal = triggerEvent as IMouseEventInternal;
                if (mouseEventInternal != null)
                {
                    ((IMouseEventInternal)e).triggeredByOS = mouseEventInternal.triggeredByOS;
                }
            }

            e.target = target;
            e.menu = menu;
            e.m_ContextualMenuManager = menuManager;

            return e;
        }

        protected override void Init()
        {
            base.Init();
            LocalInit();
        }

        void LocalInit()
        {
            menu = null;
            m_ContextualMenuManager = null;

            if (triggerEvent != null)
            {
                triggerEvent.Dispose();
                triggerEvent = null;
            }
        }

        public ContextualMenuPopulateEvent()
        {
            LocalInit();
        }

        protected internal override void PostDispatch(IPanel panel)
        {
            if (!isDefaultPrevented && m_ContextualMenuManager != null)
            {
                menu.PrepareForDisplay(triggerEvent);
                m_ContextualMenuManager.DoDisplayMenu(menu, triggerEvent);
            }

            base.PostDispatch(panel);
        }
    }
}
