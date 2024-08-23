using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewBehaviourScript : MonoBehaviour
{
    [Header ("Line Renderers")]

    [SerializeField] private LineRenderer _leftLineRenderer;
    [SerializeField] private LineRenderer _rightLineRenderer;

    [Header("Transform Reference")]

    [SerializeField] private Transform _leftStartPosition;
    [SerializeField] private Transform _rightStartPosition;
    [SerializeField] private Transform _centerPosition;
    [SerializeField] private Transform _idlePosition;

    [Header("Slingshot Stats")]
    [SerializeField] private float _maxDistance =  3.5f;

    [Header("Scripts")]

    [SerializeField] private SlingShotArea _slingShotArea;


    private Vector2 _slingShotLinePositions;

    private bool _clickedWithinArea;

    private void Awake()
    {
        _leftLineRenderer.enabled = false;
        _rightLineRenderer.enabled = false;
    }

    private void Update()
    {

        if (Mouse.current.leftButton.wasPressedThisFrame && _slingShotArea.isWithinSlingshotArea())
        {
           _clickedWithinArea = true;
        }
        
        if (Mouse.current.leftButton.isPressed && _clickedWithinArea)
        {
            DrawSlingShot();
        }

        if(Mouse.current.leftButton.wasReleasedThisFrame)
        {
            _clickedWithinArea = false;
            SetLines(_idlePosition.position);
        }

    }

    private void DrawSlingShot()
    {
        // Draw lines from the slingshot to the mouse position

        if ( !_leftLineRenderer.enabled && !_rightLineRenderer.enabled)
        {
            _leftLineRenderer.enabled = true;
            _rightLineRenderer.enabled = true;
        }

        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        _slingShotLinePositions = _centerPosition.position + Vector3.ClampMagnitude(touchPosition - _centerPosition.position , _maxDistance);

        SetLines(_slingShotLinePositions);
    }

    private void SetLines(Vector2 position)
    {
        _leftLineRenderer.SetPosition(0, position);
        _leftLineRenderer.SetPosition(1, _leftStartPosition.position);

        _rightLineRenderer.SetPosition(0, position);
        _rightLineRenderer.SetPosition(1, _rightStartPosition.position);
    }
    
}
