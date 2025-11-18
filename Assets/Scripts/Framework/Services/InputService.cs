using System;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Framework.Services
{
    public interface IInput
    {
        bool IsBlocked { set; }
        Subject<Vector2> OnMouseMoved { get; }
        Subject<Unit> OnClicked { get; }
    }
    public class InputService : IDisposable, IInput
    {
        private readonly InputControls _controls;

        public Subject<Vector2> OnMouseMoved { get; } = new();
        public Subject<Unit> OnClicked { get; } = new();

        public InputService()
        {
            IsBlocked = true;
            _controls = new InputControls();
            _controls.Enable();

            _controls.UI.Point.performed += OnMouseMove;
            _controls.UI.Click.performed += OnClick;
        }

        public bool IsBlocked { get; set; }

        private void OnMouseMove(InputAction.CallbackContext context)
        {
            if (IsBlocked) return;
            OnMouseMoved.OnNext(context.ReadValue<Vector2>());
        }

        private void OnClick(InputAction.CallbackContext _)
        {
            if (IsBlocked) return;
            OnClicked.OnNext(Unit.Default);
        }
        
        public void Dispose()
        {
            _controls.UI.Point.performed -= OnMouseMove;
            _controls.UI.Click.performed -= OnClick;
            
            _controls.Disable();
            _controls?.Dispose();

            OnMouseMoved.Dispose();
            OnClicked.Dispose();
        }
    }
}